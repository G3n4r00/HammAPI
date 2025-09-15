using System.ComponentModel.DataAnnotations;

namespace HammAPI.DTOs
{
    public class UpdateUsuarioDTO
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
}
