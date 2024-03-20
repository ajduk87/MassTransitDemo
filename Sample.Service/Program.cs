using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.ExtensionsDependencyInjectionIntegration.MultiBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sample.Components.Consumers;
using Sample.Contracts;
using static MassTransit.Logging.LogCategoryName;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = new HostBuilder()
            .ConfigureAppConfiguration((hostingContext, config) => 
            {
                config.AddJsonFile("appsettings.json", true);
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((hostContext, services) => 
            {
                services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);

                services.AddMassTransit(cfg => 
                {
                    cfg.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>();

                    Console.WriteLine("SubmitOrderConsumer is added.");

                    cfg.UsingRabbitMq((context, busConfigurator) =>
                    {
                        busConfigurator.Host(new Uri("rabbitmq://localhost/"), h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        });

                        // Configure endpoints
                        busConfigurator.ConfigureEndpoints(context);
                    });


                    //cfg.AddBus(provider => Bus.Factory.CreateUsingRabbitMq());
                    cfg.AddRequestClient<SubmitOrder>(
                        new Uri($"exchange:{KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>()}"));
                });
            })
            .ConfigureLogging((hostingContext, logging) => 
            {
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                logging.AddConsole();
            });
        await builder.RunConsoleAsync();
    }



}