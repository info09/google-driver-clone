using AppCoreAPI.Data.Entities;
using AppCoreAPI.Dtos;
using AutoMapper;

namespace AppCoreAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<RootFolder, RootFolderDto>();
            CreateMap<RootFolderDto, RootFolder>();
            CreateMap<AppUser, UserDto>();
            CreateMap<SharedToUserAddDto, SharedToUser>().ForMember(d => d.ShortUrl, o => o.MapFrom<ShareUrlResolver>());
        }
    }
}
