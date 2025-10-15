using HammAPI.Data;
using HammAPI.DTOs;
using HammAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HammAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly HammAPIDbContext _context;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public UsuariosController(HammAPIDbContext context, IPasswordHasher<Usuario> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Retorna o perfil do usuário autenticado.
        /// </summary>
        [HttpGet("me")]
        public async Task<ActionResult<UsuarioDTO>> GetProfile()
        {

            var authHeader = Request.Headers["Authorization"].ToString();
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _context.Usuarios.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.ToString() == userId);

            if (user == null) return NotFound();

            return Ok(new UsuarioDTO
            {
                Id = user.Id,
                PrimeiroNome = user.PrimeiroNome,
                UltimoNome = user.UltimoNome,
                Email = user.Email
            });
        }

        /// <summary>
        /// Atualiza o perfil do usuário autenticado.
        /// </summary>
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUsuarioDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _context.Usuarios.FirstOrDefaultAsync(x => x.Id.ToString() == userId);
            if (user == null) return NotFound();

            // Atualiza apenas campos enviados
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
    }
}
