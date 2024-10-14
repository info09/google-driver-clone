using AppCoreAPI.Data.Entities;
using AppCoreAPI.Dtos;
using AppCoreAPI.Errors;
using AppCoreAPI.SeedWorks.Interfaces;
using AppCoreAPI.Services.Interfaces;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(ITokenService tokenService, IUnitOfWork unitOfWork, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
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
    }
}
