using data_loading.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// CORS yapılandırmasını ekle
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("https://localhost:7146")  // Frontend'inizin çalıştığı URL
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// JSON serileştirme ayarlarını yapılandırma
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });


// Add services to the container.

builder.Services.AddControllers();

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add SQLite database context with lazy loading
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")).UseLazyLoadingProxies());


var app = builder.Build();

// CORS middleware'i ekleyin
app.UseCors("AllowSpecificOrigin");

// Configure the HTTP request pipeline.
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Swagger'ı ekleyin
    app.UseSwaggerUI(); // Swagger UI'yi aktif hale getirin
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
