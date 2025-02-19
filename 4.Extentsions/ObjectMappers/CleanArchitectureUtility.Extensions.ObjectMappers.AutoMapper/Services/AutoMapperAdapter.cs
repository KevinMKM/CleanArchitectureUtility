using AutoMapper;
using CleanArchitectureUtility.Extensions.Abstractions.ObjectMappers;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureUtility.Extensions.ObjectMappers.AutoMapper.Services;

public class AutoMapperAdapter : IMapperAdapter
{
    private readonly IMapper _mapper;
    private readonly ILogger<AutoMapperAdapter> _logger;

    public AutoMapperAdapter(IMapper mapper, ILogger<AutoMapperAdapter> logger)
    {
        _mapper = mapper;
        _logger = logger;
        _logger.LogInformation("AutoMapper Adapter Start working");
    }

    public TDestination Map<TSource, TDestination>(TSource source)
    {
        _logger.LogTrace($"AutoMapper Adapter Map {typeof(TSource)} To {typeof(TDestination)} with data {source}");
        return _mapper.Map<TDestination>(source);
    }
}