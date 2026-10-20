using Microsoft.EntityFrameworkCore;
using TDS2_Clase01.Data.Interface;
using TDS2_Clase01.Models.Entidades;

namespace TDS2_Clase01.Data.AccesoDatos
{
    public class DAEmpresa : IDAEmpresa
    {
        private readonly ApplicationDbContext _db;

        public DAEmpresa(ApplicationDbContext db)
        {
            _db = db;
        }

        public IEnumerable<Empresa> GetEmpresa()
        {
            return _db.Empresa.AsNoTracking().ToList();
        }
    }
}
