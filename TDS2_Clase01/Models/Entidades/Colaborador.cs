using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TDS2_Clase01.Models.Entidades
{
    /// <summary>Persona que trabaja en una empresa.</summary>
    public class Colaborador
    {
        /// <summary>Identificador único. Lo genera la base de datos al registrar.</summary>
        /// <example>1</example>
        [Key]
        [Display(Name = "Id")]
        [Required]
        public int IdColaborador { get; set; }

        /// <summary>Nombres del colaborador (máx. 50 caracteres).</summary>
        /// <example>Ana</example>
        [Display(Name = "Nombres")]
        [Required(ErrorMessage = "Debe de ingresar los nombres del colaborador")]
        [MaxLength(50, ErrorMessage = "El campo no debe tener más de 50 caracteres")]
        public string Nombres { get; set; }

        /// <summary>Apellidos del colaborador (máx. 50 caracteres).</summary>
        /// <example>Torres Ruiz</example>
        [Display(Name = "Apellidos")]
        [Required(ErrorMessage = "Debe de ingresar los apellidos del colaborador")]
        [MaxLength(50, ErrorMessage = "El campo no debe tener más de 50 caracteres")]
        public string Apellidos { get; set; }

        /// <summary>Documento Nacional de Identidad (máx. 8 caracteres).</summary>
        /// <example>12345678</example>
        [Display(Name = "DNI")]
        [Required(ErrorMessage = "Debe de ingresar el DNI del colaborador")]
        [MaxLength(8, ErrorMessage = "El campo no debe tener más de 8 caracteres")]
        public string DNI { get; set; }

        /// <summary>Sexo del colaborador (máx. 15 caracteres).</summary>
        /// <example>Femenino</example>
        [Display(Name = "Sexo")]
        [Required(ErrorMessage = "Debe de ingresar el sexo del colaborador")]
        [MaxLength(15, ErrorMessage = "El campo no debe tener más de 15 caracteres")]
        public string Sexo { get; set; }

        /// <summary>Dirección de domicilio (máx. 55 caracteres).</summary>
        /// <example>Av. Siempre Viva 742</example>
        [Display(Name = "Direccion")]
        [Required(ErrorMessage = "Debe de ingresar la direccion del colaborador")]
        [MaxLength(55, ErrorMessage = "El campo no debe tener más de 55 caracteres")]
        public string Direccion { get; set; }

        /// <summary>Id de la empresa a la que pertenece.</summary>
        /// <example>1</example>
        public int IdEmpresa { get; set; }

        [ForeignKey("IdEmpresa")]
        [JsonIgnore]
        public virtual Empresa? Empresa { get; set; }

        /// <summary>Fecha de creación. La asigna el servidor.</summary>
        [Display(Name = "Registro")]
        public DateTime FechaRegistro { get; set; }

        /// <summary>Fecha de la última modificación (nula si nunca se modificó). La asigna el servidor.</summary>
        [Display(Name = "Modificacion")]
        public DateTime? Modificacion { get; set; }
    }
}
