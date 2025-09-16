namespace HammAPI.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("categorias", Schema = "public")]
    public class Categoria
    {
        [Key]
        [Column("id_categoria")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("nome")]
        public string Nome { get; set; }

        [Required]
        [Column("tipo")]
        public string Tipo { get; set; } // Receita ou Despesa

        [Required]
        [Column("e_padrao")]
        public bool EPadrao { get; set; }

        [Column("descricao")]
        public string? Descricao { get; set; }

        // Navegação
        public ICollection<Transacao> Transacoes { get; set; }
    }
}
