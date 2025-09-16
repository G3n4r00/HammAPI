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
    public class TransacoesController : ControllerBase
    {
        private readonly HammAPIDbContext _context;

        public TransacoesController(HammAPIDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransacaoDTO>>> GetAll(int pageNumber = 1, int pageSize = 20)
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var list = await _context.Transacoes
                .AsNoTracking()
                .OrderByDescending(t => t.Data)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TransacaoDTO
                {
                    Id = t.Id,
                    UsuarioId = t.UsuarioId,
                    Valor = t.Valor,
                    Data = t.Data,
                    Descricao = t.Descricao,
                    CategoriaId = t.CategoriaId,
                    Tipo = t.Tipo,
                    MetodoPagamento = t.MetodoPagamento
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TransacaoDTO>> Get(Guid id)
        {
            var t = await _context.Transacoes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (t == null) return NotFound();
            return Ok(new TransacaoDTO
            {
                Id = t.Id,
                UsuarioId = t.UsuarioId,
                Valor = t.Valor,
                Data = t.Data,
                Descricao = t.Descricao,
                CategoriaId = t.CategoriaId, 
                Tipo = t.Tipo,
                MetodoPagamento = t.MetodoPagamento
            });
        }

        [HttpPost]
        public async Task<ActionResult<TransacaoDTO>> Create(CreateTransacaoDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // checar usuário existe
            var userExists = await _context.Usuarios.AnyAsync(u => u.Id == dto.UsuarioId);
            if (!userExists) return BadRequest(new { message = "Usuário não encontrado." });

            if (dto.Valor == 0) return BadRequest(new { message = "Valor não pode ser zero." });

            var t = new Transacao
            {
                UsuarioId = dto.UsuarioId,
                Valor = dto.Valor,
                Data = dto.Data,
                Descricao = dto.Descricao,
                CategoriaId = dto.CategoriaId,
                Tipo = dto.Tipo,
                MetodoPagamento = dto.MetodoPagamento
            };

            _context.Transacoes.Add(t);
            await _context.SaveChangesAsync();

            var result = new TransacaoDTO
            {
                Id = t.Id,
                UsuarioId = t.UsuarioId,
                Valor = t.Valor,
                Data = t.Data,
                Descricao = t.Descricao,
                CategoriaId = t.CategoriaId,
                Tipo = t.Tipo,
                MetodoPagamento = t.MetodoPagamento
            };

            return CreatedAtAction(nameof(Get), new { id = t.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateTransacaoDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var t = await _context.Transacoes.FirstOrDefaultAsync(x => x.Id == id);
            if (t == null) return NotFound();

            // Não permitimos mudar o UsuarioId aqui (se quiser suportar, faça regra explícita)
            t.Valor = dto.Valor;
            t.Data = dto.Data;
            t.Descricao = dto.Descricao;
            t.CategoriaId = dto.CategoriaId;
            t.Tipo = dto.Tipo;
            t.MetodoPagamento = dto.MetodoPagamento;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var t = await _context.Transacoes.FindAsync(id);
            if (t == null) return NotFound();

            _context.Transacoes.Remove(t);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

}
