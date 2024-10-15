using AppCoreAPI.Data.Entities;
using AppCoreAPI.Dtos;
using AppCoreAPI.Errors;
using AppCoreAPI.SeedWorks.Interfaces;
using AppCoreAPI.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;

        public AccountController(ITokenService tokenService, IGoogleDriveService googleDriveService, IUnitOfWork unitOfWork, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper)
        {
            _tokenService = tokenService;
            _googleDriveService = googleDriveService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(i => i.UserName == loginDto.UserName.ToLower());
            if (user == null)
                return BadRequest(new ApiResponse(400, "Invalid Username"));

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
                return BadRequest(new ApiResponse(400, "Invalid password"));

            return new UserDto()
            {
                UserName = user.UserName,
                DisplayName = user.DisplayName,
                Token = await _tokenService.CreateTokenAsync(user)
            };
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.UserName))
                return BadRequest("Username is taken");

            var user = new AppUser
            {
                UserName = registerDto.UserName.ToLower(),
                DisplayName = registerDto.DisplayName,
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded)
                return BadRequest(result.Errors);

            var rootFolder = new RootFolderDto
            {
                Name = Guid.NewGuid().ToString().Substring(0, 10),
                UserId = user.Id
            };

            var rootFolderDb = _mapper.Map<RootFolderDto, RootFolder>(rootFolder);
            _unitOfWork.RootFolderRepository.Add(rootFolderDb);

            if (await _unitOfWork.Complete())
            {
                // tao thu muc root
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", rootFolder.Name);
                await _googleDriveService.CreateDirectory(path);

                return Ok(new UserDto
                {
                    UserName = user.UserName,
                    DisplayName = user.DisplayName,
                    Token = await _tokenService.CreateTokenAsync(user)
                });
            }

            return BadRequest(new ApiResponse(400, "Can not add root folder into database"));
        }

        [HttpGet("username-exists")]
        public async Task<ActionResult<bool>> CheckUserNameExistAsync([FromQuery] string userName)
        {
            return await _userManager.FindByNameAsync(userName) != null;
        }

        [HttpGet("get-users")]
        public async Task<ActionResult> GetUsers(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                var users = await _userManager.Users.ToListAsync();
                return Ok(_mapper.Map<List<AppUser>, List<UserDto>>(users));
            }
            else
            {
                var users = await _userManager.Users.Where(x => x.UserName.Contains(userName.ToLower())).ToListAsync();
                return Ok(_mapper.Map<List<AppUser>, List<UserDto>>(users));
            }
        }


        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}
