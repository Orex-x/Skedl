using Microsoft.Extensions.DependencyInjection;
using Skedl.DataStorage.Services.DatabaseContexts;

var services = new ServiceCollection();

services.AddDbContext<DatabaseSpbgu>();

services.BuildServiceProvider();

Console.ReadKey();