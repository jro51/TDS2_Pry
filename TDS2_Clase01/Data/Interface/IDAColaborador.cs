using TDS2_Clase01.Models.Entidades;

namespace TDS2_Clase01.Data.Interface
{
    public interface IDAColaborador
    {
        IEnumerable<Colaborador> GetColaborador();
        int InsertColaborador(Colaborador Entidad);
        Colaborador GetIdColaborador(int id);
        Boolean UpdateColaborador(Colaborador Entity);
        Boolean DeleteColaborador(int id);
    }
}
