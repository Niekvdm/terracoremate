using Serilog;
using TerracoreMate.Extensions;
using TerracoreMate.HostedServices;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddSerilog(Log.Logger);

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddTerracoreMate();

builder.Services.AddHostedService<MainService>();

var app = builder.Build();

app.Run();