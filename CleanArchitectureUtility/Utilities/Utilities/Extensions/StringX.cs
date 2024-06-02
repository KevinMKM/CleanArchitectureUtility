using System.Text;
using System.Text.RegularExpressions;

namespace CleanArchitectureUtility.Utilities.Utilities.Extensions;

public static class StringX
{
    public static long ToSafeLong(this string input, long replacement = long.MinValue) => long.TryParse(input, out var result) ? result : replacement;

    public static long? ToSafeNullableLong(this string input) => long.TryParse(input, out var result) ? result : null;

    public static int ToSafeInt(this string input, int replacement = int.MinValue) => int.TryParse(input, out var result) ? result : replacement;

    public static int? ToSafeNullableInt(this string input) => int.TryParse(input, out var result) ? result : null;

    public static string ToStringOrEmpty(this string? input) => input ?? string.Empty;

    public static string ToUnderscoreCase(this string input) => string.Concat(input.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();

    public static byte[] ToByteArray(this string input) => Encoding.UTF8.GetBytes(input);

    public static string FromByteArray(this byte[] input) => Encoding.UTF8.GetString(input);

    public static bool IsNumeric(this string nationalCode) => new Regex(@"\d+").IsMatch(nationalCode);

    public static bool IsLengthBetween(this string input, int minLength, int maxLength) => input.Length <= maxLength && input.Length >= minLength;

    public static bool IsLengthLessThan(this string input, int length) => input.Length < length;

    public static bool IsLengthLessThanOrEqual(this string input, int length) => input.Length <= length;

    public static bool IsLengthGreaterThan(this string input, int length) => input.Length > length;

    public static bool IsLengthGreaterThanOrEqual(this string input, int length) => input.Length >= length;

    public static bool IsLengthEqual(this string input, int length) => input.Length == length;

    public static string AggregateToString(this IEnumerable<string> source, string splitter)
        => source.Where(s => !string.IsNullOrWhiteSpace(s)).Aggregate((s1, s2) => $"{s1}{splitter}{s2}");

    public static string AggregateToString(this List<string> source, string splitter) => source.AsEnumerable().AggregateToString(splitter);
}