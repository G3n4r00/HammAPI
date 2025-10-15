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
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransacoesController : ControllerBase
    {
        private readonly HammAPIDbContext _context;

        public TransacoesController(HammAPIDbContext context)
        {
            _context = context;
        }

        private Guid GetUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(idClaim);
        }

        /// <summary>
        /// Retorna todas as transações do usuário autenticado, com paginação.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransacaoDTO>>> GetAll(int pageNumber = 1, int pageSize = 20)
        {
            var userId = GetUserId();

            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var list = await _context.Transacoes
                .AsNoTracking()
                .Where(t => t.UsuarioId == userId) // filtro por usuário
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

        /// <summary>
        /// Retorna uma transação específica do usuário autenticado.
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TransacaoDTO>> Get(Guid id)
        {
            var userId = GetUserId();

            var t = await _context.Transacoes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.UsuarioId == userId); // 🔒 restrição

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

        /// <summary>
        /// Cria uma nova transação para o usuário autenticado.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TransacaoDTO>> Create(CreateTransacaoDTO dto)
        {
            var userId = GetUserId();

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var categoriaExists = await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);
            if (!categoriaExists)
                return BadRequest(new { message = "Categoria não encontrada." });

            if (dto.Valor <= 0)
                return BadRequest(new { message = "Valor deve ser maior que zero." });

            if (dto.Data > DateTime.UtcNow.Date)
                return BadRequest(new { message = "Data de transação não pode ser no futuro." });

            dto.MetodoPagamento = dto.MetodoPagamento.ToUpper();
            if (dto.MetodoPagamento != "CREDITO" && dto.MetodoPagamento != "DEBITO")
                return BadRequest(new { message = "O método de pagamento deve ser 'CREDITO' ou 'DEBITO'." });

            var t = new Transacao
            {
                UsuarioId = userId, // sempre pega do token
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

        /// <summary>
        /// Atualiza uma transação do usuário autenticado.
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateTransacaoDTO dto)
        {
            var userId = GetUserId();

            var t = await _context.Transacoes.FirstOrDefaultAsync(x => x.Id == id && x.UsuarioId == userId);
            if (t == null) return NotFound();

            if (dto.Valor <= 0)
                return BadRequest(new { message = "Valor deve ser maior que zero." });

            if (dto.Data > DateTime.UtcNow.Date)
                return BadRequest(new { message = "Data de transação não pode ser no futuro." });

            dto.MetodoPagamento = dto.MetodoPagamento.ToUpper();
            if (dto.MetodoPagamento != "CREDITO" && dto.MetodoPagamento != "DEBITO")
                return BadRequest(new { message = "O método de pagamento deve ser 'CREDITO' ou 'DEBITO'." });

            if (!await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId))
                return BadRequest(new { message = "Categoria não encontrada." });

            t.Valor = dto.Valor;
            t.Data = dto.Data;
            t.Descricao = dto.Descricao;
            t.CategoriaId = dto.CategoriaId;
            t.Tipo = dto.Tipo;
            t.MetodoPagamento = dto.MetodoPagamento;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Remove uma transação do usuário autenticado.
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetUserId();

            var t = await _context.Transacoes.FirstOrDefaultAsync(x => x.Id == id && x.UsuarioId == userId);
            if (t == null) return NotFound();

            _context.Transacoes.Remove(t);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}


