using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces;
using Shop.Domain.Interfaces;
using Shop.Persistence.Context;
using Shop.Persistence.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// تنظیم DbContext
builder.Services.AddDbContext<ShopDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// تنظیم UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddControllers();

// تنظیمات DI(depency injection)
builder.Services.AddScoped<ICategoryService, Shop.Application.Services.CategoryService>();
builder.Services.AddScoped<IProductService, Shop.Application.Services.ProductService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
