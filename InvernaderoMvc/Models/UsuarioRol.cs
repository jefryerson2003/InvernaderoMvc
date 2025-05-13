namespace ArcgisLoginApp.Models;

public class UsuarioRol
{
    public int UsuarioRoleId { get; set; }
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public int RoleId { get; set; }
    public Rol Rol { get; set; } = null!;
}
