using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using personal.api.Models;
using personal.api.Models.DTO.Admin.Auth;
using personal.api.Repositories.Interface;

namespace personal.api.Controllers.Admin
{
    [Route("api/administrador/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ITokenRepository tokenRepository;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenRepository tokenRepository
            )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenRepository = tokenRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var user = await userManager.FindByNameAsync(model.email);
            if (user == null)
                return BadRequest("Usuario o contraseña incorrectos.");

            var result = await signInManager.CheckPasswordSignInAsync(user, model.password, false);
            if (!result.Succeeded)
                return Unauthorized("Usuario o contraseña incorrectos.");

            var roles = await userManager.GetRolesAsync(user);
            if (roles.IndexOf("Administrador") == -1)
            {
                ModelState.AddModelError("error", "Email o password incorrecto.");
                return ValidationProblem(ModelState);
            }

            var jwtToken = tokenRepository.CreateJwtToken(user, roles.ToList());
            var response = new LoginResponseDto()
            {
                AccessToken = jwtToken,
                TokenType = "bearer",
                User = new UserDto()
                {
                    Id = user.Id,
                    Name = user.Nombre ?? "" + " " + user.Apellidos ?? "",
                    Avatar = "",
                    Roles = roles.ToList(),
                    Status = "online",
                    Email = user.Email,
                }
            };


            return Ok(response);
        }
    }
}
