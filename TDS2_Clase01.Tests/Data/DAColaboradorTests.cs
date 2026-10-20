using TDS2_Clase01.Data;
using TDS2_Clase01.Data.AccesoDatos;
using TDS2_Clase01.Tests.Helpers;

namespace TDS2_Clase01.Tests.Data
{
    public class DAColaboradorTests
    {
        [Fact]
        public void InsertColaborador_PersisteYDevuelveElIdGenerado()
        {
            var options = Datos.NuevaBase();
            int id;
            using (var db = new ApplicationDbContext(options))
            {
                id = new DAColaborador(db).InsertColaborador(Datos.NuevoColaborador());
            }

            Assert.True(id > 0);
            using var verificacion = new ApplicationDbContext(options);
            Assert.Equal("Ana", verificacion.Colaborador.Single(c => c.IdColaborador == id).Nombres);
        }

        [Fact]
        public void GetColaborador_IncluyeLaEmpresaRelacionada()
        {
            var options = Datos.NuevaBase();
            using (var arrange = new ApplicationDbContext(options))
            {
                var colaborador = Datos.NuevoColaborador();
                colaborador.Empresa = Datos.NuevaEmpresa();
                arrange.Colaborador.Add(colaborador);
                arrange.SaveChanges();
            }

            using var db = new ApplicationDbContext(options);
            var resultado = new DAColaborador(db).GetColaborador().ToList();

            var unico = Assert.Single(resultado);
            Assert.NotNull(unico.Empresa);
            Assert.Equal("Industrias JF S.A.C.", unico.Empresa!.RazonSocial);
        }

        [Fact]
        public void GetIdColaborador_Existente_DevuelveElColaborador()
        {
            var options = Datos.NuevaBase();
            int id;
            using (var arrange = new ApplicationDbContext(options))
            {
                var colaborador = Datos.NuevoColaborador();
                arrange.Colaborador.Add(colaborador);
                arrange.SaveChanges();
                id = colaborador.IdColaborador;
            }

            using var db = new ApplicationDbContext(options);
            var resultado = new DAColaborador(db).GetIdColaborador(id);

            Assert.NotNull(resultado);
            Assert.Equal(id, resultado!.IdColaborador);
        }

        [Fact]
        public void GetIdColaborador_Inexistente_DevuelveNull()
        {
            using var db = new ApplicationDbContext(Datos.NuevaBase());

            Assert.Null(new DAColaborador(db).GetIdColaborador(999));
        }

        [Fact]
        public void UpdateColaborador_CambiaLosCamposPeroNoLaFechaDeRegistro()
        {
            var options = Datos.NuevaBase();
            int id;
            using (var arrange = new ApplicationDbContext(options))
            {
                var original = Datos.NuevoColaborador();
                arrange.Colaborador.Add(original);
                arrange.SaveChanges();
                id = original.IdColaborador;
            }

            bool actualizado;
            using (var db = new ApplicationDbContext(options))
            {
                var cambios = Datos.NuevoColaborador(id);
                cambios.Direccion = "Calle Nueva 456";
                cambios.FechaRegistro = DateTime.MinValue; // no debe sobrescribirse
                cambios.Modificacion = new DateTime(2026, 5, 1);
                actualizado = new DAColaborador(db).UpdateColaborador(cambios);
            }

            Assert.True(actualizado);
            using var verificacion = new ApplicationDbContext(options);
            var guardado = verificacion.Colaborador.Single(c => c.IdColaborador == id);
            Assert.Equal("Calle Nueva 456", guardado.Direccion);
            Assert.Equal(new DateTime(2020, 1, 1), guardado.FechaRegistro);
            Assert.Equal(new DateTime(2026, 5, 1), guardado.Modificacion);
        }

        [Fact]
        public void UpdateColaborador_Inexistente_DevuelveFalse()
        {
            using var db = new ApplicationDbContext(Datos.NuevaBase());

            Assert.False(new DAColaborador(db).UpdateColaborador(Datos.NuevoColaborador(999)));
        }

        [Fact]
        public void DeleteColaborador_Existente_LoEliminaYDevuelveTrue()
        {
            var options = Datos.NuevaBase();
            int id;
            using (var arrange = new ApplicationDbContext(options))
            {
                var colaborador = Datos.NuevoColaborador();
                arrange.Colaborador.Add(colaborador);
                arrange.SaveChanges();
                id = colaborador.IdColaborador;
            }

            bool eliminado;
            using (var db = new ApplicationDbContext(options))
            {
                eliminado = new DAColaborador(db).DeleteColaborador(id);
            }

            Assert.True(eliminado);
            using var verificacion = new ApplicationDbContext(options);
            Assert.Empty(verificacion.Colaborador);
        }

        [Fact]
        public void DeleteColaborador_Inexistente_DevuelveFalse()
        {
            using var db = new ApplicationDbContext(Datos.NuevaBase());

            Assert.False(new DAColaborador(db).DeleteColaborador(999));
        }
    }
}
