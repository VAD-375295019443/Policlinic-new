//1
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PolyclinicWeb.Data;
using PolyclinicWebAPI.Classes;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


/*
// добавление сервисов аутентификации
builder.Services.AddAuthentication("Bearer")  // схема аутентификации - с помощью jwt-токенов
    .AddJwtBearer();      // подключение аутентификации с помощью jwt-токенов
*/


builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,
            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            ValidateLifetime = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true
        };
    });


//Настройка логирования для всего приложения.
builder.Host.UseSerilog((ctx, lc) => lc
.WriteTo.File(@"d:\Project C#\Polyclinic\Polyclinic\SerilogReport\Data.log", Serilog.Events.LogEventLevel.Information)
.WriteTo.Console(Serilog.Events.LogEventLevel.Verbose));



// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


//+1
builder.Services.AddControllers();



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer(); //Обеспечивает работу с метаданными эндпоинта (Эндпоинт = экшн).
builder.Services.AddSwaggerGen(); //Позволяет писать документацию в автоматическом режиме.
//-1


//1
var app = builder.Build();


//+1
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//-1


//1
app.UseHttpsRedirection();

//1
app.UseAuthentication();   // добавление middleware аутентификации
app.UseAuthorization();

//1
app.MapControllers();


//1
app.Run();
