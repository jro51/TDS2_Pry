using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TDS2_Clase01.Models.Entidades;

namespace TDS2_Clase01.Data
{
    /// <summary>
    /// Contexto de EF Core de la aplicación (Identity + Colaboradores + Empresas).
    /// La cadena de conexión se configura en <c>Program.cs</c> a partir de
    /// <c>ConnectionStrings:DefaultConnection</c> (appsettings, user-secrets o variables de entorno).
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Colaborador> Colaborador { get; set; }

        public DbSet<Empresa> Empresa { get; set; }
    }
}
