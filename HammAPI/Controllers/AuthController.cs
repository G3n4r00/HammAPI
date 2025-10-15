using HammAPI.Models;
using HammAPI.Repository;
using HammAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace HammAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IAuthService _authService;

        public AuthController(IUserRepository userRepo, IAuthService authService)
        {
            _userRepo = userRepo;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var existing = await _userRepo.GetByEmailAsync(dto.Email);
            if (existing != null) return Conflict("Email já cadastrado.");

            var usuario = new Usuario
            {
                PrimeiroNome = dto.PrimeiroNome,
                UltimoNome = dto.UltimoNome,
                Email = dto.Email,
                SenhaHash = _authService.HashPassword(dto.Senha)
            };

            await _userRepo.AddAsync(usuario);
            return CreatedAtAction(null, new { id = usuario.Id }, new { usuario.Id, usuario.Email });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email);
            if (user == null) return Unauthorized("Credenciais inválidas.");

            if (!_authService.VerifyPassword(dto.Senha, user.SenhaHash))
                return Unauthorized("Credenciais inválidas.");

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { token });
        }
    }

    public record RegisterDto(string PrimeiroNome, string UltimoNome, string Email, string Senha);
    public record LoginDto(string Email, string Senha);
}
