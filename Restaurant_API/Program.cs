using Core.Services.AccountService;
using Core.Services.MenuS;
using Core.Services.OutletSer;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IOutletService, OutletService>();
builder.Services.AddScoped<IMenuService, MenuService>();
// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMyOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

<<<<<<< HEAD
builder.Configuration.AddJsonFile("Restaurant_API_appsettings.json", optional: true, reloadOnChange: true);
=======
builder.Configuration.AddJsonFile("Restaurent_API_appsettings.json", optional: true, reloadOnChange: true);
>>>>>>> c513c9b1fa518970f994ead4c50374b2b1b49d0c

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowMyOrigins");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
