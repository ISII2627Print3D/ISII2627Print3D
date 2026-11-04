namespace AppForSEII.API.Data {
    public class SeedData {
        public static void Initialize(ApplicationDbContext dbContext, IServiceProvider serviceProvider, ILogger logger) {
            List<string> rolesNames = new List<string> { "Administrator", "Employee", "Customer" };

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            try {
                SeedRoles(roleManager, rolesNames);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the roles in the Database.");
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            try {
                SeedUsers(userManager, rolesNames);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Users in the Database.");
            }
            try {
                SeedImpresoras(dbContext);
            }
            catch (Exception ex) {
                logger.LogError(ex, "Error al insertar los datos iniciales de impresoras.");
            }
        }

        public static void SeedImpresoras(ApplicationDbContext dbContext)
        {
            var impresoras = new List<Impresora3D>
            {
                new Impresora3D
                {
                    Nombre = "Impresora Filamento 01",
                    Modelo = "Modelo F1",
                    Tipo = TipoImpresora.Filamento,
                    Descripcion = "Impresora de filamento para piezas y prototipos.",
                    PrecioKilovatioHora = 0.1500m,
                    PrecioReserva = 3.00m
                },
                new Impresora3D
                {
                    Nombre = "Impresora Resina 01",
                    Modelo = "Modelo R1",
                    Tipo = TipoImpresora.Resina,
                    Descripcion = "Impresora de resina para modelos de detalle.",
                    PrecioKilovatioHora = 0.1800m,
                    PrecioReserva = 5.00m
                },
                new Impresora3D
                {
                    Nombre = "Impresora Polimero 01",
                    Modelo = "Modelo P1",
                    Tipo = TipoImpresora.Polimero,
                    Descripcion = "Impresora de polímero para piezas personalizadas.",
                    PrecioKilovatioHora = 0.2000m,
                    PrecioReserva = 7.00m
                }
            };

            foreach (var impresora in impresoras)
            {
                bool existe = dbContext.Impresoras3D.Any(
                    i => i.Nombre == impresora.Nombre);

                if (!existe)
                {
                    dbContext.Impresoras3D.Add(impresora);
                }
            }

            dbContext.SaveChanges();
        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager, List<string> roles) {

            foreach (string roleName in roles) {
                //it checks such role does not exist in the database 
                if (!roleManager.RoleExistsAsync(roleName).Result) {
                    IdentityRole role = new IdentityRole();
                    role.Name = roleName;
                    role.NormalizedName = roleName;
                    IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                }
            }

        }

        public static void SeedUsers(UserManager<ApplicationUser> userManager, List<string> roles) {
            //first, it checks the user does not already exist in the DB
            if (userManager.FindByNameAsync("elena@uclm.es").Result == null) {
                ApplicationUser user = new ApplicationUser("1", "Elena", "Navarro Martínez", "elena@uclm.es");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "Password1234%");
                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    //administrator role
                    userManager.AddToRoleAsync(user, roles[0]).Wait();
                }
            }


            if (userManager.FindByNameAsync("peter@uclm.es").Result == null) {
                //A customer class has been defined because it has different attributes (purchase, rental, etc.)
                ApplicationUser user = new ApplicationUser("3", "Peter", "Jackson", "peter@uclm.es");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "OtherPass12$");

                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    //customer role
                    userManager.AddToRoleAsync(user, roles[2]).Wait();

                }
            }

        }





    }
}
