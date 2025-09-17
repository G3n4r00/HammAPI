using System.ComponentModel.DataAnnotations;

namespace HammAPI.DTOs
{
    public record MetaDTO
    {
        [Required]
        public Guid Id { get; init; }
        [Required]
        public Guid UsuarioId { get; init; }
        [Required]
        public string Nome { get; init; }
        [Required]
        public decimal ValorObjetivo { get; init; }
        public decimal? ValorAtual { get; init; }
        [Required]
        public DateTime DataAlvo { get; init; }
        public DateTime? DataInicio { get; init; }
        public string? Descricao { get; init; }
        public string? Status { get; init; }
    }

    public record CreateMetaDTO
    {
        [Required]
        public Guid UsuarioId { get; init; }

        [Required]
        public string Nome { get; init; }

        [Required]
        public decimal ValorObjetivo { get; init; }
        public decimal ValorAtual { get; init; }
        [Required]
        public DateTime DataAlvo { get; init; }
        public DateTime? DataInicio { get; init; }
        public string? Descricao { get; init; }
        public string? Status { get; init; }
    }

    public record UpdateMetaDTO
    {
        [Required]
        public string Nome { get; init; }
        [Required]
        public decimal ValorObjetivo { get; init; }
        public decimal ValorAtual { get; init; }
        [Required]
        public DateTime DataAlvo { get; init; }
        public string? Descricao { get; init; }
        public string? Status { get; init; }
    }
}
