namespace HammAPI.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("transacoes", Schema = "public")]
    public class Transacao
    {
        [Key]
        [Column("id_transacao")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("id_usuario")]
        public Guid UsuarioId { get; set; }

        [Required]
        [Column("id_categoria")]
        public Guid CategoriaId { get; set; }

        [Required]
        [Column("valor", TypeName = "money")]
        public decimal Valor { get; set; }

        [Required]
        [Column("data")]
        public DateTime Data { get; set; }

        [Column("descricao")]
        public string Descricao { get; set; }

        [Column("tipo")]
        public string Tipo { get; set; } // Receita ou Despesa

        [Column("metodo_pagamento")]
        public string MetodoPagamento { get; set; }

        // Navegação
        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

        [ForeignKey("CategoriaId")]
        public Categoria Categoria { get; set; }
    }
}
