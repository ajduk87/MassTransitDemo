using MassTransit;
using System;

namespace Rabbit.Details
{
    public class RabbitMqFactory
    {
        private string _server;
        private string _username;
        private string _password;

        public RabbitMqFactory(string server, string username, string password)
        {
            _server = server;
            _username = username;
            _password = password;
        }

        public IBusControl Create<TConsumer>(string endpointName) where TConsumer : class, IConsumer, new()
        {
            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(_server, h =>
                {
                    h.Username(_username);
                    h.Password(_password);
                });


                cfg.ReceiveEndpoint(endpointName, e =>
                {
                    e.Lazy = true;
                    e.PrefetchCount = 20;
                    e.Consumer(() => new TConsumer());
                });

            });
        }


        public IBusControl Create(Dictionary<string, Type> endpointConsumerMap)
        {
            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(_server, h =>
                {
                    h.Username(_username);
                    h.Password(_password);
                });

                foreach (var kvp in endpointConsumerMap)
                {
                    var endpointName = kvp.Key;
                    var consumerType = kvp.Value;

                    cfg.ReceiveEndpoint(endpointName, e =>
                    {
                        e.Lazy = true;
                        e.PrefetchCount = 20;
                        e.Consumer(consumerType, type => Activator.CreateInstance(type) as IConsumer);
                    });
                }
            });
        }



    }
}
