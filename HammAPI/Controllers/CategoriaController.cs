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

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var c = await _context.Categorias.FindAsync(id);
            if (c == null) return NotFound();
            _context.Categorias.Remove(c);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

}
