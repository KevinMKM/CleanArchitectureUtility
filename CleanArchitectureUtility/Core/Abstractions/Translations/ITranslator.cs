namespace CleanArchitectureUtility.Core.Abstractions.Translations;

public interface ITranslator
{
    string GetString(string? name);

    string GetString(string? pattern, params string?[] arguments);

    string GetConcatString(char separator = ' ', params string?[] names);
}