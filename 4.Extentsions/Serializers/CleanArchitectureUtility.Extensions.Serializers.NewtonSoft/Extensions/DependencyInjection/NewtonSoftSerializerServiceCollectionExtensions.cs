using CleanArchitectureUtility.Extensions.Abstractions.Serializers;
using CleanArchitectureUtility.Extensions.Serializers.NewtonSoft.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureUtility.Extensions.Serializers.NewtonSoft.Extensions.DependencyInjection;

public static class NewtonSoftSerializerServiceCollectionExtensions
{
    public static IServiceCollection AddNewtonSoftSerializer(this IServiceCollection services)
        => services.AddSingleton<IJsonSerializer, NewtonSoftSerializer>();
}