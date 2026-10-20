using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using TDS2_Clase01.Data.Interface;
using TDS2_Clase01.Models.Dtos;
using TDS2_Clase01.Models.Entidades;

namespace TDS2_Clase01.Controllers
{
    /// <summary>
    /// API REST para administrar colaboradores y consultar empresas.
    /// Pensada para ser consumida por el frontend Angular (http://localhost:4200).
    /// </summary>
    [ApiController]
    [Route("api/[Controller]")]
    [Produces("application/json")]
    [AllowAnonymous]
    [EnableCors("AngularPolicy")]
    public class ColaboradorAPIController : ControllerBase
    {
        private readonly IDAColaborador _daColaborador;
        private readonly IDAEmpresa _daEmpresa;
        private readonly ILogger<ColaboradorAPIController> _logger;

        public ColaboradorAPIController(
            IDAColaborador daColaborador,
            IDAEmpresa daEmpresa,
            ILogger<ColaboradorAPIController> logger)
        {
            _daColaborador = daColaborador;
            _daEmpresa = daEmpresa;
            _logger = logger;
        }

        /// <summary>Lista todos los colaboradores.</summary>
        /// <response code="200">Listado de colaboradores (puede estar vacío).</response>
        [HttpGet("listar")]
        [ProducesResponseType(typeof(IEnumerable<Colaborador>), StatusCodes.Status200OK)]
        public IActionResult Listar()
        {
            return Ok(_daColaborador.GetColaborador());
        }

        /// <summary>Lista todas las empresas (útil para poblar combos en el frontend).</summary>
        /// <response code="200">Listado de empresas.</response>
        [HttpGet("listarEmpresa")]
        [ProducesResponseType(typeof(IEnumerable<Empresa>), StatusCodes.Status200OK)]
        public IActionResult ListarEmpresaAngular()
        {
            return Ok(_daEmpresa.GetEmpresa());
        }

