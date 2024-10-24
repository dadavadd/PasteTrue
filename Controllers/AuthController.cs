using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using PasteTrue.Data;
using PasteTrue.DTOs.User;
using PasteTrue.Models;
using PasteTrue.Services.Interfaces;

namespace PasteTrue.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ApiControllerBase
    {
        private readonly IJwtTokenService _tokenService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IJwtTokenService tokenService,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AuthController> logger)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<NewUserDto>> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = new User
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email,
                    CreatedAt = DateTime.UtcNow
                };

                var createdUser = await _userManager.CreateAsync(user, registerDto.Password);
                if (!createdUser.Succeeded)
                {
                    return BadRequest(new { createdUser.Errors });
                }

                var roleResult = await _userManager.AddToRoleAsync(user, "User");
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    return StatusCode(500, new { roleResult.Errors });
                }

                return Ok(new NewUserDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user");
                return StatusCode(500, "Error registering user");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<NewUserDto>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _userManager.Users
                    .FirstOrDefaultAsync(t => t.UserName == loginDto.UserName);

                if (user == null)
                    return Unauthorized(new { Message = "Invalid username or password" });

                var result = await _signInManager
                    .CheckPasswordSignInAsync(user, loginDto.Password, false);

                if (!result.Succeeded)
                    return Unauthorized(new { Message = "Invalid username or password" });

                return Ok(new NewUserDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login attempt");
                return StatusCode(500, "Error during login attempt");
            }
        }
    }
}
