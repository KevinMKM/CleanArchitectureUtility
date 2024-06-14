using CleanArchitectureUtility.Core.Abstractions.Serializers;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureUtility.Utilities.Serializers.Microsoft;

public static class MicrosoftSerializerServiceCollectionX
{
    public static IServiceCollection AddMicrosoftSerializer(this IServiceCollection services)
        => services.AddSingleton<IJsonSerializer, MicrosoftSerializer>();
}