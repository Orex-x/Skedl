
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Skedl.Api.Services.UserService;
using Skedl.DataCatcher.Services.DatabaseContexts;
using Skedl.DataCatcher.Services.HttpServices;
using Skedl.DataCatcher.Services.Quartz;
using Skedl.DataCatcher.Services.Quartz.Jobs.Spbgu;
using Skedl.DataCatcher.Services.Quartz.Spbgu;
using Skedl.DataCatcher.Services.RabbitMqServices;
using Skedl.DataCatcher.Services.Spbgu;
using System.Text;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var httpServiceBaseUrl = configuration["HttpService:BaseUrl"]!;
var rabbitmqHostName = configuration["Rabbitmq:HostName"]!;
var rabbitmqUserName = configuration["Rabbitmq:UserName"]!;
var rabbitmqPassword = configuration["Rabbitmq:Password"]!;
var connectionStringsSpbgu = configuration["ConnectionStrings:Spbgu"]!;
var spbguGroupCatchJobIntervalInHours = Convert.ToInt32(configuration["Quartz:SpbguGroupCatchJob:IntervalInHours"]!); 
var spbguScheduleCatchJobCronSchedule = configuration["Quartz:SpbguScheduleCatchJob:CronSchedule"]!; 
var spbguScheduleDeleteJobCronSchedule = configuration["Quartz:SpbguScheduleDeleteJob:CronSchedule"]!; 
var jwtKey = configuration["Jwt:Key"]!; 


AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<SpbguGroupCatchJob>();
builder.Services.AddTransient<SpbguScheduleCatchJob>();
var httpService = new HttpService(httpServiceBaseUrl);

builder.Services.AddSingleton<IRabbitMqService>(
    new RabbitMqService(rabbitmqHostName, rabbitmqUserName, rabbitmqPassword));

builder.Services.AddSingleton<IHttpService>(httpService);

builder.Services.AddDbContext<DatabaseSpbgu>(op => op.UseNpgsql(connectionStringsSpbgu));


builder.Services.AddScoped<ISpbguGroupCatch, SpbguGroupCatch>();
builder.Services.AddScoped<ISpbguScheduleDelete, SpbguScheduleDelete>();
builder.Services.AddScoped<ISpbguScheduleCatch, SpbguScheduleCatch>();

var container = builder.Services.BuildServiceProvider();
var jobFactory = new DiJobFactory(container);
var quartzService = new QuartzService(jobFactory);

await quartzService.AddCatcherRepeat<SpbguGroupCatchJob>(spbguGroupCatchJobIntervalInHours);
await quartzService.AddCatcherRepeatWithCron<SpbguScheduleCatchJob>(spbguScheduleCatchJobCronSchedule);
await quartzService.AddCatcherRepeatWithCron<SpbguScheduleDeleteJob>(spbguScheduleDeleteJobCronSchedule);

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
                .GetBytes(jwtKey)),
            ValidateIssuer = false,
            ValidateAudience = false,

        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
