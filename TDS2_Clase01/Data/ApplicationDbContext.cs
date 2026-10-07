using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TDS2_Clase01.Models.Entidades;

namespace TDS2_Clase01.Data
{
    public class ApplicationDbContext: IdentityDbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Colaborador> Colaborador { get; set; }

        public DbSet<Empresa> Empresa { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=JULIO\\SQLEXPRESS;" +
                "Database=DBIndustriasJFTDS_II_26_I;" +
                "User Id=sa;" +
                "Password=123;" + 
                "MultipleActiveResultSets=True;" +
                "Encrypt=false;"
                );
        }
    }
}
