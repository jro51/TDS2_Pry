using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TDS2_Clase01.Data;
using TDS2_Clase01.Models.Entidades;

namespace TDS2_Clase01.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ColaboradorApiRestController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        
        public ColaboradorApiRestController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Colaborador>>> GetColaborador()
        {
            return await context.Colaborador.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Colaborador>>GetColaborador(int id)
        {
            var colaborador = await context.Colaborador.FindAsync(id);
            if(colaborador == null)
            {
                return NotFound();
            }
            return colaborador;
        }

        [HttpPost]
        public async Task<ActionResult<Colaborador>> PostColaborador(Colaborador colaborador)
        {
            context.Colaborador.Add(colaborador);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetColaborador), new {id=colaborador.IdColaborador}, colaborador);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult>PutColaborador(int id, Colaborador colaborador)
        {
            if (id!=colaborador.IdColaborador)
            {
                return BadRequest();
            }
            context.Entry(colaborador).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if (!context.Colaborador.Any(e => e.IdColaborador == id))
                    return NotFound();
                else
                    throw;
            }
            return NoContent();
        }



    }
}
