using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FaiscaMcQueen_Transportes.Data
{
    public class FaiscaMcQueenContext : IdentityDbContext<IdentityUser>
    {
        public FaiscaMcQueenContext(DbContextOptions<FaiscaMcQueenContext> options) : base(options) 
        {
        
        }

        public DbSet<Ativo> Ativos { get; set; }
        public DbSet<Tecnico> Tecnicos { get; set; }
        public DbSet<Intervencao> Intervencoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUser>()
                .HasIndex(user => user.Email)
                .IsUnique();

            modelBuilder.Entity<Intervencao>(entity =>
            {
                entity.HasKey(i => i.Id);

                // Um Ativo tem Muitas Intervenções
                entity.HasOne(intervencao => intervencao.Ativo)
                    .WithMany(ativo => ativo.Intervencoes)
                    .HasForeignKey(intervencao => intervencao.AtivoId)
                    .OnDelete(DeleteBehavior.Restrict); // Evita apagar o ativo se houver histórico

                //Um Técnico tem Muitas Intervenções
                entity.HasOne(intervencao => intervencao.Tecnico)
                    .WithMany(tecnico => tecnico.Intervencoes)
                    .HasForeignKey(intervencao => intervencao.TecnicoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

        }
    }
}
