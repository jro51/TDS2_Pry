
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using TDS2_Clase01.Data;
using TDS2_Clase01.Data.AccesoDatos;
using TDS2_Clase01.Data.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization(options => options.AddPolicy("AllowLayoutJefe", policy => policy.RequireRole("Jefe")));

builder.Services.AddAuthorization(options => options.AddPolicy("AllowLayoutAsistente", policy => policy.RequireRole("Asistente")));

builder.Services.AddAuthorization(options => options.AddPolicy("AllowLayoutJefeAsistente", policy => policy.RequireRole("Jefe", "Asistente")));

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IDAColaborador, DAColaborador>();
builder.Services.AddSingleton<IDAEmpresa, DAEmpresa>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title="Colaborador API",
        Version="v1"
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy.WithOrigins(
            "http://localhost:4200"
            )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:4200");
    context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization");
    context.Response.Headers.Append("Access-Control-Allow-Origin", "GET, POST, PUT, DELETE, OPTIONS");
    if (context.Request.Method=="OPTIONS")
    {
        context.Response.StatusCode = 200;
        return;
    }
    await next();
});

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
