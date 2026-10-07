using TDS2_Clase01.Models.Entidades;

namespace TDS2_Clase01.Data.Interface
{
    public interface IDAEmpresa
    {
        IEnumerable<Empresa> GetEmpresa();
    }
}
