using APInventario.Data;
using APInventario.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APInventario.Controllers
{
    [ApiController]
    [Route("movimientos")]
    public class MovimientosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MovimientosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: movimientos - Para consultar el historial
        [HttpGet]
        public async Task<IActionResult> ObtenerMovimientos()
        {
            var historial = await (from h in _context.HistorialInventario
                                   join p in _context.Productos on h.IdProducto equals p.IdProducto into pGroup
                                   from prod in pGroup.DefaultIfEmpty()
                                   orderby h.IdMovimiento descending
                                   select new
                                   {
                                       id_movimiento = h.IdMovimiento,
                                       fecha = h.FechaMovimiento, // Utiliza la columna fecha_movimiento
                                       producto = prod != null ? prod.Nombre : $"Producto #{h.IdProducto}",
                                       tipo_movimiento = h.TipoMovimiento,
                                       cantidad = h.Cantidad,
                                       motivo = h.Motivo ?? "-",
                                       rol_usuario = "Sistema"
                                   }).ToListAsync();

            return Ok(new { movimientos = historial });
        }
        // POST: movimientos - Para registrar una entrada o salida
        [HttpPost]
    public async Task<IActionResult> RegistrarMovimiento([FromBody] MovimientoCrearDto dto)
    {
      var tipoUpper = dto.TipoMovimiento?.ToUpper();
      var rol = dto.RolUsuario?.Trim();

      // 1. Validaciones por rol
      if (rol == "Vendedor" && tipoUpper != "SALIDA")
      {
        return BadRequest(new { mensaje = "El rol Vendedor solo tiene permitido registrar SALIDAS" });
      }

      if (rol == "Almacenista" && tipoUpper != "ENTRADA")
      {
        return BadRequest(new { mensaje = "El rol Almacenista solo tiene permitido registrar ENTRADAS" });
      }

      // El Administrador (o Admin) no entra en ningún 'if' restrictivo, 
      // por lo que puede procesar libremente tanto ENTRADA como SALIDA.

      // 2. Verificar existencia del producto y stock
      var producto = await _context.Productos.FindAsync(dto.IdProducto);
      if (producto == null)
      {
        return NotFound(new { mensaje = "Producto no encontrado" });
      }

      if (tipoUpper == "SALIDA" && producto.StockActual < dto.Cantidad)
      {
        return BadRequest(new { mensaje = $"Stock insuficiente. Disponibles: {producto.StockActual}" });
      }

      // 3. Actualizar stock del producto
      if (tipoUpper == "ENTRADA")
      {
        producto.StockActual += dto.Cantidad;
      }
      else if (tipoUpper == "SALIDA")
      {
        producto.StockActual -= dto.Cantidad;
      }

      // 4. Registrar en historial_inventario
      var nuevoHistorial = new HistorialInventario
      {
        IdProducto = dto.IdProducto,
        TipoMovimiento = tipoUpper,
        Cantidad = dto.Cantidad,
        Motivo = dto.Motivo
      };

      _context.HistorialInventario.Add(nuevoHistorial);
      await _context.SaveChangesAsync();

      return Ok(new { mensaje = "Movimiento registrado con éxito", stockActual = producto.StockActual });
    }
  }

    public class MovimientoCrearDto
    {
        public int IdProducto { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public string? Motivo { get; set; }
        public string RolUsuario { get; set; } = string.Empty;
    }
}