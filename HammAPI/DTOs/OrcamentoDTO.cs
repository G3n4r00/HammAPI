using System.ComponentModel.DataAnnotations;

namespace HammAPI.DTOs
{
    public record OrcamentoDTO
    {
        [Required]
        public Guid Id { get; init; }

        [Required]
        public Guid UsuarioId { get; init; }

        [Required]
        public decimal ValorLimite { get; init; }

        [Required]
        public string Nome { get; init; }
        [Required]

        public string Mes { get; init; }
        [Required]

        public string Ano { get; init; }

        public decimal? ValorUtilizado { get; init; }

    }

    public record CreateOrcamentoDTO
    {
        [Required]
        public Guid UsuarioId { get; init; }
        [Required]
        public decimal ValorLimite { get; init; }

        [Required]
        public string Mes { get; init; }

        [Required]
        public string Nome { get; init; }

        [Required]
        public string Ano { get; init; }
        public decimal? ValorUtilizado { get; init; }
    }

    public record UpdateOrcamentoDTO
    {
        [Required]
        public decimal ValorLimite { get; init; }

        [Required]
        public string Mes { get; init; }

        [Required]
        public string Ano { get; init; }

        [Required]
        public string Nome { get; init; }
        public decimal? ValorUtilizado { get; init; }
    }   

}
