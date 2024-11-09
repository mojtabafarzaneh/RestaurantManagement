using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
//Data protection
builder.Services.AddDataProtection();
//Implementing DbContext
builder.Services.AddDbContext<ApplicationDBContex>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Implementing identity
builder.Services.AddIdentityCore<Customer>()
    .AddRoles<RestaurantRoles>()
    .AddEntityFrameworkStores<ApplicationDBContex>()
    .AddDefaultTokenProviders();


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