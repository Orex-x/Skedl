using System.Text;
using MassTransit;
using MassTransit.MultiBus;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Skedl.DataStorage.Services;
using Skedl.DataStorage.Services.Data;
using Skedl.DataStorage.Services.Data.DataGroup;
using Skedl.DataStorage.Services.RabbitMq;
using Skedl.DataStorage.Services.UserService;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();



AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(mt => mt.AddMassTransit(x =>
{
  x.UsingRabbitMq((ctx, cfg) =>
  {
      cfg.Host("192.168.0.103");
  });
}));

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IDataCatcher, DataCatcher>();
builder.Services.AddDbContext<DatabaseContext>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

new Huiznaetkaktebyanazvazz().Start();

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


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();    // аутентификация
app.UseAuthorization();     // авторизация

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();