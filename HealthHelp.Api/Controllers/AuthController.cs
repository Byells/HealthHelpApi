using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Asp.Versioning;
using HealthHelp.Api.Data;
using HealthHelp.Api.Dtos;
using HealthHelp.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;

namespace HealthHelp.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [SwaggerTag("Gerencia a autenticação e o cadastro de usuários")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("register")]
        [SwaggerOperation(
            Summary = "Registrar novo usuário",
            Description = "Cria uma nova conta de usuário no sistema com os dados fornecidos (Nome, Email, Senha, Data de Nascimento)."
        )]
        [SwaggerResponse(200, "Usuário criado com sucesso")]
        [SwaggerResponse(409, "Conflito: Já existe um usuário com este email")]
        [SwaggerResponse(500, "Erro interno ao criar usuário (ex: senha não atende aos requisitos)")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
            {
                return StatusCode(StatusCodes.Status409Conflict, 
                    new { Message = "Usuário com este email já existe." });
            }

            ApplicationUser user = new()
            {
                Email = registerDto.Email,
                UserName = registerDto.Email,
                Name = registerDto.Name,
                DateOfBirth = registerDto.DateOfBirth,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Message = "Criação do usuário falhou.", Errors = result.Errors });
            }

            return Ok(new { Message = "Usuário criado com sucesso!" });
        }

        [HttpPost]
        [Route("login")]
        [SwaggerOperation(
            Summary = "Realizar login",
            Description = "Autentica o usuário com email e senha e retorna um Token JWT (Bearer) para acesso aos endpoints protegidos."
        )]
        [SwaggerResponse(200, "Login realizado com sucesso (Token retornado)")]
        [SwaggerResponse(401, "Credenciais inválidas (Email ou senha incorretos)")]public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                var token = GenerateJwtToken(user);
                
                return Ok(new
                {
                    token = $"Bearer {new JwtSecurityTokenHandler().WriteToken(token)}",
                    expiration = token.ValidTo
                });
            }

            return Unauthorized(new { Message = "Email ou senha inválidos." });
        }
        
        private JwtSecurityToken GenerateJwtToken(ApplicationUser user)
        {
            var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var jwtIssuer = _configuration["JWT:Issuer"];
            var jwtAudience = _configuration["JWT:Audience"];

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
            };
            
            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                expires: DateTime.Now.AddHours(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(jwtKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }
    }
}