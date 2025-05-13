namespace ArcgisLoginApp.Models;

public class Rol
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = null!;
    public ICollection<UsuarioRol> UsuarioRoles { get; set; } = null!;
}
