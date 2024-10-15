using AppCoreAPI.Data.Entities;
using AppCoreAPI.Dtos;
using AppCoreAPI.Errors;
using AppCoreAPI.Extensions;
using AppCoreAPI.Helpers;
using AppCoreAPI.SeedWorks.Interfaces;
using AppCoreAPI.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AppCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SharedController : ControllerBase
    {
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public SharedController(IGoogleDriveService googleDriveService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _googleDriveService = googleDriveService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> GetShortUrl([FromQuery] FileOrFolderParams fileOrFolderParams)
        {
            var entity = await _unitOfWork.SharedToUserRepository.GetSharedToUserByUrl(fileOrFolderParams.Url, User?.Identity?.Name!);
            if (entity == null)
                return Unauthorized(new ApiResponse(401, "You are Unauthorized"));

            fileOrFolderParams.Path = entity.FullPath;

            var list = await _googleDriveService.GetFileAndFolders(fileOrFolderParams);

            Response.AddPaginationHeader(list.CurrentPage, list.PageSize, list.TotalCount, list.TotalPages);

            return Ok(new
            {
                data = list,
                parentPath = entity.FullPath
            });
        }

        [HttpPost]
        public async Task<ActionResult> PostShortUrl(SharedToUserAddDto sharedToUser)
        {
            sharedToUser.Url = Guid.NewGuid().ToString().Substring(0, 8);
            sharedToUser.OwnerUserName = User?.Identity?.Name!;
            var sharedToUserEntity = _mapper.Map<SharedToUserAddDto, SharedToUser>(sharedToUser);
            if (sharedToUser.SharedUserName != null || sharedToUser?.SharedUserName?.Length != 0)
            {
                foreach (var username in sharedToUser?.SharedUserName!)
                {
                    // khong them chinh no
                    if (User?.Identity?.Name != username)
                    {
                        var entity = new SharedToUser
                        {
                            FullPath = sharedToUserEntity.FullPath,
                            IsFolder = sharedToUserEntity.IsFolder,
                            Name = sharedToUserEntity.Name,
                            OwnerUserName = sharedToUserEntity.OwnerUserName,
                            SharedUserName = username,
                            ShortUrl = sharedToUserEntity.ShortUrl,
                            Url = sharedToUserEntity.Url
                        };
                        _unitOfWork.SharedToUserRepository.Add(entity);
                    }
                }
                if (await _unitOfWork.Complete())
                    return Ok(new
                    {
                        url = sharedToUser.Url,
                        parentPath = sharedToUserEntity.FullPath//khong su dung
                    });
                else
                    return BadRequest(new ApiResponse(400, "Error while add entity to shared database"));
            }
            else
            {
                return NoContent();
            }
        }
    }
}
