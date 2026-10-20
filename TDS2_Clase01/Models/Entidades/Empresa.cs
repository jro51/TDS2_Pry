using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TDS2_Clase01.Models.Entidades
{
    /// <summary>Empresa a la que pertenecen los colaboradores.</summary>
    public class Empresa
    {
        /// <summary>Identificador único de la empresa.</summary>
        /// <example>1</example>
        [Key]
        [Display(Name = "Id")]
        public int IdEmpresa { get; set; }

        /// <summary>Razón social (máx. 50 caracteres).</summary>
        /// <example>Industrias JF S.A.C.</example>
        [Display(Name = "RazonSocial")]
        [Required(ErrorMessage = "Debe de ingresar la razon social de la empresa")]
        [MaxLength(50, ErrorMessage = "El campo no debe tener más de 50 caracteres")]
        public string RazonSocial { get; set; }

        /// <summary>RUC de la empresa (máx. 15 caracteres).</summary>
        /// <example>20123456789</example>
        [Display(Name = "RUC")]
        [Required(ErrorMessage = "Debe de ingresar el RUC de la empresa")]
        [MaxLength(15, ErrorMessage = "El campo no debe tener más de 15 caracteres")]
        public string RUC { get; set; }

        /// <summary>Dirección fiscal (máx. 35 caracteres).</summary>
        /// <example>Jr. Los Olivos 123</example>
        [Display(Name = "Direccion")]
        [Required(ErrorMessage = "Debe de ingresar la direccion de la empresa")]
        [MaxLength(35, ErrorMessage = "El campo no debe tener más de 35 caracteres")]
        public string Direccion { get; set; }

        [JsonIgnore]
        public virtual ICollection<Colaborador> Colaborador { get; set; }
    }
}
