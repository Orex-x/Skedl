
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using Skedl.Api.Services.UserService;
using Skedl.DataCatcher.Services.DatabaseContexts;
using Skedl.DataCatcher.Services.HttpServices;
using Skedl.DataCatcher.Services.Quartz;
using Skedl.DataCatcher.Services.Quartz.Jobs.Spbgu;
using Skedl.DataCatcher.Services.Quartz.Spbgu;
using Skedl.DataCatcher.Services.RabbitMqServices;
using System.Text;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<SpbguGroupCatchJob>();
builder.Services.AddTransient<SpbguScheduleCatchJob>();
var httpService = new HttpService(configuration["HttpService:BaseUrl"]!);

builder.Services.AddSingleton<IRabbitMqService>(
    new RabbitMqService(configuration["Rabbitmq:HostName"]!, configuration["Rabbitmq:UserName"]!, configuration["Rabbitmq:Password"]!));

builder.Services.AddSingleton<IHttpService>(httpService);
builder.Services.AddDbContext<DatabaseSpbgu>(op => op.UseNpgsql(configuration["ConnectionStrings:Spbgu"]!));

var container = builder.Services.BuildServiceProvider();
// Create an instance of the job factory
var jobFactory = new DiJobFactory(container);
var quartzService = new QuartzService(jobFactory);



await quartzService.AddCatcherRepeat<SpbguGroupCatchJob>(Convert.ToInt32(configuration["Quartz:SpbguGroupCatchJob:IntervalInHours"]!));
await quartzService.AddCatcherRepeatWithCron<SpbguScheduleCatchJob>(configuration["Quartz:SpbguScheduleCatchJob:CronSchedule"]!);
await quartzService.AddCatcherRepeatWithCron<SpbguScheduleDeleteJob>(configuration["Quartz:SpbguScheduleDeleteJob:CronSchedule"]!);

builder.Services.AddSingleton(quartzService);

builder.Services.AddMvc(setupAction => {
    setupAction.EnableEndpointRouting = false;})
    .AddJsonOptions(jsonOptions => {
        jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;})
    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);



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

app.UseCors("NgOrigins");
app.UseRouting();

app.UseAuthentication();    // аутентификация
app.UseAuthorization();     // авторизация

app.MapControllerRoute(
    name: "default",
    pattern: "DataCatcher/{controller=Home}/{action=Index}/{id?}");

app.Run();
