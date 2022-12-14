using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOcelot()
                .AddCacheManager(settings => settings.WithDictionaryHandle());


builder.Host.ConfigureLogging((hostingContext, loggingBuilder) =>
{
    loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();

}).ConfigureAppConfiguration((hostingContext, configBuilder) =>
{
    configBuilder.AddJsonFile($"ocelot.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true);
});

var app = builder.Build();

await app.UseOcelot();

//app.MapGet("/", () => "Hello World!");

app.Run();
