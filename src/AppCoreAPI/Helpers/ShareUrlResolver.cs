using AppCoreAPI.Data.Entities;
using AppCoreAPI.Dtos;
using AutoMapper;
using AutoMapper.Execution;

namespace AppCoreAPI.Helpers
{
    public class ShareUrlResolver : IValueResolver<SharedToUserAddDto, SharedToUser, string>
    {
        private readonly IConfiguration _config;
        public ShareUrlResolver(IConfiguration config)
        {
            _config = config;
        }
        public string Resolve(SharedToUserAddDto source, SharedToUser destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.Url))
            {
                return _config["ApiUrl"] + "huytq/Shared/" + source.Url;
            }

            return null;
        }
    }
}
