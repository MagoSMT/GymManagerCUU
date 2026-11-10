
using Microsoft.EntityFrameworkCore;
using GymManager.Data;

var builder = WebApplication.CreateBuilder(args);

// Configura los servicios que utiliza la app
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var connectionString = builder.Configuration.GetConnectionString("ConexionGym");

// Configura la conexión con MySQL mediante Entity Framework Core
builder.Services.AddDbContext<GymDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
);

var app = builder.Build();

// Activa el uso de sesiones
app.UseSession();

// Configura el manejo de errores y seguridad 
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// Configura la ruta principal del sistema
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Prueba la conexion con la base de datos
app.MapGet("/db-test", async (GymDbContext db) =>
{
    bool conectado = await db.Database.CanConnectAsync();

    return conectado
        ? Results.Ok("¡Conexión exitosa con GymManager!")
        : Results.Problem("No se pudo conectar con MySQL, estas todo wey para programar.");
});

app.Run();
