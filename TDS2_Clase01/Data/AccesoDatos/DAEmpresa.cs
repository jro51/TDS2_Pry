using TDS2_Clase01.Data.Interface;
using TDS2_Clase01.Models.Entidades;

namespace TDS2_Clase01.Data.AccesoDatos
{
    public class DAEmpresa : IDAEmpresa
    {
        public IEnumerable<Empresa> GetEmpresa()
        {
            var ListadoEmp = new List<Empresa>();
            using (var db = new ApplicationDbContext())
            {
                ListadoEmp = db.Empresa.ToList();
            }
            return ListadoEmp;
        }
    }
}
