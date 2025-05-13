using ArcgisLoginApp.Data;
using ArcgisLoginApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ArcgisLoginApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AccountController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Permite el acceso anónimo al formulario de login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null!)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // Permite el acceso anónimo al POST de login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = null!)
        {
            ViewData["ReturnUrl"] = returnUrl;

            // Buscar usuario y sus roles
            var user = await _db.Usuarios
                .Include(u => u.UsuarioRoles)
                  .ThenInclude(ur => ur.Rol)
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña inválidos");
                return View();
            }

            // Construir claims (nombre + roles)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("UsuarioId", user.UsuarioId.ToString())
            };
            foreach (var ur in user.UsuarioRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, ur.Rol.RoleName));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Crear la cookie de autenticación
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,                 // recuerda la sesión
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                });

            // Redirigir al origen o al Home
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Dashboard");
        }

        // Cierra la sesión (sólo usuarios autenticados pueden llegar aquí)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }


        // Página que se muestra si no tiene suficiente permiso
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
