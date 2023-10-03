using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApplication1.Helpers;
using WebApplication1.Services;


var builder = WebApplication.CreateBuilder(args);
IServiceCollection allServices = builder.Services;

string connection = builder.Configuration.GetConnectionString("WebApiDatabase");

// Add services to the container.
allServices.AddControllers();
allServices.AddDbContext<DataContext>(options =>
    options.UseNpgsql(connection));

// builder.Configuration.GetValue<string>("JWT:AccessSecretKey")
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
allServices.AddEndpointsApiExplorer();
allServices.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        option.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        });
    }
);
var lol = builder.Configuration.GetValue<string>("JWT:AccessSecretKey");

allServices.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JWT:AccessSecretKey"))),
            ValidateIssuerSigningKey = true
        };
    });

allServices.AddSingleton<TokenService>();
allServices.Configure<JwtSetting>(builder.Configuration.GetSection("JWT"));
allServices.AddCors();
allServices.AddTransient<UserService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder => builder.AllowAnyOrigin());
app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();
app.MapGet("/login", async (HttpContext context) =>
{
    var claimsIdentity = new ClaimsIdentity("Undefined");
    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
    // установка аутентификационных куки
    await context.SignInAsync(claimsPrincipal);
    return Results.Redirect("/");
});

app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return "Данные удалены";
});
app.Map("/", (HttpContext context) =>
{
    var user = context.User.Identity;
    if (user is not null && user.IsAuthenticated)
    {
        return $"Пользователь аутентифицирован. Тип аутентификации: {user.AuthenticationType}";
    }
    else
    {
        return "Пользователь НЕ аутентифицирован";
    }
});
app.Run();