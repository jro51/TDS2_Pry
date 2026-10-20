using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using TDS2_Clase01.Controllers;
using TDS2_Clase01.Data.Interface;
using TDS2_Clase01.Models.Dtos;
using TDS2_Clase01.Models.Entidades;
using TDS2_Clase01.Tests.Helpers;

namespace TDS2_Clase01.Tests.Controllers
{
    public class ColaboradorAPIControllerTests
    {
        private readonly IDAColaborador _daColaborador = Substitute.For<IDAColaborador>();
        private readonly IDAEmpresa _daEmpresa = Substitute.For<IDAEmpresa>();
        private readonly ColaboradorAPIController _controller;

        public ColaboradorAPIControllerTests()
        {
            _controller = new ColaboradorAPIController(
                _daColaborador, _daEmpresa, NullLogger<ColaboradorAPIController>.Instance);
        }

        private void ExisteLaEmpresa(int id) =>
            _daEmpresa.GetEmpresa().Returns(new[] { Datos.NuevaEmpresa(id) });

        // ---------- Listar ----------

        [Fact]
        public void Listar_DevuelveOkConLosColaboradores()
        {
            var lista = new[] { Datos.NuevoColaborador(1), Datos.NuevoColaborador(2) };
            _daColaborador.GetColaborador().Returns(lista);

            var ok = Assert.IsType<OkObjectResult>(_controller.Listar());

            Assert.Same(lista, ok.Value);
        }

        [Fact]
        public void ListarEmpresaAngular_DevuelveOkConLasEmpresas()
        {
            ExisteLaEmpresa(1);

            var ok = Assert.IsType<OkObjectResult>(_controller.ListarEmpresaAngular());

            Assert.Single(Assert.IsAssignableFrom<IEnumerable<Empresa>>(ok.Value));
        }

        // ---------- Registrar ----------

        [Fact]
        public void Registrar_SinEmpresaValida_DevuelveBadRequest()
        {
            var entidad = Datos.NuevoColaborador(idEmpresa: 0);

            Assert.IsType<BadRequestObjectResult>(_controller.Registrar(entidad));
            _daColaborador.DidNotReceive().InsertColaborador(Arg.Any<Colaborador>());
        }

        [Fact]
        public void Registrar_ConCamposEnBlanco_DevuelveBadRequest()
        {
            var entidad = Datos.NuevoColaborador(idEmpresa: 1);
            entidad.Nombres = "   ";

            Assert.IsType<BadRequestObjectResult>(_controller.Registrar(entidad));
            _daColaborador.DidNotReceive().InsertColaborador(Arg.Any<Colaborador>());
        }

        [Fact]
        public void Registrar_EmpresaInexistente_DevuelveBadRequest()
        {
            ExisteLaEmpresa(1);

            var resultado = _controller.Registrar(Datos.NuevoColaborador(idEmpresa: 99));

            var bad = Assert.IsType<BadRequestObjectResult>(resultado);
            Assert.Equal("La empresa indicada no existe", Assert.IsType<MensajeRespuesta>(bad.Value).Mensaje);
            _daColaborador.DidNotReceive().InsertColaborador(Arg.Any<Colaborador>());
        }

        [Fact]
        public void Registrar_Valido_DevuelveOkConElIdGeneradoYAsignaLaFecha()
        {
            ExisteLaEmpresa(1);
            _daColaborador.InsertColaborador(Arg.Any<Colaborador>()).Returns(42);
            var entidad = Datos.NuevoColaborador(idEmpresa: 1);
            entidad.FechaRegistro = default;

            var resultado = _controller.Registrar(entidad);

            var ok = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(42, Assert.IsType<RegistroColaboradorRespuesta>(ok.Value).IdColaborador);
            Assert.NotEqual(default, entidad.FechaRegistro);
        }

        [Fact]
        public void Registrar_ErrorInesperado_Devuelve500SinFiltrarElDetalleInterno()
        {
            ExisteLaEmpresa(1);
            _daColaborador.InsertColaborador(Arg.Any<Colaborador>())
                .Throws(new InvalidOperationException("detalle interno secreto"));

            var resultado = _controller.Registrar(Datos.NuevoColaborador(idEmpresa: 1));

            var error = Assert.IsType<ObjectResult>(resultado);
            Assert.Equal(500, error.StatusCode);
            var mensaje = Assert.IsType<MensajeRespuesta>(error.Value).Mensaje;
            Assert.DoesNotContain("secreto", mensaje);
        }

