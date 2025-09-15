namespace HammAPI.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("metas", Schema = "public")]
    public class Meta
    {
        [Key]
        [Column("id_meta")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("id_usuario")]
        public Guid UsuarioId { get; set; }

        [Column("nome")]
        public string Nome { get; set; }

        [Column("valor_objetivo", TypeName = "money")]
        public decimal? ValorObjetivo { get; set; }

        [Column("valor_atual", TypeName = "money")]
        public decimal? ValorAtual { get; set; }

        [Column("dia_prazo")]
        public DateTime? DiaPrazo { get; set; }

        [Column("status")]
        public string Status { get; set; } // EmProgresso, Concluida, Cancelada

        // Navegação
        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }
    }
}
