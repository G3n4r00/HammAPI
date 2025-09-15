namespace HammAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("orcamento", Schema = "public")]
    public class Orcamento
    {
        [Key]
        [Column("id_orcamento")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("id_usuario")]
        public Guid UsuarioId { get; set; }

        [Column("valor_limite", TypeName = "money")]
        public decimal? ValorLimite { get; set; }

        [Column("mes")]
        public int? Mes { get; set; }

        [Column("ano")]
        public int? Ano { get; set; }

        [Column("valor_utilizado", TypeName = "money")]
        public decimal? ValorUtilizado { get; set; }

        // Navegação
        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }
    }
}
