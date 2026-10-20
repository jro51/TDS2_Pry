using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TDS2_Clase01.Data;
using TDS2_Clase01.Tests.Helpers;

namespace TDS2_Clase01.Tests.Integration
{
    /// <summary>
    /// Levanta la aplicación real en memoria, reemplazando SQL Server por una base InMemory
    /// (con una empresa sembrada) para que los tests no dependan de ninguna base externa.
    /// </summary>
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _nombreBase = Guid.NewGuid().ToString();

        /// <summary>Id de la empresa sembrada, para usarla en los registros de prueba.</summary>
        public int EmpresaId { get; private set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            builder.ConfigureServices(services =>
            {
                // Quita TODO lo que EF registró para ApplicationDbContext (DbContextOptions<T>,
                // IDbContextOptionsConfiguration<T>, ...) para no mezclar SqlServer con InMemory.
                var registrosEf = services
                    .Where(d => d.ServiceType.IsGenericType
                                && d.ServiceType.GetGenericArguments().Contains(typeof(ApplicationDbContext)))
                    .ToList();
                foreach (var registro in registrosEf)
                {
                    services.Remove(registro);
                }

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase(_nombreBase));
            });
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);

            using var scope = host.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var empresa = Datos.NuevaEmpresa();
            db.Empresa.Add(empresa);
            db.SaveChanges();
            EmpresaId = empresa.IdEmpresa;

            return host;
        }
    }
}
