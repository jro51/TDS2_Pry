using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TDS2_Clase01.Models.Entidades
{
    public class Colaborador
    {
        [Key]
        [Display(Name = "Id")]
        [Required]
        public int IdColaborador { get; set; }

        [Display(Name = "Nombres")]
        [Required(ErrorMessage = "Debe de ingresar los nombres del colaborador")]
        [MaxLength(50, ErrorMessage = "El campo no debe tener más de 50 caracteres")]
        public string Nombres { get; set; }

        [Display(Name = "Apellidos")]
        [Required(ErrorMessage = "Debe de ingresar los apellidos del colaborador")]
        [MaxLength(50, ErrorMessage = "El campo no debe tener más de 50 caracteres")]
        public string Apellidos { get; set; }

        [Display(Name = "DNI")]
        [Required(ErrorMessage = "Debe de ingresar el DNI del colaborador")]
        [MaxLength(8, ErrorMessage = "El campo no debe tener más de 8 caracteres")]
        public string DNI { get; set; }

        [Display(Name = "Sexo")]
        [Required(ErrorMessage = "Debe de ingresar el sexo del colaborador")]
        [MaxLength(15, ErrorMessage = "El campo no debe tener más de 15 caracteres")]
        public string Sexo { get; set; }

        [Display(Name = "Direccion")]
        [Required(ErrorMessage = "Debe de ingresar la direccion del colaborador")]
        [MaxLength(55, ErrorMessage = "El campo no debe tener más de 55 caracteres")]
        public string Direccion { get; set; }

        public int IdEmpresa { get; set; }

        [ForeignKey("IdEmpresa")]
        [JsonIgnore]
        public virtual Empresa? Empresa { get; set; }

        [Display(Name = "Registro")]
        public DateTime FechaRegistro { get; set; }

        [Display(Name = "Modificacion")]
        public DateTime? Modificacion { get; set; }
    }
}
