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

        /// <summary>
        /// Retorna todas as transações com paginação.
        /// </summary>
        /// <param name="pageNumber">Número da página (padrão = 1)</param>
        /// <param name="pageSize">Tamanho da página, máximo 100 (padrão = 20)</param>
        /// <returns>Lista paginada de transações</returns>
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

        /// <summary>
        /// Retorna uma transação pelo seu ID.
        /// </summary>
        /// <param name="id">ID da transação</param>
        /// <returns>Objeto da transação encontrada</returns>
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

        /// <summary>
        /// Cria uma nova transação.
        /// </summary>
        /// <param name="dto">Dados da transação a ser criada</param>
        /// <returns>Objeto da transação criada</returns>
        [HttpPost]
        public async Task<ActionResult<TransacaoDTO>> Create(CreateTransacaoDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // checar usuário e categoria existem
            var userExists = await _context.Usuarios.AnyAsync(u => u.Id == dto.UsuarioId);
            var categoriaExists = await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);

            if (!userExists) return BadRequest(new { message = "Usuário não encontrado." });
            if (!categoriaExists) return BadRequest(new { message = "Categoria não encontrada." });

            if (dto.Valor <= 0) return BadRequest(new { message = "Valor deve ser maior que zero." });
            if (dto.Data > DateTime.UtcNow.Date) return BadRequest(new { message = "Data de transação não pode ser no futuro" });
            dto.MetodoPagamento = dto.MetodoPagamento.ToUpper();
            if (dto.MetodoPagamento != "CREDITO" || dto.MetodoPagamento != "DEBITO") return BadRequest(new { message = "O metodo de pagamento deve ser 'CREDITO' ou 'DEBITO'" });

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

        /// <summary>
        /// Atualiza uma transação existente.
        /// </summary>
        /// <param name="id">ID da transação</param>
        /// <param name="dto">Dados da transação a serem atualizados</param>
        /// <returns>Sem conteúdo caso sucesso</returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateTransacaoDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var categoriaExists = await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);

            if (!categoriaExists) return BadRequest(new { message = "Categoria não encontrada." });

            if (dto.Valor <= 0) return BadRequest(new { message = "Valor deve ser maior que zero." });
            if (dto.Data > DateTime.UtcNow.Date) return BadRequest(new { message = "Data de transação não pode ser no futuro" });

            dto.MetodoPagamento = dto.MetodoPagamento.ToUpper();
            if (dto.MetodoPagamento != "CREDITO" || dto.MetodoPagamento != "DEBITO") return BadRequest(new { message = "O metodo de pagamento deve ser 'CREDITO' ou 'DEBITO'" });

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

        /// <summary>
        /// Remove uma transação pelo ID.
        /// </summary>
        /// <param name="id">ID da transação</param>
        /// <returns>Sem conteúdo caso sucesso</returns>
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
