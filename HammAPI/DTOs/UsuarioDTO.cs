namespace HammAPI.DTOs
{
    public class UsuarioDTO
    {
        public Guid Id { get; set; }

        public string PrimeiroNome { get; set; }

        public string UltimoNome { get; set; }

        public string Email { get; set; }
    }
}
