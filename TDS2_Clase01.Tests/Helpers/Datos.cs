using Microsoft.EntityFrameworkCore;
using TDS2_Clase01.Data;
using TDS2_Clase01.Models.Entidades;

namespace TDS2_Clase01.Tests.Helpers
{
    /// <summary>Utilidades para construir datos y contextos de prueba.</summary>
    internal static class Datos
    {
        /// <summary>Opciones para una base InMemory nueva y aislada por cada test.</summary>
        public static DbContextOptions<ApplicationDbContext> NuevaBase() =>
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        public static Empresa NuevaEmpresa(int id = 0) => new()
        {
            IdEmpresa = id,
            RazonSocial = "Industrias JF S.A.C.",
            RUC = "20123456789",
            Direccion = "Jr. Los Olivos 123"
        };

        public static Colaborador NuevoColaborador(int id = 0, int idEmpresa = 1) => new()
        {
            IdColaborador = id,
            Nombres = "Ana",
            Apellidos = "Torres Ruiz",
            DNI = "12345678",
            Sexo = "Femenino",
            Direccion = "Av. Siempre Viva 742",
            IdEmpresa = idEmpresa,
            FechaRegistro = new DateTime(2020, 1, 1)
        };
    }
}
