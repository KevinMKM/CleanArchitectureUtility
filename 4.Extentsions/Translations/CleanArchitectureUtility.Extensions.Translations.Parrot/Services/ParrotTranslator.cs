using System.Globalization;
using CleanArchitectureUtility.Extensions.Abstractions.Translations;
using CleanArchitectureUtility.Extensions.Translations.Parrot.Database;
using CleanArchitectureUtility.Extensions.Translations.Parrot.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CleanArchitectureUtility.Extensions.Translations.Parrot.Services;

public class ParrotTranslator : ITranslator, IDisposable
{
    private readonly string _currentCulture;
    private readonly ParrotSqlRepository _localizer;
    private readonly ILogger<ParrotTranslator> _logger;

    public ParrotTranslator(IOptions<ParrotTranslatorOptions> configuration, ILogger<ParrotTranslator> logger)
    {
        _currentCulture = CultureInfo.CurrentCulture.ToString();
        _logger = logger;
        if (string.IsNullOrWhiteSpace(_currentCulture))
        {
            _currentCulture = "en-US";
            _logger.LogInformation("Parrot Translator current culture is null and set to en-US");
        }
        _localizer = new ParrotSqlRepository(configuration.Value, logger);
        _logger.LogInformation($"Parrot Translator Start working with culture {_currentCulture}");
    }

    public string this[string name]
    {
        get => GetString(name);
        set => throw new NotImplementedException();
    }

    public string this[CultureInfo culture, string name]
    {
        get => GetString(culture, name);
        set => throw new NotImplementedException();
    }

    public string this[string name, params string[] arguments]
    {
        get => GetString(name, arguments);
        set => throw new NotImplementedException();
    }

    public string this[CultureInfo culture, string name, params string[] arguments]
    {
        get => GetString(culture, name, arguments);
        set => throw new NotImplementedException();
    }

    public string this[char separator, params string[] names]
    {
        get => GetConcatString(separator, names);
        set => throw new NotImplementedException();
    }

    public string this[CultureInfo culture, char separator, params string[] names]
    {
        get => GetConcatString(culture, separator, names);
        set => throw new NotImplementedException();
    }

    public string GetString(string name)
    {
        _logger.LogTrace($"Parrot Translator GetString with name {name}");
        return _localizer.Get(name, _currentCulture);
    }

    public string GetString(CultureInfo culture, string name)
    {
        _logger.LogTrace($"Parrot Translator GetString  with culture {culture} name {name}");
        return _localizer.Get(name, culture is null ? _currentCulture : culture.ToString());
    }

    public string GetString(string pattern, params string[] arguments)
    {
        _logger.LogTrace($"Parrot Translator GetString with pattern {pattern} and arguments {arguments}");
        for (var i = 0; i < arguments.Length; i++) 
            arguments[i] = GetString(arguments[i]);

        pattern = GetString(pattern);
        for (var i = 0; i < arguments.Length; i++)
        {
            var placeHolder = $"{{{i}}}";
            pattern = pattern.Replace(placeHolder, arguments[i]);
        }

        return pattern;
    }

    public string GetString(CultureInfo culture, string pattern, params string[] arguments)
    {
        _logger.LogTrace($"Parrot Translator GetString with culture {culture} and  pattern {pattern} and arguments {arguments}");
        for (var i = 0; i < arguments.Length; i++) 
            arguments[i] = GetString(culture, arguments[i]);

        pattern = GetString(culture, pattern);
        for (var i = 0; i < arguments.Length; i++)
        {
            var placeHolder = $"{{{i}}}";
            pattern = pattern.Replace(placeHolder, arguments[i]);
        }

        return pattern;
    }

    public string GetConcatString(char separator = ' ', params string[] names)
    {
        _logger.LogTrace($"Parrot Translator GetConcatString with separator {separator} and names {names}");
        for (var i = 0; i < names.Length; i++) 
            names[i] = GetString(names[i]);

        return string.Join(separator, names);
    }

    public string GetConcatString(CultureInfo culture, char separator = ' ', params string[] names)
    {
        _logger.LogTrace($"Parrot Translator GetConcatString with culture {culture} and separator {separator} and names {names}");
        for (var i = 0; i < names.Length; i++) 
            names[i] = GetString(culture, names[i]);

        return string.Join(separator, names);
    }

    public void Dispose() => _logger.LogInformation($"Parrot Translator Stop working with culture {_currentCulture}");
}