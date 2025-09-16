using System.ComponentModel.DataAnnotations;

namespace HammAPI.DTOs
{
    public record UsuarioDTO
    {
        public Guid Id { get; set; }

        public string PrimeiroNome { get; set; }

        public string UltimoNome { get; set; }

        public string Email { get; set; }
    }

    public record UpdateUsuarioDTO
    {
        [Required]
        public string PrimeiroNome { get; set; }

        [Required]
        public string UltimoNome { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [MinLength(8)]
        public string Senha { get; set; }
    }

    public record CreateUsuarioDTO
    {
        [Required]
        public string PrimeiroNome { get; set; }

        [Required]
        public string UltimoNome { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(8)]
        public string Senha { get; set; }

    }
}
