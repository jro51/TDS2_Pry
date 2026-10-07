using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TDS2_Clase01.Models.Entidades
{
    public class Empresa
    {
        [Key]
        [Display(Name = "Id")]
        public int IdEmpresa { get; set; }

        [Display(Name = "RazonSocial")]
        [Required(ErrorMessage = "Debe de ingresar la razon social de la empresa")]
        [MaxLength(50, ErrorMessage = "El campo no debe tener más de 50 caracteres")]
        public string RazonSocial { get; set; }

        [Display(Name = "RUC")]
        [Required(ErrorMessage = "Debe de ingresar el RUC de la empresa")]
        [MaxLength(15, ErrorMessage = "El campo no debe tener más de 15 caracteres")]
        public string RUC { get; set; }

        [Display(Name = "Direccion")]
        [Required(ErrorMessage = "Debe de ingresar la direccion de la empresa")]
        [MaxLength(35, ErrorMessage = "El campo no debe tener más de 35 caracteres")]
        public string Direccion { get; set; }

        [JsonIgnore]
        public virtual ICollection<Colaborador> Colaborador { get; set; }
    }
}
