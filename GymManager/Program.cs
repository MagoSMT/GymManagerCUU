using Microsoft.EntityFrameworkCore;
using GymManager.Data;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
var connectionString = builder.Configuration.GetConnectionString("ConexionGym");

builder.Services.AddDbContext<GymDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
);

var app = builder.Build();
    app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapGet("/db-test", async (GymDbContext db) =>
{
    bool conectado = await db.Database.CanConnectAsync();

    return conectado
        ? Results.Ok("¡Conexión exitosa con GymManager!")
        : Results.Problem("No se pudo conectar con MySQL, estas todo wey para programar.");
});

app.Run();
