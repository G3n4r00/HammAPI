using HammAPI.Data;
using HammAPI.DTOs;
using HammAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace HammAPI.Controllers
{
    /// <summary>
    /// Controlador responsável pelo gerenciamento dos Orçamentos
    /// Permite realizar operações de CRUD
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OrcamentosController : ControllerBase
    {
        private readonly HammAPIDbContext _context;
        public OrcamentosController(HammAPIDbContext context) => _context = context;

        /// <summary>
        /// Retorna todos os orçamentos cadastrados.
        /// </summary>
        /// <returns>Lista de orçamentos</returns>
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
                    Mes = o.Mes ?? DateTime.Now.ToString("MMMM"), 
                    Ano = o.Ano ?? DateTime.Now.ToString("YYYY"),
                    ValorUtilizado = o.ValorUtilizado
                })
                .ToListAsync();
            return Ok(list);
        }

        /// <summary>
        /// Retorna um orçamento específico pelo seu ID.
        /// </summary>
        /// <param name="id">ID do orçamento</param>
        /// <returns>Objeto de orçamento encontrado</returns>
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
                Mes = o.Mes ?? DateTime.Now.ToString("MMMM"),
                Ano = o.Ano ?? DateTime.Now.ToString("YYYY"),
                ValorUtilizado = o.ValorUtilizado
            });
        }

        /// <summary>
        /// Cria um novo orçamento.
        /// </summary>
        /// <param name="dto">Dados para criação do orçamento</param>
        /// <returns>Objeto de orcamento criado</returns>
        [HttpPost]
        public async Task<ActionResult<OrcamentoDTO>> Create(CreateOrcamentoDTO dto)
        {
            var userExists = await _context.Usuarios.AnyAsync(u => u.Id == dto.UsuarioId);
            if (!userExists) return BadRequest(new { message = "Usuário não encontrado." });
            if (dto.ValorLimite <= 0) return BadRequest(new { message = "Valor deve ser maior que zero." });

            var o = new Orcamento
            {
                UsuarioId = dto.UsuarioId,
                Nome = dto.Nome,
                ValorLimite = dto.ValorLimite,
                Mes = dto.Mes ?? DateTime.Now.ToString("MMMM"),
                Ano = dto.Ano ?? DateTime.Now.ToString("YYYY"),
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
                Mes = o.Mes ?? DateTime.Now.ToString("MMMM"),
                Ano = o.Ano ?? DateTime.Now.ToString("YYYY"),
                ValorUtilizado = o.ValorUtilizado
            };
            return CreatedAtAction(nameof(Get), new { id = o.Id }, result);
        }

        /// <summary>
        /// Atualiza os dados de um orçamento existente.
        /// </summary>
        /// <param name="id">ID do orçamento</param>
        /// <param name="dto">Dados para atualização</param>
        /// <returns>Sem conteudo caso sucesso</returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateOrcamentoDTO dto)
        {
            var o = await _context.Orcamentos.FirstOrDefaultAsync(x => x.Id == id);
            if (o == null) return NotFound();
            if (dto.ValorLimite <= 0) return BadRequest(new { message = "Valor deve ser maior que zero." });

            o.Nome = dto.Nome;
            o.ValorLimite = dto.ValorLimite;
            o.Mes = dto.Mes;
            o.Ano = dto.Ano;
            o.ValorUtilizado = dto.ValorUtilizado;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Remove um orçamento pelo ID.
        /// </summary>
        /// <param name="id">ID do orçamento</param>
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
