using AppCoreAPI.Data.Entities;
using AppCoreAPI.Dtos;
using AppCoreAPI.Errors;
using AppCoreAPI.Extensions;
using AppCoreAPI.SeedWorks.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RootFolderController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RootFolderController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<ActionResult> PostNewRoot()
        {
            var rootFolder = new RootFolderDto
            {
                Name = Guid.NewGuid().ToString().Substring(0, 10),
                UserId = User.GetUserId(),
            };

            var rootFolderDb = _mapper.Map<RootFolderDto, RootFolder>(rootFolder);
            _unitOfWork.RootFolderRepository.Add(rootFolderDb);

            if(await _unitOfWork.Complete())
                return Ok(rootFolder);

            return BadRequest(new ApiResponse(400, "Can not add"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RootFolder>> GetRootFoler(int id)
        {
            var rootFolder = await _unitOfWork.RootFolderRepository.GetRootFolder(id);

            return Ok(rootFolder);
        }
    }
}
