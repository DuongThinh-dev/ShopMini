using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.Repositories.Interfaces;
using PaymentService.Repositories.Implementations;
using PaymentService.Services.Interfaces;
using PaymentService.Services.Implementations;
using PaymentServiceImplementation = PaymentService.Services.Implementations.PaymentService;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký PaymentDbContext
builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentServiceImplementation>();

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

app.Run();
