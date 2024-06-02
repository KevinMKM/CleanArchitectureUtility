namespace CleanArchitectureUtility.Utilities.Utilities.Extensions;

public static class GuidX
{
    public static bool IsNullOrEmpty(this Guid? guid) => guid == null || guid == Guid.Empty;

    public static bool IsEmpty(this Guid guid) => guid == Guid.Empty;
}