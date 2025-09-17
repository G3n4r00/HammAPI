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
    public class OrcamentosController : ControllerBase
    {
        private readonly HammAPIDbContext _context;
        public OrcamentosController(HammAPIDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrcamentoDTO>>> GetAll()
        {
            var list = await _context.Orcamentos
                .AsNoTracking()
                .Select(o => new OrcamentoDTO
                {
                    Id = o.Id,
                    UsuarioId = o.UsuarioId,
                    Nome = o.Nome,
                    ValorLimite = o.ValorLimite,                   
                    Mes = o.Mes ?? 01, //AUSTAR AQUI
                    Ano = o.Ano ?? 2025,// AJUSTAR AQUI
                    ValorUtilizado = o.ValorUtilizado
                })
                .ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OrcamentoDTO>> Get(Guid id)
        {
            var o = await _context.Orcamentos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (o == null) return NotFound();
            return Ok(new OrcamentoDTO
            {
                Id = o.Id,
                UsuarioId = o.UsuarioId,
                Nome = o.Nome,
                ValorLimite = o.ValorLimite,
                Mes = o.Mes,
                Ano = o.Ano,
                ValorUtilizado = o.ValorUtilizado
            });
        }

        [HttpPost]
        public async Task<ActionResult<OrcamentoDTO>> Create(CreateOrcamentoDTO dto)
        {
            var userExists = await _context.Usuarios.AnyAsync(u => u.Id == dto.UsuarioId);
            if (!userExists) return BadRequest(new { message = "Usuário não encontrado." });

            var o = new Orcamento
            {
                UsuarioId = dto.UsuarioId,
                Nome = dto.Nome,
                ValorLimite = dto.ValorLimite,
                Mes = dto.Mes,
                Ano = dto.Ano,
                ValorUtilizado = dto.ValorUtilizado
            };

            _context.Orcamentos.Add(o);
            await _context.SaveChangesAsync();

            var result = new OrcamentoDTO
            {
                Id = o.Id,
                UsuarioId = o.UsuarioId,
                Nome = o.Nome,
                ValorLimite = o.ValorLimite,
                Mes = o.Mes,
                Ano = o.Ano,
                ValorUtilizado = o.ValorUtilizado
            };
            return CreatedAtAction(nameof(Get), new { id = o.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateOrcamentoDTO dto)
        {
            var o = await _context.Orcamentos.FirstOrDefaultAsync(x => x.Id == id);
            if (o == null) return NotFound();

            o.Nome = dto.Nome;
            o.ValorLimite = dto.ValorLimite;
            o.Mes = dto.Mes;
            o.Ano = dto.Ano;
            o.ValorUtilizado = dto.ValorUtilizado;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var o = await _context.Orcamentos.FindAsync(id);
            if (o == null) return NotFound();
            _context.Orcamentos.Remove(o);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

}
