using TDS2_Clase01.Models.Entidades;

namespace TDS2_Clase01.Data.Interface
{
    public interface IDAColaborador
    {
        IEnumerable<Colaborador> GetColaborador();
        int InsertColaborador(Colaborador Entidad);

        /// <returns>El colaborador, o <c>null</c> si no existe.</returns>
        Colaborador? GetIdColaborador(int id);

        /// <returns><c>true</c> si se actualizó; <c>false</c> si el colaborador no existe.</returns>
        Boolean UpdateColaborador(Colaborador Entity);

        /// <returns><c>true</c> si se eliminó; <c>false</c> si el colaborador no existe.</returns>
        Boolean DeleteColaborador(int id);
    }
}
