using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using TDS2_Clase01.Data.AccesoDatos;
using TDS2_Clase01.Models.Entidades;

namespace TDS2_Clase01.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [AllowAnonymous]
    [EnableCors("AngularPolicy")]
    public class ColaboradorAPIController : ControllerBase
    {
        [HttpGet("listar")]
        public IActionResult Index()
        {
            var model = new DAColaborador();
            var listado = model.GetColaborador();
            return Ok(listado);
        }

        //Para probar el listado de empresas
        [HttpGet("listarEmpresa")]
        public IActionResult ListarEmpresaAngular()
        {
            var model = new DAEmpresa();
            var listado = model.GetEmpresa();
            return Ok(listado);
        }

        [HttpPost]
        public IActionResult Registrar([FromBody] Colaborador entidad)
        {
            if (entidad == null)
            {
                return BadRequest(new { mensaje = "No se enviaron datos" });
            }
            if (string.IsNullOrWhiteSpace(entidad.Nombres))
            {
                return BadRequest(new { mensaje = "Debe de ingresar los nombres" });
            }
            if (string.IsNullOrWhiteSpace(entidad.Apellidos))
            {
                return BadRequest(new { mensaje = "Debe de ingresar los apellidos" });
            }
            if (string.IsNullOrWhiteSpace(entidad.Sexo))
            {
                return BadRequest(new { mensaje = "Debe de ingresar el sexo" });
            }
            if (string.IsNullOrWhiteSpace(entidad.Direccion))
            {
                return BadRequest(new { mensaje = "Debe de ingresar la dirección" });
            }
            if (entidad.IdEmpresa <= 0)
            {
                return BadRequest(new { mensaje = "Debe de ingresar una empresa válida" });
            }
            try
            {
                entidad.FechaRegistro = DateTime.Now;
                entidad.Modificacion = null;
                var model = new DAColaborador();
                var idGenerado = model.InsertColaborador(entidad);
                return Ok(new
                {
                    mensaje = "Colaborador registrado correctamente",
                    idColaborador = idGenerado
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, new
                {
                    mensaje = "Ocurrió un error al registrar el colaborador",
                    detalle = e.Message
                });
            }
        }

        [HttpGet("detalle/{id}")]
        public IActionResult Detalle(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "El id no es valido"
                });
            }
            try
            {
                var model = new DAColaborador();
                var resultado = model.GetIdColaborador(id);
                if (resultado == null)
                {
                    return NotFound(new
                    {
                        mensaje = "No se encontro el colaborador"
                    });
                }
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Ocurrio un error al obtener el detalle del colaborador",
                    detalle = ex.Message
                });
            }
        }


        [HttpPut("editar/{id}")]
        public IActionResult Editar(int id, [FromBody] Colaborador entidad)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "El id no es válido"
                });
            }
            if(entidad == null)
            {
                return BadRequest(new
                {
                    mensaje = "No se enviaron datos"
                });
            }
            if (id != entidad.IdColaborador)
            {
                return BadRequest(new
                {
                    mensaje = "El id de la ruta no conicide con el id del objeto"
                });
            }
            if (string.IsNullOrWhiteSpace(entidad.Nombres))
            {
                return BadRequest(new
                {
                    mensaje = "Debe de ingresar los nombres"
                });
            }
            if (string.IsNullOrWhiteSpace(entidad.Apellidos))
            {
                return BadRequest(new
                {
                    mensaje = "Debe de ingresar los apellidos"
                });
            }
            if (string.IsNullOrWhiteSpace(entidad.DNI))
            {
                return BadRequest(new
                {
                    mensaje = "Debe de ingresar el DNI"
                });
            }
            if (string.IsNullOrWhiteSpace(entidad.Sexo))
            {
                return BadRequest(new
                {
                    mensaje = "Debe de ingresar el sexo"
                });
            }
            if (string.IsNullOrWhiteSpace(entidad.Direccion))
            {
                return BadRequest(new
                {
                    mensaje = "Debe de ingresar la direccion"
                });
            }
            if (entidad.IdEmpresa <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "Debe de ingresar una empresa valida"
                });
            }

            try
            {
                var model = new DAColaborador();
                var actual = model.GetIdColaborador(id);
                if (actual != null)
                {
                    return NotFound(new
                    {
                        mensaje = "No se encontro el colaborador para editar"
                    });
                }
                entidad.FechaRegistro = actual.FechaRegistro;
                entidad.Modificacion = DateTime.Now;
                var resultado = model.UpdateColaborador(entidad);
                if (!resultado)
                {
                    return BadRequest(new
                    {
                        mensaje = "No se pudo actualizar el colaborador"
                    });
                }
                return Ok(new
                {
                    mensaje = "Colaborador actualizado correctamente"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Ocurrio un error al obtener al editar el colaborador",
                    detalle = ex.Message
                });
            }
        }

        [HttpDelete("eliminar/{id}")]
        public IActionResult Eliminar(int id)
        {
            if(id <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "El id no es válido"
                });
            }
            try
            {
                var model = new DAColaborador();
                var actual = model.GetIdColaborador(id);
                if(actual == null)
                {
                    return NotFound(new
                    {
                        mensaje = "No se encontró el código del colaborador"
                    });
                }
                var resultado = model.DeleteColaborador(id);
                if (!resultado)
                {
                    return BadRequest(new
                    {
                        mensaje = "No se puede eliminar al colaborador"
                    });
                }
                return Ok(new
                {
                    mensaje = "Colaborador eliminado correctamente"
                });
            }
            catch (Exception ex) {
                return StatusCode(500, new
                {
                    mensaje = "Ocurrió un error al eliminar el colaborador",
                    detalle = ex.Message
                });
            }

            
        }

    }
}
