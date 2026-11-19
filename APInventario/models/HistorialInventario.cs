using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APInventario.models
{
    [Table("historial_inventario")]
    public class HistorialInventario
    {
        [Key]
        [Column("id_movimiento")]
        public int IdMovimiento { get; set; }

        [Column("id_producto")]
        public int IdProducto { get; set; }

        [Required]
        [Column("tipo_movimiento")]
        public string TipoMovimiento { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("fecha_movimiento")]
        public DateTime FechaMovimiento { get; set; }

        [Column("motivo")]
        public string? Motivo { get; set; }
    }
}