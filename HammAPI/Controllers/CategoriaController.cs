using HammAPI.Data;
using HammAPI.DTOs;
using HammAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HammAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriasController : ControllerBase
    {
        private readonly HammAPIDbContext _context;
        public CategoriasController(HammAPIDbContext context) => _context = context;

        /// <summary>
        /// Retorna todas as categorias disponíveis (padrão + personalizadas do usuário).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetAll()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = Guid.Parse(userIdStr);

            var list = await _context.Categorias
                .AsNoTracking()
                .Where(c => c.EPadrao || c.UsuarioId == userId)
                .Select(c => new CategoriaDTO
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Descricao = c.Descricao,
                    Tipo = c.Tipo,
                    EPadrao = c.EPadrao,
                    UsuarioId = c.UsuarioId
                })
                .ToListAsync();

            return Ok(list);
        }

        /// <summary>
        /// Retorna uma categoria específica, se o usuário tiver acesso a ela.
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CategoriaDTO>> Get(Guid id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = Guid.Parse(userIdStr);

            var c = await _context.Categorias
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && (x.EPadrao || x.UsuarioId == userId));

            if (c == null)
                return NotFound("Categoria não encontrada ou acesso não permitido.");

            return Ok(new CategoriaDTO
            {
                Id = c.Id,
                Nome = c.Nome,
                Descricao = c.Descricao,
                Tipo = c.Tipo,
                EPadrao = c.EPadrao,
                UsuarioId = c.UsuarioId
            });
        }

        /// <summary>
        /// Cria uma nova categoria personalizada para o usuário autenticado.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CategoriaDTO>> Create(CreateCategoriaDTO dto)
        {
            var usuarioIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usuarioId = Guid.Parse(usuarioIdStr);

            // Evita duplicidade de categorias do mesmo nome/tipo para o mesmo usuário
            var exists = await _context.Categorias.AnyAsync(c =>
                c.UsuarioId == usuarioId &&
                c.Nome.ToLower() == dto.Nome.ToLower() &&
                c.Tipo == dto.Tipo);

            if (exists)
                return Conflict("Você já possui uma categoria com esse nome e tipo.");

            var c = new Categoria
            {
                Nome = dto.Nome,
                Tipo = dto.Tipo,
                Descricao = dto.Descricao,
                EPadrao = false,
                UsuarioId = usuarioId
            };

            _context.Categorias.Add(c);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = c.Id }, new CategoriaDTO
            {
                Id = c.Id,
                Nome = c.Nome,
                Tipo = c.Tipo,
                Descricao = c.Descricao,
                EPadrao = c.EPadrao,
                UsuarioId = c.UsuarioId
            });
        }

        /// <summary>
        /// Atualiza uma categoria personalizada do usuário autenticado.
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateCategoriaDTO dto)
        {
            var usuarioIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usuarioId = Guid.Parse(usuarioIdStr);

            var c = await _context.Categorias.FirstOrDefaultAsync(x => x.Id == id);

            if (c == null)
                return NotFound("Categoria não encontrada.");

            if (c.EPadrao)
                return Forbid("Categorias padrão não podem ser alteradas.");

            if (c.UsuarioId != usuarioId)
                return Forbid("Você só pode alterar suas próprias categorias.");

            c.Nome = dto.Nome;
            c.Tipo = dto.Tipo;
            c.Descricao = dto.Descricao;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Remove uma categoria personalizada do usuário autenticado.
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var usuarioIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usuarioId = Guid.Parse(usuarioIdStr);

            var c = await _context.Categorias.FirstOrDefaultAsync(x => x.Id == id);

            if (c == null)
                return NotFound("Categoria não encontrada.");

            if (c.EPadrao)
                return Forbid("Categorias padrão não podem ser excluídas.");

            if (c.UsuarioId != usuarioId)
                return Forbid("Você só pode excluir suas próprias categorias.");

            _context.Categorias.Remove(c);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
