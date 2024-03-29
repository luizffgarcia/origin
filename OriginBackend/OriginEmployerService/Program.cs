using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OriginCommon;
using OriginDatabase;
using OriginEmployerService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IFileProcessor, FileProcessor>();

// Add services to the container.
builder.Services.AddDbContext<UserManagementDbContext>(options => options.UseInMemoryDatabase("EmployerDatabase"));

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OriginEmployerService", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OriginEmployerService V1"));
}

app.UseAuthorization();

app.MapControllers();

app.Run();
