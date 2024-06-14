using CleanArchitectureUtility.Core.Abstractions.Serializers;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureUtility.Utilities.Serializers.EPPlus;

public static class EpPlusExcelSerializerServiceCollectionX
{
    public static IServiceCollection AddEpPlusExcelSerializer(this IServiceCollection services)
        => services.AddSingleton<IExcelSerializer, EpPlusExcelSerializer>();
}