using TDS2_Clase01.Data;
using TDS2_Clase01.Data.AccesoDatos;
using TDS2_Clase01.Tests.Helpers;

namespace TDS2_Clase01.Tests.Data
{
    public class DAEmpresaTests
    {
        [Fact]
        public void GetEmpresa_DevuelveTodasLasEmpresas()
        {
            var options = Datos.NuevaBase();
            using (var arrange = new ApplicationDbContext(options))
            {
                arrange.Empresa.AddRange(Datos.NuevaEmpresa(), Datos.NuevaEmpresa());
                arrange.SaveChanges();
            }

            using var db = new ApplicationDbContext(options);

            Assert.Equal(2, new DAEmpresa(db).GetEmpresa().Count());
        }

        [Fact]
        public void GetEmpresa_SinDatos_DevuelveListaVacia()
        {
            using var db = new ApplicationDbContext(Datos.NuevaBase());

            Assert.Empty(new DAEmpresa(db).GetEmpresa());
        }
    }
}
