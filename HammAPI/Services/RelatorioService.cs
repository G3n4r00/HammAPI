using HammAPI.Data;
using HammAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using System;

namespace HammAPI.Services
{
    public class RelatorioService
    {
        private readonly HammAPIDbContext _context;

        public RelatorioService(HammAPIDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UsuarioExisteAsync(Guid usuarioId)
        {
            return await _context.Usuarios.AnyAsync(u => u.Id == usuarioId);
        }

        public async Task<RelatorioDTO> GerarRelatorioAsync(Guid usuarioId, int? mes, int? ano)
        {
            var query = _context.Transacoes
                .Include(t => t.Categoria)
                .Where(t => t.UsuarioId == usuarioId);

            if (mes.HasValue && ano.HasValue)
            {
                query = query.Where(t => t.Data.Month == mes.Value && t.Data.Year == ano.Value);
            }
            else if (ano.HasValue)
            {
                query = query.Where(t => t.Data.Year == ano.Value);
            }

            var transacoes = await query.ToListAsync();

            // Totais de receita e despesa
            var totalReceitas = transacoes.Where(t => t.Tipo == "Receita").Sum(t => t.Valor);
            var totalDespesas = transacoes.Where(t => t.Tipo == "Despesa").Sum(t => t.Valor);

            // Orcamento total do período
            var orcamentos = await _context.Orcamentos
                .Where(o => o.UsuarioId == usuarioId)
                .ToListAsync();
            var orcamentoTotal = orcamentos.Sum(o => o.ValorLimite);

            // Top 5 maiores despesas e receitas
            var top5Despesas = transacoes
                .Where(t => t.Tipo == "Despesa")
                .OrderByDescending(t => t.Valor)
                .Take(5)
                .Select(t => new TransacaoResumoDto
                {
                    Id = t.Id,
                    Descricao = t.Descricao,
                    Valor = t.Valor,
                    Data = t.Data,
                    Tipo = t.Tipo
                })
                .ToList();

            var top5Receitas = transacoes
                .Where(t => t.Tipo == "Receita")
                .OrderByDescending(t => t.Valor)
                .Take(5)
                .Select(t => new TransacaoResumoDto
                {
                    Id = t.Id,
                    Descricao = t.Descricao,
                    Valor = t.Valor,
                    Data = t.Data,
                    Tipo = t.Tipo
                })
                .ToList();

            // Gasto por categoria
            var gastoPorCategoria = transacoes
                .Where(t => t.Tipo == "Despesa" && t.Categoria != null)
                .GroupBy(t => new { t.CategoriaId, t.Categoria.Nome })
                .Select(g => new GastoCategoriaDto
                {
                    CategoriaId = g.Key.CategoriaId,
                    NomeCategoria = g.Key.Nome,
                    TotalGasto = g.Sum(x => x.Valor)
                })
                .ToList();

            // Metas ativas
            var hoje = DateTime.UtcNow;
            var metas = await _context.Metas
                .Where(m => m.UsuarioId == usuarioId && m.DataAlvo > hoje)
                .ToListAsync();

            var metasDto = metas.Select(m => new MetaStatusDto
            {
                Id = m.Id,
                Nome = m.Nome,
                ValorMeta = m.ValorObjetivo,
                TotalEconomizado = m.ValorAtual,
                DataFim = m.DataAlvo,
                Status = m.Status
            }).ToList();

            return new RelatorioDTO
            {
                TotalReceitas = totalReceitas,
                TotalDespesas = totalDespesas,
                OrcamentoTotal = orcamentoTotal,
                Top5Despesas = top5Despesas,
                Top5Receitas = top5Receitas,
                GastoPorCategoria = gastoPorCategoria,
                MetasAtivas = metasDto
            };
        }
    }
}
