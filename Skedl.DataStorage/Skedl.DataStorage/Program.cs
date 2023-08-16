using Microsoft.Extensions.DependencyInjection;
using Skedl.DataStorage.Services.DatabaseContexts;
using Skedl.DataStorage.Services.RabbitMqService.RabbitMqServices;

var services = new ServiceCollection();

services.AddDbContext<DatabaseSpbgu>();
services.AddSingleton<IRabbitMqService, RabbitMqService>();

services.BuildServiceProvider();

Console.ReadKey();