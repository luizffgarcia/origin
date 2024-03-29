using Microsoft.OpenApi.Models;
using OriginCommon;
using OriginUserAccessManagementService.Clients;
using OriginUserAccessManagementService.HttpClients;
using OriginUserAccessManagementService.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<IUserServiceHttpClient, UserServiceHttpClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["UserServiceBaseUrl"]);
});

builder.Services.AddHttpClient<IEmployerServiceHttpClient, EmployerServiceHttpClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["EmployerServiceBaseUrl"]);
});

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OriginUserAccessManagementService", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OriginUserAccessManagementService V1"));
}

app.UseAuthorization();

app.MapControllers();

app.Run();
