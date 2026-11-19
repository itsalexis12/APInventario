using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APInventario.Data;
using APInventario.models;

namespace APInventario.Controllers;

[ApiController]
[Route("categorias")]
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriasController(AppDbContext context)
    {
        _context = context;
    }

    // 1. GET /categorias (LEER TODAS)
    [HttpGet]
    public async Task<IActionResult> GetCategorias()
    {
        var categorias = await _context.Categorias
            .Select(c => new
            {
                id_categoria = c.IdCategoria,
                nombre = c.Nombre,
                descripcion = c.Descripcion
            })
            .ToListAsync();

        return Ok(new { categorias });
    }

    // 2. POST /categorias (CREAR NUEVA CATEGORÍA)
    [HttpPost]
    public async Task<IActionResult> CrearCategoria([FromBody] CategoriaCrearDto dto)
    {
        try
        {
            var nuevaCategoria = new Categoria
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion
            };

            _context.Categorias.Add(nuevaCategoria);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Categoría creada con éxito", id = nuevaCategoria.IdCategoria });
        }
        catch (Exception ex)
        {
            return BadRequest(new { detail = ex.InnerException?.Message ?? ex.Message });
        }
    }
}

// DTO para recibir el JSON al crear categoría
public class CategoriaCrearDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}