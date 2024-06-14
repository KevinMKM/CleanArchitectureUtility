using CleanArchitectureUtility.Core.Abstractions.Serializers;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureUtility.Utilities.Serializers.NewtonSoft;

public static class NewtonSoftSerializerServiceCollectionX
{
    public static IServiceCollection AddNewtonSoftSerializer(this IServiceCollection services)
        => services.AddSingleton<IJsonSerializer, NewtonSoftSerializer>();
}