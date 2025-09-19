using HammAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HammAPI.Data
{
    public class HammAPIDbContext : DbContext
    {
        public HammAPIDbContext(DbContextOptions<HammAPIDbContext> options)
            : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Meta> Metas { get; set; }
        public DbSet<Orcamento> Orcamentos { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Transacao>().Property(t => t.Valor).HasColumnType("decimal(18,2)");


            base.OnModelCreating(modelBuilder);
        }
    }
}

