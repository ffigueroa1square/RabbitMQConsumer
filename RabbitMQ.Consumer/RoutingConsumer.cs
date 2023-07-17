using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Consumer
{
    public static class RoutingConsumer
    {
        public static void Consume(IModel channel)
        {
            var exchangeName = "ex.directRouted";
            var routingKeys = new string[] { "orange", "blue" };
            // we will create a queue provided by RabbitMQ, it will contain a random queue name like amq.gen-JzTY20BRgKO-HjmUJj0wLg
            // we create a non-durable, exclusive, autodelete queue
            var queueName = channel.QueueDeclare().QueueName;

            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);
            foreach (var route in routingKeys)
            {
                channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: route);
            }

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = e.RoutingKey;
                Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
