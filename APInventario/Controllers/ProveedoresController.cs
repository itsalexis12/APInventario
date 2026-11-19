using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APInventario.Data;
using APInventario.models;

namespace APInventario.Controllers;

[ApiController]
[Route("proveedores")]
public class ProveedoresController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProveedoresController(AppDbContext context)
    {
        _context = context;
    }

    // 1. GET /proveedores (LEER TODOS)
    [HttpGet]
    public async Task<IActionResult> GetProveedores()
    {
        var proveedores = await _context.Proveedores
            .Select(p => new
            {
                id_proveedor = p.IdProveedor,
                nombre_empresa = p.NombreEmpresa,
                telefono = p.Telefono,
                email = p.Email
            })
            .ToListAsync();

        return Ok(new { proveedores });
    }

    // 2. POST /proveedores (CREAR NUEVO PROVEEDOR)
    [HttpPost]
    public async Task<IActionResult> CrearProveedor([FromBody] ProveedorCrearDto dto)
    {
        try
        {
            var nuevoProveedor = new Proveedor
            {
                NombreEmpresa = dto.NombreEmpresa,
                Telefono = dto.Telefono,
                Email = dto.Email
            };

            _context.Proveedores.Add(nuevoProveedor);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Proveedor registrado con éxito", id = nuevoProveedor.IdProveedor });
        }
        catch (Exception ex)
        {
            return BadRequest(new { detail = ex.InnerException?.Message ?? ex.Message });
        }
    }
}

// DTO para recibir el JSON al crear proveedor
public class ProveedorCrearDto
{
    public string NombreEmpresa { get; set; } = string.Empty;
    public string? Contacto { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
}