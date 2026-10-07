using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TDS2_Clase01.Data.AccesoDatos;
using TDS2_Clase01.Data.Interface;
using TDS2_Clase01.Models.Entidades;
using X.PagedList.Extensions;

namespace TDS2_Clase01.Controllers
{
    [Authorize]
    public class ColaboradorController : Controller
    {
        private readonly IDAColaborador DAColaborador;
        private readonly IDAEmpresa DAEmpresa;

        public ColaboradorController(IDAColaborador daColaborador, IDAEmpresa daEmpresa)
        {
            DAColaborador = daColaborador;
            DAEmpresa = daEmpresa;
        }



        public IActionResult ListadoColaborador(int page=1)
        {
            var pageNumber = page;
            var modelo = DAColaborador.GetColaborador();
            //var model = new DAColaborador();
            //var listado = model.GetColaborador();
            var listado = modelo.OrderByDescending(x => x.IdColaborador).ToList().ToPagedList(pageNumber, 8);
            return View(listado);
        }

        public IActionResult ListadoColaboradorVB()
        {

            //var model = new DAColaborador();
            //ViewBag.Colaborador = model.GetColaborador();
            ViewBag.Colaborador = DAColaborador.GetColaborador();
            return View();
        }

        public IActionResult Create()
        {
            //var ObjEmp = new DAEmpresa();
            //ViewBag.ObjEmp = ObjEmp.GetEmpresa();
            ViewBag.ObjEmp = DAEmpresa.GetEmpresa();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Colaborador Entidad)
        {
            Entidad.FechaRegistro = DateTime.Now;
            //var ObjCreate = new DAColaborador();
            //var model = ObjCreate.InsertColaborador(Entidad);
            var model = DAColaborador.InsertColaborador(Entidad);
            if(model > 0)
            {
                return RedirectToAction("ListadoColaborador");
            }
            else
            {
                return View(model);
            }
        }

        public IActionResult Details(int id)
        {
            //var ColabDet = new DAColaborador();
            //var model = ColabDet.GetIdColaborador(id);
            var model = DAColaborador.GetIdColaborador(id);
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            //var EmpDA = new DAEmpresa();
            //ViewBag.Empresa=EmpDA.GetEmpresa();
            ViewBag.Empresa = DAEmpresa.GetEmpresa();

            //var ColabDA = new DAColaborador();
            //var model = ColabDA.GetIdColaborador(id);
            var model = DAColaborador.GetIdColaborador(id);
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(Colaborador entity)
        {
            entity.Modificacion = DateTime.Now;
            //var ColEdit = new DAColaborador();
            //var resultado = ColEdit.UpdateColaborador(entity);
            var resultado = DAColaborador.UpdateColaborador(entity);
            if (resultado)
            {
                return RedirectToAction("ListadoColaborador");
            }
            else
            {
                return View(entity);
            }
        }

        public IActionResult Delete(int id)
        {
            //var model = new DAColaborador();
            //var resultado = model.GetIdColaborador(id);
            var resultado = DAColaborador.GetIdColaborador(id);
            return View(resultado);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            //var DeleteColaborador = new DAColaborador();
            //var model = DeleteColaborador.DeleteColaborador(id);
            var model = DAColaborador.DeleteColaborador(id);
            return RedirectToAction("ListadoColaborador");
        }

    }
}
