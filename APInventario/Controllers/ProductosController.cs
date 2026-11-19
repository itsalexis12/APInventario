using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APInventario.Data;
using APInventario.models;

namespace APInventario.Controllers;

[ApiController]
[Route("productos")]
public class ProductosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductosController(AppDbContext context)
    {
        _context = context;
    }

    // 1. GET /productos (LEER TODOS)
    [HttpGet]
    public async Task<IActionResult> GetProductos()
    {
        var productos = await _context.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Proveedor)
            .OrderByDescending(p => p.IdProducto)
            .Select(p => new
            {
                id_producto = p.IdProducto,
                codigo_barras = p.CodigoBarras,
                producto = p.Nombre,
                categoria = p.Categoria != null ? p.Categoria.Nombre : null,
                proveedor = p.Proveedor != null ? p.Proveedor.NombreEmpresa : null,
                stock_actual = p.StockActual,
                stock_minimo = p.StockMinimo,
                precio_venta = p.PrecioVenta
            })
            .ToListAsync();

        return Ok(new { productos });
    }

    // 2. GET /productos/alertas (ALERTAS DE STOCK)
    [HttpGet("alertas")]
    public async Task<IActionResult> GetAlertas()
    {
        var alertas = await _context.Productos
            .Where(p => p.StockActual <= p.StockMinimo)
            .Select(p => new
            {
                id_producto = p.IdProducto,
                codigo_barras = p.CodigoBarras,
                nombre = p.Nombre,
                stock_actual = p.StockActual,
                stock_minimo = p.StockMinimo
            })
            .ToListAsync();

        return Ok(new { alertas_stock = alertas });
    }

    // 3. POST /productos (GUARDAR NUEVO PRODUCTO)
    [HttpPost]
    public async Task<IActionResult> CrearProducto([FromBody] ProductoCrearDto dto)
    {
        try
        {
            var nuevoProducto = new Producto
            {
                CodigoBarras = dto.CodigoBarras,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                IdCategoria = dto.IdCategoria,
                IdProveedor = dto.IdProveedor,
                PrecioCompra = dto.PrecioCompra,
                PrecioVenta = dto.PrecioVenta,
                StockActual = dto.StockActual,
                StockMinimo = dto.StockMinimo
            };

            _context.Productos.Add(nuevoProducto);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Producto registrado con éxito", id = nuevoProducto.IdProducto });
        }
        catch (Exception ex)
        {
            return BadRequest(new { detail = ex.InnerException?.Message ?? ex.Message });
        }
    }

    // 4. DELETE /productos/{id} (ELIMINAR PRODUCTO)
    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarProducto(int id)
    {
        try
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound(new { mensaje = "El producto no existe." });
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Producto eliminado correctamente." });
        }
        catch (DbUpdateException)
        {
            // Se ejecuta si la base de datos bloquea la eliminación por llaves foráneas (movimientos o ventas asociadas)
            return BadRequest(new { mensaje = "No se puede borrar el producto porque ya tiene movimientos registrados en el inventario." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error interno al intentar eliminar", detail = ex.Message });
        }
    }
}

// DTO para recibir el JSON desde el Frontend
public class ProductoCrearDto
{
    public string CodigoBarras { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int IdCategoria { get; set; }
    public int IdProveedor { get; set; }
    public decimal PrecioCompra { get; set; }
    public decimal PrecioVenta { get; set; }
    public int StockActual { get; set; }
    public int StockMinimo { get; set; }
}