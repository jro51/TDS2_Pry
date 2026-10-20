using Microsoft.EntityFrameworkCore;
using TDS2_Clase01.Data.Interface;
using TDS2_Clase01.Models.Entidades;

namespace TDS2_Clase01.Data.AccesoDatos
{
    public class DAColaborador : IDAColaborador
    {
        private readonly ApplicationDbContext _db;

        public DAColaborador(ApplicationDbContext db)
        {
            _db = db;
        }

        public IEnumerable<Colaborador> GetColaborador()
        {
            return _db.Colaborador
                .AsNoTracking()
                .Include(item => item.Empresa)
                .ToList();
        }

        public int InsertColaborador(Colaborador Entidad)
        {
            _db.Colaborador.Add(Entidad);
            _db.SaveChanges();
            return Entidad.IdColaborador;
        }

        public Colaborador? GetIdColaborador(int id)
        {
            // AsNoTracking: la entidad leída no queda rastreada y se puede
            // adjuntar luego otra instancia con el mismo Id en UpdateColaborador.
            return _db.Colaborador
                .AsNoTracking()
                .FirstOrDefault(item => item.IdColaborador == id);
        }

        public Boolean UpdateColaborador(Colaborador Entity)
        {
            var entry = _db.Colaborador.Attach(Entity);
            try
            {
                entry.State = EntityState.Modified;
                // La fecha de registro nunca se sobrescribe.
                entry.Property(item => item.FechaRegistro).IsModified = false;
                return _db.SaveChanges() != 0;
            }
            catch (DbUpdateConcurrencyException)
            {
                // El registro ya no existe.
                return false;
            }
            finally
            {
                entry.State = EntityState.Detached;
            }
        }

        public Boolean DeleteColaborador(int id)
        {
            var entity = _db.Colaborador.Find(id);
            if (entity is null)
            {
                return false;
            }

            _db.Colaborador.Remove(entity);
            return _db.SaveChanges() != 0;
        }
    }
}
