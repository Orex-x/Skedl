using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Skedl.Api.Services.Databases;
using Skedl.Api.Services.UserService;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var connectionStringsSpbgu = configuration["ConnectionStrings:Spbgu"]!;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddDbContext<DatabaseSpbgu>(op => op.UseNpgsql(connectionStringsSpbgu));

builder.Services.AddMvc(setupAction=> {
        setupAction.EnableEndpointRouting = false;
    }).AddJsonOptions(jsonOptions =>
    {
        jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
    }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true, // Включаем проверку срока действия токена
            ClockSkew = TimeSpan.Zero, // Не разрешаем "небольшую погрешность" в проверке времени
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
            ValidateIssuer = false,
            ValidateAudience = false,

        };
    });
    
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("NgOrigins");
app.UseRouting();

app.UseAuthentication();    // аутентификация
app.UseAuthorization();     // авторизация

app.MapControllerRoute(
    name: "default",
    pattern: "Api/{controller=Home}/{action=Index}/{id?}");

app.Run();