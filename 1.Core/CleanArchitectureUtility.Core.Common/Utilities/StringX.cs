namespace CleanArchitectureUtility.Core.Common.Utilities
{
    public static class StringX
    {
        public static bool IsLengthBetween(this string source, int min, int max)
            => source.Length >= min && source.Length <= max;

        public static string AggregateX(this List<string>? source, string seed)
            => source == null || !source.Any() ? string.Empty : source.Aggregate((s1, s2) => $"{s1}{seed}{s2}");

        public static string AggregateX(this IEnumerable<string>? source, string seed)
            => source?.ToList().AggregateX(seed);
    }
}