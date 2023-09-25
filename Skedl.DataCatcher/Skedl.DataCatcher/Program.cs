using Microsoft.Extensions.DependencyInjection;
using Quartz;
using RabbitMQ.Client;
using Skedl.DataCatcher.Services.DatabaseContexts;
using Skedl.DataCatcher.Services.HttpServices;
using Skedl.DataCatcher.Services.Quartz;
using Skedl.DataCatcher.Services.Quartz.Spbgu;
using Skedl.DataCatcher.Services.RabbitMqServices;
using System.Reflection.PortableExecutable;

var services = new ServiceCollection();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

services.AddDbContext<DatabaseSpbgu>();

//spbguJobs
services.AddTransient<GroupCatchJob>();
services.AddTransient<ScheduleCatchJob>();

var httpService = new HttpService("http://parser");

services.AddSingleton<IRabbitMqService, RabbitMqService>();
services.AddSingleton<IHttpService>(httpService);

var container = services.BuildServiceProvider();

// Create an instance of the job factory
var jobFactory = new DiJobFactory(container);

var quartzService = new QuartzService(jobFactory);

await quartzService.AddCatcherRepeat<GroupCatchJob>(SystemTime.UtcNow().AddMinutes(1), 4320);
await quartzService.AddCatcherRepeat<ScheduleCatchJob>(SystemTime.UtcNow().AddHours(6), 144);

Console.WriteLine("Start DataCatcher");

await Task.Delay(25000);

var responseMessage = await httpService.GetAsync("spbgu");
if (responseMessage.IsSuccessStatusCode)
{
    Console.WriteLine(await responseMessage.Content.ReadAsStringAsync());
}
else
{
    Console.WriteLine($"{responseMessage.StatusCode} : {await responseMessage.Content.ReadAsStringAsync()}");
}


while (true);

