using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using TDS2_Clase01.Data;
using TDS2_Clase01.Data.AccesoDatos;
using TDS2_Clase01.Data.Interface;

var builder = WebApplication.CreateBuilder(args);

// La cadena de conexión NO va en el código: se lee de appsettings, user-secrets
// (dotnet user-secrets set "ConnectionStrings:DefaultConnection" "...") o variables de entorno.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AllowLayoutJefe", policy => policy.RequireRole("Jefe"));
    options.AddPolicy("AllowLayoutAsistente", policy => policy.RequireRole("Asistente"));
    options.AddPolicy("AllowLayoutJefeAsistente", policy => policy.RequireRole("Jefe", "Asistente"));
});

builder.Services.AddControllersWithViews();

// Scoped: comparten el DbContext (que también es scoped) durante una misma petición.
builder.Services.AddScoped<IDAColaborador, DAColaborador>();
builder.Services.AddScoped<IDAEmpresa, DAEmpresa>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Colaboradores API",
        Version = "v1",
        Description = "API REST para administrar colaboradores y consultar empresas. " +
                      "Proyecto académico del curso TDS2 (ASP.NET Core, EF Core, SQL Server)."
    });

    // Usa los comentarios XML (///) de controladores y modelos como documentación.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Colaboradores API v1");
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AngularPolicy");
app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllers();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();

// Necesario para que los tests de integración (WebApplicationFactory<Program>) vean la clase Program.
public partial class Program { }
