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

        [Required]
        [Column("nome")]
        public string Nome { get; set; }


        [Required]
        [Column("valor_objetivo", TypeName = "money")]
        public decimal ValorObjetivo { get; set; }

        [Column("valor_atual", TypeName = "money")]
        public decimal? ValorAtual { get; set; }

        [Column("data_inicio")]
        public DateTime? DataInicio { get; set; }

        [Required]
        [Column("dia_prazo")]
        public DateTime DataAlvo { get; set; }

        [Column("descricao")]
        public string? Descricao { get; set; }

        [Column("status")]
        public string? Status { get; set; } // EmProgresso, Concluida, Cancelada

        // Navegação
        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }
    }
}
