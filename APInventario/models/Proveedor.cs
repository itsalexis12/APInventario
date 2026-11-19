using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APInventario.models;

[Table("proveedores")]
public class Proveedor
{
    [Key]
    [Column("id_proveedor")]
    public int IdProveedor { get; set; }

    [Required]
    [StringLength(150)]
    [Column("nombre_empresa")]
    public string NombreEmpresa { get; set; } = string.Empty;

    [Column("contacto_nombre")]
    public string? ContactoNombre { get; set; }

    [Column("telefono")]
    public string? Telefono { get; set; }

    [Column("email")]
    public string? Email { get; set; }

    [Column("direccion")]
    public string? Direccion { get; set; }
}