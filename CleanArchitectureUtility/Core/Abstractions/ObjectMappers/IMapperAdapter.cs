namespace CleanArchitectureUtility.Core.Abstractions.ObjectMappers;

public interface IMapperAdapter
{
    TDestination Map<TSource, TDestination>(TSource source);
}