        /// <summary>Registra un nuevo colaborador.</summary>
        /// <remarks>
        /// La fecha de registro la asigna el servidor; cualquier valor enviado se ignora.
        ///
        ///     POST /api/ColaboradorAPI
        ///     {
        ///        "nombres": "Ana",
        ///        "apellidos": "Torres Ruiz",
        ///        "dni": "12345678",
        ///        "sexo": "Femenino",
        ///        "direccion": "Av. Siempre Viva 742",
        ///        "idEmpresa": 1
        ///     }
        /// </remarks>
        /// <param name="entidad">Datos del colaborador.</param>
        /// <response code="200">Colaborador registrado; devuelve el id generado.</response>
        /// <response code="400">Datos inválidos o empresa inexistente.</response>
        /// <response code="500">Error inesperado al guardar.</response>
        [HttpPost]
        [ProducesResponseType(typeof(RegistroColaboradorRespuesta), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeRespuesta), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MensajeRespuesta), StatusCodes.Status500InternalServerError)]
        public IActionResult Registrar([FromBody] Colaborador entidad)
        {
            var error = ValidarColaborador(entidad);
            if (error != null)
            {
                return BadRequest(new MensajeRespuesta(error));
            }

            try
            {
                if (!_daEmpresa.GetEmpresa().Any(e => e.IdEmpresa == entidad.IdEmpresa))
                {
                    return BadRequest(new MensajeRespuesta("La empresa indicada no existe"));
                }

                entidad.FechaRegistro = DateTime.Now;
                entidad.Modificacion = null;
                var idGenerado = _daColaborador.InsertColaborador(entidad);
                return Ok(new RegistroColaboradorRespuesta("Colaborador registrado correctamente", idGenerado));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar el colaborador");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new MensajeRespuesta("Ocurrió un error al registrar el colaborador"));
            }
        }

        /// <summary>Obtiene el detalle de un colaborador.</summary>
        /// <param name="id">Id del colaborador (mayor que cero).</param>
        /// <response code="200">El colaborador solicitado.</response>
        /// <response code="400">El id no es válido.</response>
        /// <response code="404">No existe un colaborador con ese id.</response>
        [HttpGet("detalle/{id}")]
        [ProducesResponseType(typeof(Colaborador), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeRespuesta), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MensajeRespuesta), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(MensajeRespuesta), StatusCodes.Status500InternalServerError)]
        public IActionResult Detalle(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new MensajeRespuesta("El id no es válido"));
            }

            try
            {
                var resultado = _daColaborador.GetIdColaborador(id);
                if (resultado == null)
                {
                    return NotFound(new MensajeRespuesta("No se encontró el colaborador"));
                }
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el colaborador {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new MensajeRespuesta("Ocurrió un error al obtener el detalle del colaborador"));
            }
        }

        /// <summary>Edita un colaborador existente.</summary>
        /// <remarks>El id de la ruta debe coincidir con <c>idColaborador</c> del cuerpo.</remarks>
        /// <param name="id">Id del colaborador a editar.</param>
        /// <param name="entidad">Datos actualizados.</param>
        /// <response code="200">Colaborador actualizado.</response>
        /// <response code="400">Id inválido, no coincide con el cuerpo, o datos inválidos.</response>
        /// <response code="404">No existe un colaborador con ese id.</response>
        [HttpPut("editar/{id}")]
        [ProducesResponseType(typeof(MensajeRespuesta), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeRespuesta), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MensajeRespuesta), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(MensajeRespuesta), StatusCodes.Status500InternalServerError)]
        public IActionResult Editar(int id, [FromBody] Colaborador entidad)
        {
            if (id <= 0)
            {
                return BadRequest(new MensajeRespuesta("El id no es válido"));
            }
            if (id != entidad.IdColaborador)
            {
                return BadRequest(new MensajeRespuesta("El id de la ruta no coincide con el id del objeto"));
            }
            var error = ValidarColaborador(entidad);
            if (error != null)
            {
                return BadRequest(new MensajeRespuesta(error));
            }

            try
            {
                var actual = _daColaborador.GetIdColaborador(id);
                if (actual == null)
                {
                    return NotFound(new MensajeRespuesta("No se encontró el colaborador para editar"));
                }

                entidad.FechaRegistro = actual.FechaRegistro;
                entidad.Modificacion = DateTime.Now;
                if (!_daColaborador.UpdateColaborador(entidad))
                {
                    return BadRequest(new MensajeRespuesta("No se pudo actualizar el colaborador"));
                }
                return Ok(new MensajeRespuesta("Colaborador actualizado correctamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar el colaborador {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new MensajeRespuesta("Ocurrió un error al editar el colaborador"));
            }
        }

        /// <summary>Elimina un colaborador.</summary>
        /// <param name="id">Id del colaborador a eliminar.</param>
        /// <response code="200">Colaborador eliminado.</response>
        /// <response code="400">El id no es válido.</response>
        /// <response code="404">No existe un colaborador con ese id.</response>
        [HttpDelete("eliminar/{id}")]
        [ProducesResponseType(typeof(MensajeRespuesta), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MensajeRespuesta), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MensajeRespuesta), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(MensajeRespuesta), StatusCodes.Status500InternalServerError)]
        public IActionResult Eliminar(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new MensajeRespuesta("El id no es válido"));
            }

            try
            {
                if (_daColaborador.GetIdColaborador(id) == null)
                {
                    return NotFound(new MensajeRespuesta("No se encontró el colaborador"));
                }
                if (!_daColaborador.DeleteColaborador(id))
                {
                    return BadRequest(new MensajeRespuesta("No se puede eliminar al colaborador"));
                }
                return Ok(new MensajeRespuesta("Colaborador eliminado correctamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el colaborador {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new MensajeRespuesta("Ocurrió un error al eliminar el colaborador"));
            }
        }

        /// <summary>Validaciones de negocio comunes a registrar y editar. Devuelve el mensaje de error o null.</summary>
        private static string? ValidarColaborador(Colaborador entidad)
        {
            if (string.IsNullOrWhiteSpace(entidad.Nombres)) return "Debe de ingresar los nombres";
            if (string.IsNullOrWhiteSpace(entidad.Apellidos)) return "Debe de ingresar los apellidos";
            if (string.IsNullOrWhiteSpace(entidad.DNI)) return "Debe de ingresar el DNI";
            if (string.IsNullOrWhiteSpace(entidad.Sexo)) return "Debe de ingresar el sexo";
            if (string.IsNullOrWhiteSpace(entidad.Direccion)) return "Debe de ingresar la dirección";
            if (entidad.IdEmpresa <= 0) return "Debe de ingresar una empresa válida";
            return null;
        }
    }
}
