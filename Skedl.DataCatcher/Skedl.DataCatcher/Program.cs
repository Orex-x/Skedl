﻿using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Skedl.DataCatcher.Services.DatabaseContexts;
using Skedl.DataCatcher.Services.HttpServices;
using Skedl.DataCatcher.Services.Quartz;
using Skedl.DataCatcher.Services.Quartz.Spbgu;
using Skedl.DataCatcher.Services.RabbitMqServices;

var services = new ServiceCollection();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

services.AddDbContext<DatabaseSpbgu>();

//spbguJobs
services.AddTransient<GroupCatchJob>();
services.AddTransient<ScheduleCatchJob>();


services.AddSingleton<IRabbitMqService, RabbitMqService>();
services.AddSingleton<IHttpService>(new HttpService("http://localhost:8000"));

var container = services.BuildServiceProvider();

// Create an instance of the job factory
var jobFactory = new DiJobFactory(container);

var quartzService = new QuartzService(jobFactory);

await quartzService.AddCatcher<GroupCatchJob>(SystemTime.UtcNow());
await quartzService.AddCatcher<ScheduleCatchJob>(SystemTime.UtcNow().AddHours(12));

Console.ReadKey();
