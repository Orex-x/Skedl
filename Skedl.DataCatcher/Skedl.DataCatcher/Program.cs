using Microsoft.Extensions.DependencyInjection;
using Skedl.DataCatcher.Services.DatabaseContexts;
using Skedl.DataCatcher.Services.HttpServices;
using Skedl.DataCatcher.Services.Quartz;
using Skedl.DataCatcher.Services.RabbitMqServices;

var services = new ServiceCollection();

services.AddDbContext<DatabaseSpbgu>();
services.AddTransient<SpbguCatcherJob>();
services.AddSingleton<IRabbitMqService, RabbitMqService>();
services.AddSingleton<IHttpService>(new HttpService("http://localhost:8000"));

var container = services.BuildServiceProvider();

// Create an instance of the job factory
var jobFactory = new DiJobFactory(container);

var quartzService = new QuartzService(jobFactory);

await quartzService.AddCatcher<SpbguCatcherJob>();

Console.ReadKey();
