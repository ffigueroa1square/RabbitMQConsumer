using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMQ.Consumer
{
    public class DirectExchangeConsumer
    {
        public static void Consume(IModel channel)
        {
            channel.ExchangeDeclare(exchange: "ex.direct", type: ExchangeType.Direct);
            channel.QueueDeclare(queue: "q.direct",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            channel.QueueBind(queue: "q.direct", exchange: "ex.direct", routingKey: "myBindingKey");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(message);
            };

            channel.BasicConsume(queue: "q.direct", autoAck: true, consumer: consumer);
            Console.WriteLine("Consumer started");
            Console.ReadLine();
        }
    }
}