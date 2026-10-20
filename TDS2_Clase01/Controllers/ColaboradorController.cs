using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TDS2_Clase01.Data.Interface;
using TDS2_Clase01.Models.Entidades;
using X.PagedList.Extensions;

namespace TDS2_Clase01.Controllers
{
    [Authorize]
    public class ColaboradorController : Controller
    {
        private const int TamanoPagina = 8;

        private readonly IDAColaborador _daColaborador;
        private readonly IDAEmpresa _daEmpresa;

        public ColaboradorController(IDAColaborador daColaborador, IDAEmpresa daEmpresa)
        {
            _daColaborador = daColaborador;
            _daEmpresa = daEmpresa;
        }

        public IActionResult ListadoColaborador(int page = 1)
        {
            var listado = _daColaborador.GetColaborador()
                .OrderByDescending(x => x.IdColaborador)
                .ToList()
                .ToPagedList(Math.Max(page, 1), TamanoPagina);
            return View(listado);
        }

        public IActionResult ListadoColaboradorVB()
        {
            ViewBag.Colaborador = _daColaborador.GetColaborador();
            return View();
        }

        public IActionResult Create()
        {
            ViewBag.ObjEmp = _daEmpresa.GetEmpresa();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Colaborador entidad)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ObjEmp = _daEmpresa.GetEmpresa();
                return View(entidad);
            }

            entidad.FechaRegistro = DateTime.Now;
            var id = _daColaborador.InsertColaborador(entidad);
            if (id > 0)
            {
                return RedirectToAction(nameof(ListadoColaborador));
            }

            ModelState.AddModelError(string.Empty, "No se pudo registrar el colaborador.");
            ViewBag.ObjEmp = _daEmpresa.GetEmpresa();
            return View(entidad);
        }

        public IActionResult Details(int id)
        {
            var model = _daColaborador.GetIdColaborador(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            var model = _daColaborador.GetIdColaborador(id);
            if (model == null)
            {
                return NotFound();
            }
            ViewBag.Empresa = _daEmpresa.GetEmpresa();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Colaborador entity)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Empresa = _daEmpresa.GetEmpresa();
                return View(entity);
            }

            entity.Modificacion = DateTime.Now;
            if (_daColaborador.UpdateColaborador(entity))
            {
                return RedirectToAction(nameof(ListadoColaborador));
            }

            ModelState.AddModelError(string.Empty, "No se pudo actualizar el colaborador.");
            ViewBag.Empresa = _daEmpresa.GetEmpresa();
            return View(entity);
        }

        public IActionResult Delete(int id)
        {
            var resultado = _daColaborador.GetIdColaborador(id);
            if (resultado == null)
            {
                return NotFound();
            }
            return View(resultado);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _daColaborador.DeleteColaborador(id);
            return RedirectToAction(nameof(ListadoColaborador));
        }
    }
}
