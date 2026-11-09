
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManager.Models
{
    [Table("pagos")]
    public class Pago
    {
        [Key]
        [Column("id_pago")]
        public uint IdPago { get; set; }

        [Column("id_suscripcion")]
        public uint IdSuscripcion { get; set; }

        [Column("monto", TypeName = "decimal(10,2)")]
        public decimal Monto { get; set; }

        [Column("fecha_pago")]
        public DateTime FechaPago { get; set; }

        [Required]
        [Column("metodo")]
        public string Metodo { get; set; } = "Efectivo";

        [Required]
        [Column("estado")]
        public string Estado { get; set; } = "Registrado";

        [StringLength(100)]
        [Column("referencia")]
        public string? Referencia { get; set; }

        [StringLength(500)]
        [Column("notas")]
        public string? Notas { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; }

        // hace la relacion con la suscripción
        [ForeignKey("IdSuscripcion")]
        public Suscripcion? Suscripcion { get; set; }
    }
}
