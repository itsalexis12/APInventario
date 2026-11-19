using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APInventario.Data;

namespace APInventario.Controllers;

[ApiController]
[Route("auth")]
public class LoginController : ControllerBase
{
    private readonly AppDbContext _context;

    public LoginController(AppDbContext context)
    {
        _context = context;
    }

    // POST /auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        // Busca por username o por email
        var usuario = await _context.UsuariosLogin
            .FirstOrDefaultAsync(u => u.Username == dto.Username || u.Email == dto.Username);

        if (usuario == null || usuario.PasswordHash != dto.Password)
        {
            return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos" });
        }

        return Ok(new
        {
            mensaje = "Inicio de sesión exitoso",
            usuario = new
            {
                id = usuario.IdUsuario,
                username = usuario.Username,
                email = usuario.Email,
                rol = usuario.Rol
            }
        });
    }
}

public class LoginDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}