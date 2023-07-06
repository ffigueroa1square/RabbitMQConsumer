﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

internal static class Program
{
    private static void Main(string[] args)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri("amqp://guest:guest@localhost:5672")
        };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: "demo-queue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (sender, e) =>
        {
            var body = e.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(message);
        };

        channel.BasicConsume(queue: "demo-queue", autoAck: true, consumer: consumer);
        Console.WriteLine("Consumer started");
        Console.ReadLine();
    }
}