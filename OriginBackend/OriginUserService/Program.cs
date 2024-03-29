using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OriginDatabase;
using OriginCommon;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<UserManagementDbContext>(options => options.UseInMemoryDatabase("UserDatabase"));

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OriginUserService", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OriginUserService V1"));
}

app.UseAuthorization();

app.MapControllers();

app.Run();
