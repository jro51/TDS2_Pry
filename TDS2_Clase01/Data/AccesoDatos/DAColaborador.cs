using Microsoft.EntityFrameworkCore;
using TDS2_Clase01.Data.Interface;
using TDS2_Clase01.Models.Entidades;

namespace TDS2_Clase01.Data.AccesoDatos
{
    public class DAColaborador : IDAColaborador
    {
        public IEnumerable<Colaborador> GetColaborador()
        {
            var Listado = new List<Colaborador>();
            using (var db=new ApplicationDbContext())
            {
                Listado=db.Colaborador.Include(item => item.Empresa).ToList();
            }

            return Listado;
        }

        public int InsertColaborador(Colaborador Entidad)
        {
            var resultado = 0;
            using (var db = new ApplicationDbContext()) {
                db.Add(Entidad); //Seleccionamos la fila del registro
                db.SaveChanges(); //Guardamos en la bd
                resultado = Entidad.IdColaborador;
            }
            return resultado;
        }

        public Colaborador GetIdColaborador(int id)
        {
            var resultado = new Colaborador();
            using (var db = new ApplicationDbContext())
            {
                resultado = db.Colaborador.Where(item => item.IdColaborador == id).FirstOrDefault();
            }
            return resultado;
        }

        public Boolean UpdateColaborador(Colaborador Entity) {
            var resultado = false;
            using (var db = new ApplicationDbContext()) {
                db.Colaborador.Attach(Entity);
                db.Entry(Entity).State = EntityState.Modified;
                db.Entry(Entity).Property(item => item.FechaRegistro).IsModified = false;
                resultado = db.SaveChanges() != 0;
            }
            return resultado;
        }

        public Boolean DeleteColaborador(int id) {
            var resultado = false;
            using (var db = new ApplicationDbContext()) {
                var entity = new Colaborador() { IdColaborador = id};
                db.Colaborador.Attach(entity);
                db.Colaborador.Remove(entity);
                resultado = db.SaveChanges() != 0;
            }
            return resultado;
        }
    }
}
