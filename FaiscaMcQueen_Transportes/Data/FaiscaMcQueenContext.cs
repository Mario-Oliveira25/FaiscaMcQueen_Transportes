using FaiscaMcQueen_Transportes.Data.FaiscaMcQueen;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FaiscaMcQueen_Transportes.Data
{
    public class FaiscaMcQueenContext : IdentityDbContext<IdentityUser>
    {
        public FaiscaMcQueenContext(DbContextOptions<FaiscaMcQueenContext> options) : base(options) { }

        public DbSet<Ativo> Ativos { get; set; } = default!;
    }
}
