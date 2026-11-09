
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManager.Models
{
    // Clase para manejar la información de los socios
    [Table("socios")]
    public class Socio
    {
        [Key]
        [Column("id_socio")]
        public uint IdSocio { get; set; }

        // Datos principales del socio
        [Required]
        [StringLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        [Column("apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [StringLength(25)]
        [Column("telefono")]
        public string? Telefono { get; set; }

        [StringLength(150)]
        [Column("correo")]
        public string? Correo { get; set; }

        // Fechas relacionadas con el socio
        [Column("fecha_nacimiento", TypeName = "date")]
        public DateTime? FechaNacimiento { get; set; }

        [Column("fecha_registro", TypeName = "date")]
        public DateTime FechaRegistro { get; set; }

        // Estado actual del socio
        [Required]
        [Column("estado")]
        public string Estado { get; set; } = "Activo";

        [StringLength(500)]
        [Column("notas")]
        public string? Notas { get; set; }
    }
}
