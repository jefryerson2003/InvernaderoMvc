using System.Collections.Generic;

namespace ArcgisLoginApp.Models;

public class Usuario
{
    public int UsuarioId { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public ICollection<UsuarioRol> UsuarioRoles { get; set; } = null!;
}
