using HammAPI.Data;
using HammAPI.DTOs;
using HammAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;

namespace HammAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MetasController : ControllerBase
    {
        private readonly HammAPIDbContext _context;
        public MetasController(HammAPIDbContext context) => _context = context;

        /// <summary>
        /// Retorna todas as metas cadastradas.
        /// </summary>
        /// <returns>Lista de metas</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MetaDTO>>> GetAll()
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (usuarioId == null) return Unauthorized();

            var list = await _context.Metas
                .AsNoTracking()
                .Where(m => m.UsuarioId == Guid.Parse(usuarioId)) // Apenas metas do usuário logado
                .Select(m => new MetaDTO
                {
                    Id = m.Id,
                    UsuarioId = m.UsuarioId,
                    Nome = m.Nome,
                    ValorObjetivo = m.ValorObjetivo,
                    ValorAtual = m.ValorAtual,
                    DataInicio = m.DataInicio,
                    DataAlvo = m.DataAlvo,
                    Descricao = m.Descricao,
                    Status = m.Status
                })
                .ToListAsync();

            return Ok(list);
        }

        /// <summary>
        /// Retorna uma meta específica pelo seu identificador único.
        /// </summary>
        /// <param name="id">Identificador único da meta.</param>
        /// <returns>Objeto da meta encontrada</returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MetaDTO>> Get(Guid id)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (usuarioId == null) return Unauthorized();

            var m = await _context.Metas
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.UsuarioId == Guid.Parse(usuarioId));

            if (m == null) return NotFound();

            return Ok(new MetaDTO
            {
                Id = m.Id,
                UsuarioId = m.UsuarioId,
                Nome = m.Nome,
                ValorObjetivo = m.ValorObjetivo,
                ValorAtual = m.ValorAtual,
                DataInicio = m.DataInicio,
                DataAlvo = m.DataAlvo,
                Descricao = m.Descricao,
                Status = m.Status
            });
        }

        /// <summary>
        /// Cria uma nova meta vinculada a um usuário.
        /// </summary>
        /// <param name="dto">Dados necessários para criação da meta.</param>
        /// <returns>Objeto da meta criada</returns>

        [HttpPost]
        public async Task<ActionResult<MetaDTO>> Create(CreateMetaDTO dto)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (usuarioId == null) return Unauthorized();

            var m = new Meta
            {
                Nome = dto.Nome,
                UsuarioId = Guid.Parse(usuarioId), // 🔒 Pega do JWT
                ValorObjetivo = dto.ValorObjetivo,
                ValorAtual = dto.ValorAtual,
                DataInicio = dto.DataInicio ?? DateTime.UtcNow,
                DataAlvo = dto.DataAlvo,
                Descricao = dto.Descricao,
                Status = dto.Status ?? "EmProgresso"
            };

            _context.Metas.Add(m);
            await _context.SaveChangesAsync();

            var result = new MetaDTO
            {
                Id = m.Id,
                Nome = m.Nome,
                UsuarioId = m.UsuarioId,
                ValorObjetivo = m.ValorObjetivo,
                ValorAtual = m.ValorAtual,
                DataInicio = m.DataInicio,
                DataAlvo = m.DataAlvo,
                Descricao = m.Descricao,
                Status = m.Status
            };

            return CreatedAtAction(nameof(Get), new { id = m.Id }, result);
        }

        /// <summary>
        /// Atualiza os dados de uma meta existente.
        /// </summary>
        /// <param name="id">Identificador único da meta.</param>
        /// <param name="dto">Dados atualizados da meta.</param>
        /// <returns>Resposta sem conteúdo em caso de sucesso.</returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateMetaDTO dto)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (usuarioId == null) return Unauthorized();

            var m = await _context.Metas.FirstOrDefaultAsync(x => x.Id == id && x.UsuarioId == Guid.Parse(usuarioId));
            if (m == null) return NotFound();

            m.Nome = dto.Nome;
            m.ValorObjetivo = dto.ValorObjetivo;
            m.ValorAtual = dto.ValorAtual;
            m.DataAlvo = dto.DataAlvo;
            m.Descricao = dto.Descricao;
            m.Status = dto.Status;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Remove uma meta existente.
        /// </summary>
        /// <param name="id">Identificador único da meta.</param>
        /// <returns>Resposta sem conteúdo em caso de sucesso.</returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (usuarioId == null) return Unauthorized();

            var m = await _context.Metas.FirstOrDefaultAsync(x => x.Id == id && x.UsuarioId == Guid.Parse(usuarioId));
            if (m == null) return NotFound();

            _context.Metas.Remove(m);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

}
