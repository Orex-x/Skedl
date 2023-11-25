using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Skedl.AuthService.Services;
using Skedl.AuthService.Services.CodeGeneration;
using Skedl.AuthService.Services.FileService;
using Skedl.AuthService.Services.MailService;
using Skedl.AuthService.Services.UserService;

Directory.CreateDirectory("Files/Images");

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

var connectionStringsSpbgu = configuration["ConnectionStrings:Spbgu"]!;
var asd= configuration["GmailSettings:From"]!;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(configuration);
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ICodeGenerator, CodeGenerator>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMailService, GmailService>();
builder.Services.AddDbContext<DatabaseContext>(op => op.UseNpgsql(connectionStringsSpbgu));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true, // Включаем проверку срока действия токена
            ClockSkew = TimeSpan.Zero, // Не разрешаем "небольшую погрешность" в проверке времени
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(configuration["Jwt:Key"]!)),
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

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("NgOrigins");
app.UseRouting();

app.UseAuthentication();    // аутентификация
app.UseAuthorization();     // авторизация

app.MapControllerRoute(
    name: "default",
    pattern: "Auth/{controller=Home}/{action=Index}/{id?}");

app.Run();