        // ---------- Detalle ----------

        [Fact]
        public void Detalle_IdInvalido_DevuelveBadRequest()
        {
            Assert.IsType<BadRequestObjectResult>(_controller.Detalle(0));
        }

        [Fact]
        public void Detalle_NoExiste_DevuelveNotFound()
        {
            _daColaborador.GetIdColaborador(5).Returns((Colaborador?)null);

            Assert.IsType<NotFoundObjectResult>(_controller.Detalle(5));
        }

        [Fact]
        public void Detalle_Existe_DevuelveOkConElColaborador()
        {
            var colaborador = Datos.NuevoColaborador(5);
            _daColaborador.GetIdColaborador(5).Returns(colaborador);

            var ok = Assert.IsType<OkObjectResult>(_controller.Detalle(5));

            Assert.Same(colaborador, ok.Value);
        }

        // ---------- Editar ----------

        [Fact]
        public void Editar_IdDeRutaNoCoincideConElDelCuerpo_DevuelveBadRequest()
        {
            Assert.IsType<BadRequestObjectResult>(_controller.Editar(1, Datos.NuevoColaborador(2)));
            _daColaborador.DidNotReceive().UpdateColaborador(Arg.Any<Colaborador>());
        }

        [Fact]
        public void Editar_ColaboradorNoExiste_DevuelveNotFound()
        {
            // Regresión: antes la condición estaba invertida y un colaborador
            // existente devolvía 404 mientras que uno inexistente provocaba un error.
            _daColaborador.GetIdColaborador(7).Returns((Colaborador?)null);

            var resultado = _controller.Editar(7, Datos.NuevoColaborador(7));

            Assert.IsType<NotFoundObjectResult>(resultado);
            _daColaborador.DidNotReceive().UpdateColaborador(Arg.Any<Colaborador>());
        }

        [Fact]
        public void Editar_Existente_ConservaLaFechaDeRegistroYActualiza()
        {
            var fechaOriginal = new DateTime(2020, 1, 1);
            var actual = Datos.NuevoColaborador(7);
            actual.FechaRegistro = fechaOriginal;
            _daColaborador.GetIdColaborador(7).Returns(actual);
            _daColaborador.UpdateColaborador(Arg.Any<Colaborador>()).Returns(true);
            var cambios = Datos.NuevoColaborador(7);
            cambios.FechaRegistro = default;

            var resultado = _controller.Editar(7, cambios);

            Assert.IsType<OkObjectResult>(resultado);
            _daColaborador.Received(1).UpdateColaborador(
                Arg.Is<Colaborador>(c => c.FechaRegistro == fechaOriginal && c.Modificacion != null));
        }

        [Fact]
        public void Editar_CuandoElUpdateFalla_DevuelveBadRequest()
        {
            _daColaborador.GetIdColaborador(7).Returns(Datos.NuevoColaborador(7));
            _daColaborador.UpdateColaborador(Arg.Any<Colaborador>()).Returns(false);

            Assert.IsType<BadRequestObjectResult>(_controller.Editar(7, Datos.NuevoColaborador(7)));
        }

        // ---------- Eliminar ----------

        [Fact]
        public void Eliminar_IdInvalido_DevuelveBadRequest()
        {
            Assert.IsType<BadRequestObjectResult>(_controller.Eliminar(-1));
        }

        [Fact]
        public void Eliminar_NoExiste_DevuelveNotFoundYNoElimina()
        {
            _daColaborador.GetIdColaborador(3).Returns((Colaborador?)null);

            Assert.IsType<NotFoundObjectResult>(_controller.Eliminar(3));
            _daColaborador.DidNotReceive().DeleteColaborador(Arg.Any<int>());
        }

        [Fact]
        public void Eliminar_Existe_DevuelveOk()
        {
            _daColaborador.GetIdColaborador(3).Returns(Datos.NuevoColaborador(3));
            _daColaborador.DeleteColaborador(3).Returns(true);

            Assert.IsType<OkObjectResult>(_controller.Eliminar(3));
        }

        [Fact]
        public void Eliminar_CuandoElDeleteFalla_DevuelveBadRequest()
        {
            _daColaborador.GetIdColaborador(3).Returns(Datos.NuevoColaborador(3));
            _daColaborador.DeleteColaborador(3).Returns(false);

            Assert.IsType<BadRequestObjectResult>(_controller.Eliminar(3));
        }
    }
}
