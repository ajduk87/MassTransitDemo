using MassTransit;
using Rabbit.Details;
using Sample.Components.Consumers;
using Sample.Contracts;
using System.Windows.Markup;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("RabbirMQ Details");

        var rabbitMqFactory = new RabbitMqFactory("localhost", "guest", "guest");

        var busControl = rabbitMqFactory.Create<SubmitOrderConsumer>("order-service");


        var endpointConsumerMap = new Dictionary<string, Type>
        {
            { "submit-order-endpoint", typeof(SubmitOrderConsumer) },
            { "submit-order-discount-endpoint", typeof(SubmitOrderWithDiscountConsumer) }
        };


        var busControlMultiple = rabbitMqFactory.Create(endpointConsumerMap);


        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(30));

       

        try
        {
            //await busControl.StartAsync();
            await busControlMultiple.StartAsync();
            Console.WriteLine("Buses started successfully.");

            //var endpoint = await busControl.GetSendEndpoint(new Uri("exchange:order-service"));

            //await endpoint.Send<SubmitOrder>(new
            //{
            //    OrderId = Guid.NewGuid(),
            //    Timestamp = DateTime.Now,
            //    CustomerNumber = "1234"
            //});

            //await busControl.Publish<SubmitOrder>(new
            //{
            //    OrderId = Guid.NewGuid(),
            //    Timestamp = DateTime.Now,
            //    CustomerNumber = "1234"
            //});

            await busControlMultiple.Publish<SubmitOrder>(new
            {
                OrderId = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                CustomerNumber = "9999",
                MoneyInDollars = 100
            });

            await Task.Run(Console.ReadLine);
        }
        finally
        {

            await busControl.StopAsync(CancellationToken.None);
            await busControlMultiple.StopAsync(CancellationToken.None);
            Console.WriteLine("Buses stopped successfully.");
        }
    }
}