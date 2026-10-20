namespace TDS2_Clase01.Models.Dtos
{
    /// <summary>Respuesta simple con un mensaje legible para el cliente.</summary>
    /// <param name="Mensaje">Descripción del resultado de la operación.</param>
    public record MensajeRespuesta(string Mensaje);

    /// <summary>Respuesta de un registro exitoso.</summary>
    /// <param name="Mensaje">Descripción del resultado de la operación.</param>
    /// <param name="IdColaborador">Id generado para el nuevo colaborador.</param>
    public record RegistroColaboradorRespuesta(string Mensaje, int IdColaborador);
}
