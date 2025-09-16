using System.ComponentModel.DataAnnotations;

namespace HammAPI.DTOs
{
    public record TransacaoDTO
    {
        public Guid Id { get; init; }
        public Guid UsuarioId { get; init; }
        public decimal Valor { get; init; }
        public DateTime Data { get; init; }
        public string? Descricao { get; init; }
        public Guid CategoriaId { get; init; }
        public string? Tipo { get; init; } // Receita ou Despesa
        public string? MetodoPagamento { get; init; }    
    }

    public record UpdateTransacaoDTO
    {
        [Required]
        public decimal Valor { get; init; }

        [Required]
        public DateTime Data { get; init; }

        [StringLength(250)]
        public string? Descricao { get; init; }

        [Required]
        public Guid CategoriaId { get; init; }

        [Required]
        public string Tipo { get; set; } // Receita ou Despesa

        public string MetodoPagamento { get; set; }
    }

    public record CreateTransacaoDTO
    {
        [Required]
        public Guid UsuarioId { get; init; }

        [Required]
        public decimal Valor { get; init; }

        [Required]
        public DateTime Data { get; init; }

        [StringLength(250)]
        public string? Descricao { get; init; }

        [Required]
        public Guid CategoriaId { get; init; }

        [Required]
        public string Tipo { get; set; } // Receita ou Despesa

        public string MetodoPagamento { get; set; }
    }
}
