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
    public class CategoriasController : ControllerBase
    {
        private readonly HammAPIDbContext _context;
        public CategoriasController(HammAPIDbContext context) => _context = context;

        /// <summary>
        /// Retorna todas as categorias cadastradas.
        /// </summary>
        /// <returns>Lista de categorias</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetAll()
        {
            var list = await _context.Categorias
                .AsNoTracking()
                .Select(c => new CategoriaDTO { 
                    Id = c.Id, 
                    Nome = c.Nome, 
                    Descricao = c.Descricao,
                    Tipo = c.Tipo,
                    EPadrao = c.EPadrao
                })
                .ToListAsync();
            return Ok(list);
        }

        /// <summary>
        /// Retorna uma categoria específica pelo seu identificador único.
        /// </summary>
        /// <param name="id">Identificador único da categoria.</param>
        /// <returns>Objeto da categoria encontrada</returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CategoriaDTO>> Get(Guid id)
        {
            var c = await _context.Categorias.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) return NotFound();
            return Ok(new CategoriaDTO {
                Id = c.Id,
                Nome = c.Nome,
                Descricao = c.Descricao,
                Tipo = c.Tipo,
                EPadrao = c.EPadrao
            });
        }

        /// <summary>
        /// Cria uma nova categoria.
        /// </summary>
        /// <param name="dto">Dados necessários para criação da categoria.</param>
        /// <returns>Objeto da categoria criada</returns>
        [HttpPost]
        public async Task<ActionResult<CategoriaDTO>> Create(CreateCategoriaDTO dto)
        {
            var c = new Categoria { 
                Nome = dto.Nome,
                EPadrao = false,
                Tipo = dto.Tipo,
                Descricao = dto.Descricao 
            };

            _context.Categorias.Add(c);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = c.Id }, new CategoriaDTO { Id = c.Id, Nome = c.Nome, Descricao = c.Descricao, Tipo = c.Tipo });
        }

        /// <summary>
        /// Atualiza os dados de uma categoria existente.
        /// </summary>
        /// <param name="id">Identificador único da categoria.</param>
        /// <param name="dto">Dados atualizados da categoria.</param>
        /// <returns>Resposta sem conteúdo em caso de sucesso.</returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateCategoriaDTO dto)
        {
            var c = await _context.Categorias.FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) return NotFound();
            c.Nome = dto.Nome;
            c.Tipo = dto.Tipo;
            c.Descricao = dto.Descricao;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Remove uma categoria existente.
        /// </summary>
        /// <param name="id">Identificador único da categoria.</param>
        /// <returns>Resposta sem conteúdo em caso de sucesso.</returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var categoriaPadrao = await _context.Categorias.AnyAsync(c => c.EPadrao == false);
            if (!categoriaPadrao) return BadRequest(new { message = "Categorias padrão não podem ser deletadas." });

            var c = await _context.Categorias.FindAsync(id);
            if (c == null) return NotFound();
            _context.Categorias.Remove(c);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

}
