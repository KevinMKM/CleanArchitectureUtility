using CleanArchitectureUtility.Core.Abstractions.Caching;
using CleanArchitectureUtility.Core.Abstractions.ObjectMappers;
using CleanArchitectureUtility.Core.Abstractions.Serializers;
using CleanArchitectureUtility.Core.Abstractions.Translations;
using CleanArchitectureUtility.Core.Abstractions.UsersManagement;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureUtility.Utilities.Utilities;

public class CommonService
{
    public readonly ITranslator Translator;
    public readonly ICacheAdapter CacheAdapter;
    public readonly IMapperAdapter MapperFacade;
    public readonly ILoggerFactory LoggerFactory;
    public readonly IJsonSerializer Serializer;
    public readonly IUserInfoService UserInfoService;

    public CommonService(ITranslator translator,
        ILoggerFactory loggerFactory,
        IJsonSerializer serializer,
        IUserInfoService userInfoService,
        ICacheAdapter cacheAdapter,
        IMapperAdapter mapperFacade)
    {
        Translator = translator;
        LoggerFactory = loggerFactory;
        Serializer = serializer;
        UserInfoService = userInfoService;
        CacheAdapter = cacheAdapter;
        MapperFacade = mapperFacade;
    }
}