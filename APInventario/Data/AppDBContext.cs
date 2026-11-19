using APInventario.models;
using Microsoft.EntityFrameworkCore;

namespace APInventario.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Producto> Productos { get; set; }
    public DbSet<HistorialInventario> HistorialInventario { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Proveedor> Proveedores { get; set; }
    public DbSet<UsuarioLogin> UsuariosLogin { get; set; }
}