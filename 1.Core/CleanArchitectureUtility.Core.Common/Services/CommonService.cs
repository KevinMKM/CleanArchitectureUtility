using CleanArchitectureUtility.Extensions.Abstractions.Caching;
using CleanArchitectureUtility.Extensions.Abstractions.CommonService;
using CleanArchitectureUtility.Extensions.Abstractions.ObjectMappers;
using CleanArchitectureUtility.Extensions.Abstractions.Serializers;
using CleanArchitectureUtility.Extensions.Abstractions.Translations;
using CleanArchitectureUtility.Extensions.Abstractions.UsersManagements;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureUtility.Core.Common.Services
{
    public class CommonService : ICommonService
    {
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

        public ITranslator Translator { get; }
        public ICacheAdapter CacheAdapter { get; }
        public IMapperAdapter MapperFacade { get; }
        public ILoggerFactory LoggerFactory { get; }
        public IJsonSerializer Serializer { get; }
        public IUserInfoService UserInfoService { get; }
    }
}