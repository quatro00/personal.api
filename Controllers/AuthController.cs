using Farmacia.UI.Data;
using Farmacia.UI.Models.DTO.Auth;
using Farmacia.UI.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Farmacia.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository tokenRepository;
        private readonly RoleManager<ApplicationRole> roleManager;

        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository, RoleManager<ApplicationRole> roleManager)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
            this.roleManager = roleManager;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            try
            {
               

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException.Message); // O devolver un BadRequest(400) si el error es de entrada
            }

        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            int sistemaId = 1;
            //checamos el email
            var identityUser = await userManager.FindByNameAsync(request.username);

            if (identityUser is not null)
            {
                var checkPasswordResult = await userManager.CheckPasswordAsync(identityUser, request.password);

                if (checkPasswordResult)
                {

                    var roles = await userManager.GetRolesAsync(identityUser);
                    List<string> rolesFinal = new List<string>();


                    foreach (var item in roles)
                    {
                        var rols_ = await this.roleManager.FindByNameAsync(item);
                        if (rols_.SistemaId == sistemaId)
                        {
                            rolesFinal.Add(rols_.Name ?? "");
                        }
                    }

                    if(rolesFinal == null)
                    {
                        ModelState.AddModelError("error", "Email o password incorrecto.");
                        return ValidationProblem(ModelState);
                    }

                    if (rolesFinal.Count == 0)
                    {
                        ModelState.AddModelError("error", "Email o password incorrecto.");
                        return ValidationProblem(ModelState);
                    }

                    var jwtToken = tokenRepository.CreateJwtToken(identityUser, roles.ToList());
                    var response = new LoginResponseDto()
                    {
                        Email = identityUser.Email,
                        Roles = roles.ToList(),
                        Token = jwtToken,
                        Nombre = "Juan",
                        Apellidos = "Perez",
                        Username = identityUser.UserName
                    };

                    return Ok(response);
                }

            }

            ModelState.AddModelError("error", "Email o password incorrecto.");
            ModelState.AddModelError("error", "Email o password incorrecto.");
            return ValidationProblem(ModelState);
        }
    }
}
