using Microsoft.EntityFrameworkCore;
using UserService.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// app.MapGet("/api/db-test", async (UserDbContext db) =>
// {
//     bool connected = await db.Database.CanConnectAsync();

//     if (connected)
//     {
//         return Results.Ok("Ket noi UserDB thanh cong!");
//     }

//     return Results.Problem("Khong the ket noi UserDB");
// });

app.Run();
