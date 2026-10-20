using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using TDS2_Clase01.Controllers;
using TDS2_Clase01.Data.Interface;
using TDS2_Clase01.Models.Entidades;
using TDS2_Clase01.Tests.Helpers;
using X.PagedList;

namespace TDS2_Clase01.Tests.Controllers
{
    public class ColaboradorControllerTests
    {
        private readonly IDAColaborador _daColaborador = Substitute.For<IDAColaborador>();
        private readonly IDAEmpresa _daEmpresa = Substitute.For<IDAEmpresa>();
        private readonly ColaboradorController _controller;

        public ColaboradorControllerTests()
        {
            _controller = new ColaboradorController(_daColaborador, _daEmpresa);
            _daEmpresa.GetEmpresa().Returns(new[] { Datos.NuevaEmpresa(1) });
        }

        // ---------- Listado ----------

        // View() guarda el modelo en el ViewData del controlador, que se comparte entre
        // llamadas sucesivas: por eso el modelo se lee justo después de cada llamada.
        private List<int> IdsDeLaPagina(int pagina)
        {
            var vista = Assert.IsType<ViewResult>(_controller.ListadoColaborador(pagina));
            return Assert.IsAssignableFrom<IPagedList<Colaborador>>(vista.Model)
                .Select(c => c.IdColaborador).ToList();
        }

        [Fact]
        public void ListadoColaborador_PaginaDeAOchoYOrdenaDelMasNuevoAlMasAntiguo()
        {
            _daColaborador.GetColaborador()
                .Returns(Enumerable.Range(1, 10).Select(i => Datos.NuevoColaborador(i)).ToList());

            var primera = IdsDeLaPagina(1);
            var segunda = IdsDeLaPagina(2);

            Assert.Equal(8, primera.Count);
            Assert.Equal(10, primera.First());
            Assert.Equal(new[] { 2, 1 }, segunda);
        }

        [Fact]
        public void ListadoColaborador_ConPaginaCero_NoFallaYMuestraLaPrimera()
        {
            _daColaborador.GetColaborador().Returns(new List<Colaborador> { Datos.NuevoColaborador(1) });

            var vista = Assert.IsType<ViewResult>(_controller.ListadoColaborador(0));

            Assert.Single(Assert.IsAssignableFrom<IPagedList<Colaborador>>(vista.Model));
        }

        // ---------- Create ----------

        [Fact]
        public void Create_Get_CargaLasEmpresasEnElViewBag()
        {
            var vista = Assert.IsType<ViewResult>(_controller.Create());

            Assert.NotNull(vista.ViewData["ObjEmp"]);
        }

        [Fact]
        public void Create_Post_ModeloInvalido_VuelveALaVistaSinGuardar()
        {
            var entidad = Datos.NuevoColaborador();
            _controller.ModelState.AddModelError("Nombres", "Requerido");

            var vista = Assert.IsType<ViewResult>(_controller.Create(entidad));

            Assert.Same(entidad, vista.Model);
            Assert.NotNull(vista.ViewData["ObjEmp"]);
            _daColaborador.DidNotReceive().InsertColaborador(Arg.Any<Colaborador>());
        }

        [Fact]
        public void Create_Post_Valido_GuardaYRedirigeAlListado()
        {
            _daColaborador.InsertColaborador(Arg.Any<Colaborador>()).Returns(5);
            var entidad = Datos.NuevoColaborador();
            entidad.FechaRegistro = default;

            var resultado = _controller.Create(entidad);

            var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal("ListadoColaborador", redireccion.ActionName);
            Assert.NotEqual(default, entidad.FechaRegistro);
        }

        [Fact]
        public void Create_Post_CuandoNoSeInserta_VuelveALaVistaConError()
        {
            _daColaborador.InsertColaborador(Arg.Any<Colaborador>()).Returns(0);

            var resultado = _controller.Create(Datos.NuevoColaborador());

            Assert.IsType<ViewResult>(resultado);
            Assert.False(_controller.ModelState.IsValid);
        }

        // ---------- Details / Edit / Delete ----------

        [Fact]
        public void Details_NoExiste_DevuelveNotFound()
        {
            _daColaborador.GetIdColaborador(9).Returns((Colaborador?)null);

            Assert.IsType<NotFoundResult>(_controller.Details(9));
        }

        [Fact]
        public void Details_Existe_DevuelveLaVistaConElModelo()
        {
            var colaborador = Datos.NuevoColaborador(9);
            _daColaborador.GetIdColaborador(9).Returns(colaborador);

            var vista = Assert.IsType<ViewResult>(_controller.Details(9));

            Assert.Same(colaborador, vista.Model);
        }

        [Fact]
        public void Edit_Get_NoExiste_DevuelveNotFound()
        {
            _daColaborador.GetIdColaborador(9).Returns((Colaborador?)null);

            Assert.IsType<NotFoundResult>(_controller.Edit(9));
        }

        [Fact]
        public void Edit_Get_Existe_CargaModeloYEmpresas()
        {
            _daColaborador.GetIdColaborador(9).Returns(Datos.NuevoColaborador(9));

            var vista = Assert.IsType<ViewResult>(_controller.Edit(9));

            Assert.NotNull(vista.Model);
            Assert.NotNull(vista.ViewData["Empresa"]);
        }

        [Fact]
        public void Edit_Post_ModeloInvalido_VuelveALaVistaSinActualizar()
        {
            _controller.ModelState.AddModelError("DNI", "Requerido");

            var resultado = _controller.Edit(Datos.NuevoColaborador(9));

            Assert.IsType<ViewResult>(resultado);
            _daColaborador.DidNotReceive().UpdateColaborador(Arg.Any<Colaborador>());
        }

        [Fact]
        public void Edit_Post_Valido_ActualizaYRedirige()
        {
            _daColaborador.UpdateColaborador(Arg.Any<Colaborador>()).Returns(true);

            var resultado = _controller.Edit(Datos.NuevoColaborador(9));

            Assert.Equal("ListadoColaborador", Assert.IsType<RedirectToActionResult>(resultado).ActionName);
        }

        [Fact]
        public void Edit_Post_CuandoNoSeActualiza_VuelveALaVistaConError()
        {
            _daColaborador.UpdateColaborador(Arg.Any<Colaborador>()).Returns(false);

            var resultado = _controller.Edit(Datos.NuevoColaborador(9));

            Assert.IsType<ViewResult>(resultado);
            Assert.False(_controller.ModelState.IsValid);
        }

        [Fact]
        public void Delete_Get_NoExiste_DevuelveNotFound()
        {
            _daColaborador.GetIdColaborador(9).Returns((Colaborador?)null);

            Assert.IsType<NotFoundResult>(_controller.Delete(9));
        }

        [Fact]
        public void DeleteConfirmed_EliminaYRedirigeAlListado()
        {
            var resultado = _controller.DeleteConfirmed(9);

            _daColaborador.Received(1).DeleteColaborador(9);
            Assert.Equal("ListadoColaborador", Assert.IsType<RedirectToActionResult>(resultado).ActionName);
        }

        // ---------- Seguridad ----------

        [Fact]
        public void TodasLasAccionesPost_ExigenTokenAntiforgery()
        {
            var accionesPost = typeof(ColaboradorController)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => m.GetCustomAttribute<HttpPostAttribute>() != null)
                .ToList();

            Assert.NotEmpty(accionesPost);
            Assert.All(accionesPost, m =>
                Assert.NotNull(m.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>()));
        }
    }
}
