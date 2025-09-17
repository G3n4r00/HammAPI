using HammAPI.Data;
using HammAPI.DTOs;
using HammAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HammAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        /// Retorna todos os usuários com paginação.
        /// </summary>
        /// <param name="pageNumber">Número da página (padrão = 1)</param>
        /// <param name="pageSize">Tamanho da página, máximo 100 (padrão = 20)</param>
        /// <returns>Lista paginada de usuários</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> GetAll(int pageNumber = 1, int pageSize = 20)
        {
            pageNumber = Math.Max(1, pageNumber);

            pageSize = Math.Clamp(pageSize, 1, 100);

            var usuarios = await _context.Usuarios
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UsuarioDTO { Id = u.Id, PrimeiroNome = u.PrimeiroNome, UltimoNome = u.UltimoNome, Email = u.Email })
                .ToListAsync();
            return Ok(usuarios);
        }


        /// <summary>
        /// Retorna um usuário pelo ID.
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>Objeto do usuário encontrado</returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UsuarioDTO>> Get(Guid id)
        {
            var u = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (u == null) return NotFound();

            return Ok(new UsuarioDTO { Id = u.Id, PrimeiroNome = u.PrimeiroNome, UltimoNome = u.UltimoNome, Email = u.Email });
        }


        /// <summary>
        /// Cria um novo usuário.
        /// </summary>
        /// <param name="dto">Dados para criação do usuário</param>
        /// <returns>Usuário criado</returns>
        [HttpPost]
        public async Task<ActionResult<UsuarioDTO>> Create([FromBody] CreateUsuarioDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //checar email existente
            if (await _context.Usuarios.AnyAsync(x => x.Email == dto.Email))
            {
                return Conflict(new {message = "Email ja foi cadastrado!"});
            }

            var user = new Usuario
            {
                PrimeiroNome = dto.PrimeiroNome,
                UltimoNome = dto.UltimoNome,
                Email = dto.Email,
            };

            user.SenhaHash = _passwordHasher.HashPassword(user, dto.Senha);

            _context.Usuarios.Add(user);
            await _context.SaveChangesAsync();

            var result = new UsuarioDTO { Id = user.Id, PrimeiroNome=user.PrimeiroNome, UltimoNome=dto.UltimoNome, Email=dto.Email};
            return CreatedAtAction(nameof(Get), new { id = user.Id }, result);

        }

        /// <summary>
        /// Atualiza os dados de um usuário existente.
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <param name="dto">Dados atualizados</param>
        /// <returns>Sem conteúdo caso sucesso</returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUsuarioDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Usuarios.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            // caso email va mudar, verificar unicidade
            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
            {
                if (await _context.Usuarios.AnyAsync(x => x.Email == dto.Email && x.Id != id))
                {
                    return Conflict(new { message = "O Email solicitado já foi cadastrado por outro usuario." });
                }
                user.Email = dto.Email;
            }

            user.PrimeiroNome = dto.PrimeiroNome;

            if (!string.IsNullOrWhiteSpace(dto.Senha))
            {
                user.SenhaHash = _passwordHasher.HashPassword(user, dto.Senha);
            }

            await _context.SaveChangesAsync();
            return NoContent();
            
        }


        /// <summary>
        /// Remove um usuário pelo ID (hard delete).
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>Sem conteúdo caso sucesso</returns>
        [HttpDelete("{id:guid}")]
        public async Task <ActionResult> Delete(Guid id)
        {
            var user = await _context.Usuarios.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
