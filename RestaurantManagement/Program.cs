using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantManagement.Configurations;
using RestaurantManagement.Data;
using RestaurantManagement.Middleware;
using RestaurantManagement.Models;
using RestaurantManagement.Repository;
using RestaurantManagement.Services;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers();

//Add Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateAudience = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
//Data protection
builder.Services.AddDataProtection();
//Implementing DbContext
builder.Services.AddDbContext<ApplicationDBContex>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


//Mapper Implementation
builder.Services.AddAutoMapper(typeof(MapperConfig));

//Scopes
builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IMenuManager, MenuManager>();
builder.Services.AddScoped<DurationLoggerFilter>();

//Implementing identity
builder.Services.AddIdentityCore<Customer>()
    .AddRoles<RestaurantRoles>()
    .AddTokenProvider<DataProtectorTokenProvider<Customer>>("RestaurantManagementApi")
    .AddEntityFrameworkStores<ApplicationDBContex>()
    .AddDefaultTokenProviders();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//authentication
app.UseAuthentication();
app.UseAuthorization();



app.UseHttpsRedirection();


app.MapControllers();

app.Run();