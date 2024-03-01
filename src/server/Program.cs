using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XDChatServer.Services;
using XDChatServer;

IHostBuilder builder = Host.CreateDefaultBuilder();

IHost host = builder.ConfigureServices(
    services => {
        services.AddSingleton<IChatServerApp, ChatServerApp>();
        services.AddSingleton<IServerConfigurationService, ServerConfigurationServiceDefault>();
    }).Build();

var app = host.Services.GetRequiredService<IChatServerApp>();

app.Run();