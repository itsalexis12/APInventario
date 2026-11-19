using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace APInventario.models;
[Table("productos")]
public class Producto
{
    [Key]
    [Column("id_producto")]
    public int IdProducto { get; set; }

    [Column("codigo_barras")]
    public string? CodigoBarras { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(150)]
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Column("descripcion")]
    public string? Descripcion { get; set; }

    [Column("id_categoria")]
    public int? IdCategoria { get; set; }

    [Column("id_proveedor")]
    public int? IdProveedor { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El precio de compra no puede ser negativo")]
    [Column("precio_compra")]
    public decimal PrecioCompra { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El precio de venta no puede ser negativo")]
    [Column("precio_venta")]
    public decimal PrecioVenta { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    [Column("stock_actual")]
    public int StockActual { get; set; }

    [Column("stock_minimo")]
    public int StockMinimo { get; set; } = 5;

    [Column("fecha_creacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // Relaciones de navegación
    [ForeignKey("IdCategoria")]
    public Categoria? Categoria { get; set; }

    [ForeignKey("IdProveedor")]
    public Proveedor? Proveedor { get; set; }
}
