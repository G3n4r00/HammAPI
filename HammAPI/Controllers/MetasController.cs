using HammAPI.Data;
using HammAPI.DTOs;
using HammAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace HammAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetasController : ControllerBase
    {
        private readonly HammAPIDbContext _context;
        public MetasController(HammAPIDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MetaDTO>>> GetAll()
        {
            var list = await _context.Metas
                .AsNoTracking()
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

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MetaDTO>> Get(Guid id)
        {
            var m = await _context.Metas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
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

        [HttpPost]
        public async Task<ActionResult<MetaDTO>> Create(CreateMetaDTO dto)
        {
            var userExists = await _context.Usuarios.AnyAsync(u => u.Id == dto.UsuarioId);
            if (!userExists) return BadRequest(new { message = "Usuário não encontrado." });

            var m = new Meta
            {
                Nome = dto.Nome,
                UsuarioId = dto.UsuarioId,
                ValorObjetivo = dto.ValorObjetivo,
                ValorAtual = dto.ValorAtual,
                DataInicio = dto.DataInicio,
                DataAlvo = dto.DataAlvo,
                Descricao = dto.Descricao,
                Status = dto.Status

            };
            _context.Metas.Add(m);
            await _context.SaveChangesAsync();

            var result = new MetaDTO
            {
                Nome = dto.Nome,
                UsuarioId = dto.UsuarioId,
                ValorObjetivo = dto.ValorObjetivo,
                ValorAtual = dto.ValorAtual,
                DataInicio = dto.DataInicio,
                DataAlvo = dto.DataAlvo,
                Descricao = dto.Descricao,
                Status = dto.Status
            };
            return CreatedAtAction(nameof(Get), new { id = m.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateMetaDTO dto)
        {
            var m = await _context.Metas.FirstOrDefaultAsync(x => x.Id == id);
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

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var m = await _context.Metas.FindAsync(id);
            if (m == null) return NotFound();
            _context.Metas.Remove(m);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

}
