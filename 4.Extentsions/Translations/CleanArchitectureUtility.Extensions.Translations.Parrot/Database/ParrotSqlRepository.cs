﻿using System.Data;
using CleanArchitectureUtility.Extensions.Translations.Parrot.DataModel;
using CleanArchitectureUtility.Extensions.Translations.Parrot.Options;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureUtility.Extensions.Translations.Parrot.Database;
public class ParrotSqlRepository
{
    private readonly IDbConnection _dbConnection;
    private List<LocalizationRecord> _localizationRecords;
    private static readonly object Locker = new();
    private readonly ParrotTranslatorOptions _configuration;
    private readonly ILogger _logger;
    private readonly string _selectCommand = "Select * from [{0}].[{1}]";
    private readonly string _insertCommand = "INSERT INTO [{0}].[{1}]([Key],[Value],[Culture]) VALUES (@Key,@Value,@Culture) select SCOPE_IDENTITY()";

    public ParrotSqlRepository(ParrotTranslatorOptions configuration, ILogger logger)
    {
        _configuration = configuration;
        _logger = logger;
        _dbConnection = new SqlConnection(configuration.ConnectionString);
        if (_configuration.AutoCreateSqlTable)
            CreateTableIfNeeded();

        _selectCommand = string.Format(_selectCommand, configuration.SchemaName, configuration.TableName);
        _insertCommand = string.Format(_insertCommand, configuration.SchemaName, configuration.TableName);
        LoadLocalizationRecords(null);
        SeedData();
        LoadLocalizationRecords(null);
        _ = new Timer(LoadLocalizationRecords, null, TimeSpan.FromMinutes(configuration.ReloadDataIntervalInMinutes), TimeSpan.FromMinutes(configuration.ReloadDataIntervalInMinutes));
    }

    private void CreateTableIfNeeded()
    {
        try
        {
            var table = _configuration.TableName;
            var schema = _configuration.SchemaName;
            _logger.LogInformation($"Parrot Translator try to create table in database with connection {_configuration.ConnectionString}. Schema name is {schema}. Table name is {table}");
            var createTable = $"IF (NOT EXISTS (SELECT *  FROM INFORMATION_SCHEMA.TABLES WHERE " +
                $"TABLE_SCHEMA = '{schema}' AND  TABLE_NAME = '{table}' )) Begin " +
                $"CREATE TABLE [{schema}].[{table}]( " +
                $"[Id] [bigint] NOT NULL Primary Key Identity(1,1)," +
                $"[Key] [nvarchar](255) NOT NULL," +
                $"[Value] [nvarchar](500) NOT NULL," +
                $"[Culture] [nvarchar](5) NULL)" +
                $" End";
            _dbConnection.Execute(createTable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Create table for Parrot Translator Failed");
            throw;
        }
    }

    private void LoadLocalizationRecords(object? state)
    {
        lock (Locker)
        {
            _logger.LogInformation($"Parrot Translator load localization recorded at {DateTime.UtcNow}");
            _localizationRecords = _dbConnection.Query<LocalizationRecord>(_selectCommand, commandType: CommandType.Text).ToList();
            _logger.LogInformation($"Parrot Translator loaded localization recorded at {DateTime.UtcNow}. Total record count is {_localizationRecords.Count}");
        }
    }

    private void SeedData()
    {
        var newItemsInDefaultTranslations = _configuration.DefaultTranslations.
            Where(c => !_localizationRecords.Any(d => d.Key.Equals(c.Key) && d.Culture.Equals(c.Culture)))
            .Select(c => $"(N'{c.Key}',N'{c.Value}',N'{c.Culture}')").ToList();
        var count = newItemsInDefaultTranslations.Count;
        if (count <= 0) 
            return;

        var _command = $"INSERT INTO [{_configuration.SchemaName}].[{_configuration.TableName}]([Key],[Value],[Culture]) VALUES {string.Join(",", newItemsInDefaultTranslations)}";
        using IDbConnection db = new SqlConnection(_configuration.ConnectionString);
        db.Execute(_command, commandType: CommandType.Text);
        _logger.LogInformation($"Parrot Translator Add {count} items to its dictionary from default translations at {DateTime.UtcNow}");
    }

    public string Get(string key, string culture)
    {
        try
        {
            var record = _localizationRecords.FirstOrDefault(c => c.Key == key && c.Culture == culture);
            if (record != null) 
                return record.Value;

            _logger.LogInformation($"The key was not found and was registered with the default value in Parrot Translator. Key is {key} and culture is {culture}");
            record = new LocalizationRecord
            {
                Key = key,
                Culture = culture,
                Value = key
            };

            var parameters = new DynamicParameters();
            parameters.Add("@Key", key);
            parameters.Add("@Culture", culture);
            parameters.Add("@Value", key);
            record.Id = _dbConnection.Query<long>(_insertCommand, param: parameters, commandType: CommandType.Text).FirstOrDefault();
            _localizationRecords.Add(record);
            return record.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get Key value from Sql Server for Parrot Translator Failed");
            throw;
        }
    }
}