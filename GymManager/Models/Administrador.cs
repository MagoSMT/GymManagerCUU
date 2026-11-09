using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManager.Models
{
    [Table("administradores")]
    public class Administrador
    {
        [Key]
        [Column("id_administrador")]
        public uint IdAdministrador { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = "";

        [Column("correo")]
        public string Correo { get; set; } = "";

        [Column("password_hash")]
        public string PasswordHash { get; set; } = "";

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; }

        [Column("fecha_actualizacion")]
        public DateTime FechaActualizacion { get; set; }
    }
}
