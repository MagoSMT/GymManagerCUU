
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManager.Models
{
    // Clase para manejar las suscripciones
    [Table("suscripciones")]
    public class Suscripcion
    {
        [Key]
        [Column("id_suscripcion")]
        public uint IdSuscripcion { get; set; }

        // Datos del socio y del plan
        [Column("id_socio")]
        public uint IdSocio { get; set; }

        [Column("id_plan")]
        public uint IdPlan { get; set; }

        // Fechas de la suscripcion
        [Column("fecha_inicio", TypeName = "date")]
        public DateTime FechaInicio { get; set; }

        [Column("fecha_fin", TypeName = "date")]
        public DateTime FechaFin { get; set; }

        [Column("precio_contratado", TypeName = "decimal(10,2)")]
        public decimal PrecioContratado { get; set; }

        // Estado de la suscripcion
        [Required]
        [Column("estado")]
        public string Estado { get; set; } = "Pendiente";

        [StringLength(500)]
        [Column("observaciones")]
        public string? Observaciones { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; }

        [Column("fecha_actualizacion")]
        public DateTime FechaActualizacion { get; set; }

        // Relaciones con el socio y el plan ;)
        [ForeignKey("IdSocio")]
        public Socio? Socio { get; set; }

        [ForeignKey("IdPlan")]
        public Plan? Plan { get; set; }
    }
}
