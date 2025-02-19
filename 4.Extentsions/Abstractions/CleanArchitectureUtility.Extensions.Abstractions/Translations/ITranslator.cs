using System.Globalization;

namespace CleanArchitectureUtility.Extensions.Abstractions.Translations;

public interface ITranslator
{
    string this[string name] { get; set; }
    string this[CultureInfo culture, string name] { get; set; }

    string this[string name, params string[] arguments] { get; set; }
    string this[CultureInfo culture, string name, params string[] arguments] { get; set; }

    string this[char separator, params string[] names] { get; set; }
    string this[CultureInfo culture, char separator, params string[] names] { get; set; }

    string GetString(string name);
    string GetString(CultureInfo culture, string name);

    string GetString(string pattern, params string[] arguments);
    string GetString(CultureInfo culture, string pattern, params string[] arguments);

    string GetConcatString(char separator = ' ', params string[] names);
    string GetConcatString(CultureInfo culture, char separator = ' ', params string[] names);
}