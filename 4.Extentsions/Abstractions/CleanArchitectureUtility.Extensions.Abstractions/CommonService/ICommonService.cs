using CleanArchitectureUtility.Extensions.Abstractions.Caching;
using CleanArchitectureUtility.Extensions.Abstractions.ObjectMappers;
using CleanArchitectureUtility.Extensions.Abstractions.Serializers;
using CleanArchitectureUtility.Extensions.Abstractions.Translations;
using CleanArchitectureUtility.Extensions.Abstractions.UsersManagements;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureUtility.Extensions.Abstractions.CommonService
{
    public interface ICommonService
    {
        public ITranslator Translator { get; }
        public ICacheAdapter CacheAdapter { get; }
        public IMapperAdapter MapperFacade { get; }
        public ILoggerFactory LoggerFactory { get; }
        public IJsonSerializer Serializer { get; }
        public IUserInfoService UserInfoService { get; }
    }
}