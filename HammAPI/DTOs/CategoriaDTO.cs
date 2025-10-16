using System.ComponentModel.DataAnnotations;

namespace HammAPI.DTOs
{
    public record CategoriaDTO
    {
        [Required]
        public Guid Id { get; init; }

        [Required]
        public string Nome { get; init; }

        [Required]
        public string Tipo { get; init; } // Receita ou Despesa

        [Required]
        public bool EPadrao { get; init; }

        public string? Descricao { get; init; }

        public Guid? UsuarioId { get; init; } // visível apenas se for do próprio usuário
    }

    public record CreateCategoriaDTO
    {
        [Required]
        public string Nome { get; init; }
        [Required]
        public string Tipo { get; init; } // Receita ou Despesa
        public string? Descricao { get; init; }
    }

    public record UpdateCategoriaDTO
    {
        [Required]
        public string Nome { get; init; }
        [Required]
        public string Tipo { get; init; } // Receita ou Despesa

        public string? Descricao { get; init; }
    }
}
