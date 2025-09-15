using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HammAPI.DTOs
{
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
