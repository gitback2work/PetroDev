using AgroService;
using Serilog;



IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureServices(services =>
    {
        services.AddSingleton<ICalcService, CalcService>();
        services.AddHostedService<TimedWorker>();
    })
    .UseSerilog((context, config) =>
    {
        config.WriteTo.Console();
        config.WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day);
    })
    .Build();


await host.RunAsync();
