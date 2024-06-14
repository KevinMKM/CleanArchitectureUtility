using System.ComponentModel;

namespace CleanArchitectureUtility.Utilities.Extensions;

public static class EnumX
{
    public static string? GetEnumDescription(this Enum enumValue)
    {
        var memberInfo = enumValue.GetType().GetField(enumValue.ToString());
        var attributes = memberInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false);
        var description = attributes != null ? (attributes.FirstOrDefault() as DescriptionAttribute)?.Description : enumValue.ToString();
        return description;
    }
}