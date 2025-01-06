using Microsoft.EntityFrameworkCore;
using TodoBackend.Data;
using TodoBackend.Services;
using System.Text.Json.Serialization;
using TodoBackend.Repositories;

namespace TodoBackend;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });
            
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add DB Context
        builder.Services.AddDbContext<TodoDbContext>(options =>
            options.UseSqlite("Data Source=todos.db"));

        // Add Services
        builder.Services.AddScoped<ITodoService, TodoService>();
        builder.Services.AddScoped<ITaskAnalyzer, TaskAnalyzer>();
        builder.Services.AddScoped<ITodoRepository, TodoRepository>();

        // Add CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAngularApp",
                builder =>
                {
                    builder.WithOrigins("http://localhost:4200")
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowAngularApp");
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
