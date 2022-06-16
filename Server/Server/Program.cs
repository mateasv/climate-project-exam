using Microsoft.EntityFrameworkCore;
using Server.Extensions;
using Server.Hubs;
using Server.Models;

// https://code-maze.com/ten-things-avoid-aspnetcore-controllers/ best practices for controllers
// Scaffold-DbContext "Server=DESKTOP-4AN5991;Database=TreeDB;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -force

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR().AddMessagePackProtocol();
builder.Services.AddDbContext<TreeDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddAutoMapper(typeof(Program)); // https://code-maze.com/automapper-net-core/
builder.Services.AddControllers().AddControllersAsServices();

builder.Services.AddCors(opts => opts.AddDefaultPolicy(builder =>
{
    builder.AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod();
}));

var app = builder.Build();

app.ConfigureCustomExceptionMiddleware(); // https://code-maze.com/global-error-handling-aspnetcore/

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapRazorPages();
app.MapHub<TreeHub>("/TreeHub");

app.Run();
