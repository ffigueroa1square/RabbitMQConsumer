﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMQ.Consumer
{
    public static class RoutingConsumer
    {
        public static void Consume(IModel channel)
        {
            // Type your routingkey and press enter
            Console.WriteLine("Enter a rounting key:");

            // Create a string variable and get routingKey input from the keyboard and store it in the variable
            // happy path enter 'orange' or 'blue' value
            var routingKey = Console.ReadLine();

            var exchangeName = "ex.directRouted";

            // we will create a queue provided by RabbitMQ, it will contain a random queue name like amq.gen-JzTY20BRgKO-HjmUJj0wLg
            // we create a non-durable, exclusive, autodelete queue
            var queueName = channel.QueueDeclare().QueueName;

            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);
            channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);

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