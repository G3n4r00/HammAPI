namespace HammAPI.DTOs
{
    public record RelatorioDTO
    {
        public decimal TotalReceitas { get; set; }
        public decimal TotalDespesas { get; set; }
        public decimal OrcamentoTotal { get; set; }
        public List<TransacaoResumoDto> Top5Receitas { get; set; } = new();
        public List<TransacaoResumoDto> Top5Despesas { get; set; } = new();
        public List<GastoCategoriaDto> GastoPorCategoria { get; set; } = new();
        public List<MetaStatusDto> MetasAtivas { get; set; } = new();
    }

    public class TransacaoResumoDto
    {
        public Guid Id { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
        public string Tipo { get; set; } // Receita ou Despesa
    }

    public class GastoCategoriaDto
    {
        public Guid CategoriaId { get; set; }
        public string NomeCategoria { get; set; }
        public decimal TotalGasto { get; set; }
    }

    public class MetaStatusDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public decimal ValorMeta { get; set; }
        public decimal? TotalEconomizado { get; set; }
        public DateTime DataFim { get; set; }
        public string Status { get; set; } // Em andamento, Concluída, Expirada
    }
}
