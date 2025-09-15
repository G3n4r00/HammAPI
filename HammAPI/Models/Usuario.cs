namespace HammAPI.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("usuarios", Schema = "public")]
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("primeiro_nome")]
        public string PrimeiroNome { get; set; }

        [Required]
        [Column("ultimo_nome")]
        public string UltimoNome { get; set; }

        [Required]
        [Column("email")]
        public string Email { get; set; }

        [Required]
        [Column("senha_hash")]
        public string SenhaHash { get; set; }

        // Navegação
        public ICollection<Transacao> Transacoes { get; set; }
        public ICollection<Orcamento> Orcamentos { get; set; }
        public ICollection<Meta> Metas { get; set; }
    }
}
