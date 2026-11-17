using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MonitorSerial.Models;

namespace MonitorSerial
{
    public class RoleConfig
    {
        public static void RegisterRoles()
        {
            using (var context = new ApplicationDbContext())
            {
                var roleManager = new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(context)
                );

                string[] roles = { "Administrador", "Usuario" };

                foreach (string role in roles)
                {
                    if (!roleManager.RoleExists(role))
                    {
                        roleManager.Create(new IdentityRole(role));
                    }
                }

                var userManager = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(context)
                );

                var adminRole = roleManager.FindByName("Administrador");

                if (!adminRole.Users.Any())
                {
                    var primerUsuario = context.Users.FirstOrDefault();

                    if (primerUsuario != null)
                    {
                        userManager.AddToRole(
                            primerUsuario.Id,
                            "Administrador"
                        );
                    }
                }
            }
        }
    }
}