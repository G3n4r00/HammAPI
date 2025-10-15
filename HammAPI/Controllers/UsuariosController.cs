using HammAPI.Data;
using HammAPI.DTOs;
using HammAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HammAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly HammAPIDbContext _context;
        private readonly IPasswordHasher<Usuario> _passwordHasher;
        private readonly IConfiguration _configuration;

        public UsuariosController(
            HammAPIDbContext context,
            IPasswordHasher<Usuario> passwordHasher,
            IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        /// <summary>
        /// Registra um novo usuário.
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<UsuarioDTO>> Register([FromBody] CreateUsuarioDTO dto)
        {
            if (await _context.Usuarios.AnyAsync(x => x.Email == dto.Email))
                return Conflict(new { message = "Email já foi cadastrado!" });

            var user = new Usuario
            {
                PrimeiroNome = dto.PrimeiroNome,
                UltimoNome = dto.UltimoNome,
                Email = dto.Email
            };

            user.SenhaHash = _passwordHasher.HashPassword(user, dto.Senha);

            _context.Usuarios.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = user.Id },
                new UsuarioDTO { Id = user.Id, PrimeiroNome = user.PrimeiroNome, UltimoNome = user.UltimoNome, Email = user.Email });
        }

        /// <summary>
        /// Faz login e retorna um token JWT.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] LoginUsuarioDTO dto)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return Unauthorized(new { message = "Credenciais inválidas." });

            var result = _passwordHasher.VerifyHashedPassword(user, user.SenhaHash, dto.Senha);
            if (result == PasswordVerificationResult.Failed)
                return Unauthorized(new { message = "Credenciais inválidas." });

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        /// <summary>
        /// Retorna o perfil do usuário autenticado.
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UsuarioDTO>> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var u = await _context.Usuarios.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.ToString() == userId);

            if (u == null) return NotFound();

            return Ok(new UsuarioDTO
            {
                Id = u.Id,
                PrimeiroNome = u.PrimeiroNome,
                UltimoNome = u.UltimoNome,
                Email = u.Email
            });
        }

        /// <summary>
        /// Atualiza o perfil do usuário autenticado.
        /// </summary>
        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUsuarioDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _context.Usuarios.FirstOrDefaultAsync(x => x.Id.ToString() == userId);
            if (user == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
            {
                if (await _context.Usuarios.AnyAsync(x => x.Email == dto.Email && x.Id != user.Id))
                    return Conflict(new { message = "O Email solicitado já foi cadastrado por outro usuário." });
                user.Email = dto.Email;
            }

            user.PrimeiroNome = dto.PrimeiroNome ?? user.PrimeiroNome;

            if (!string.IsNullOrWhiteSpace(dto.Senha))
                user.SenhaHash = _passwordHasher.HashPassword(user, dto.Senha);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Exclui a própria conta.
        /// </summary>
        [HttpDelete("me")]
        [Authorize]
        public async Task<IActionResult> DeleteProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _context.Usuarios.FindAsync(Guid.Parse(userId));
            if (user == null) return NotFound();

            _context.Usuarios.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Método auxiliar para gerar o JWT
        private string GenerateJwtToken(Usuario user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.PrimeiroNome ?? "")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(4),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Rota apenas para CreatedAtAction no Register
        [NonAction]
        public async Task<ActionResult<UsuarioDTO>> GetById(Guid id)
        {
            var user = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (user == null) return NotFound();

            return new UsuarioDTO
            {
                Id = user.Id,
                PrimeiroNome = user.PrimeiroNome,
                UltimoNome = user.UltimoNome,
                Email = user.Email
            };
        }
    }
}
