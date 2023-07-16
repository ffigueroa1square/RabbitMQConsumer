using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMQ.Consumer
{
    public class FanoutExchangeConsumer
    {
        public static void Consume(IModel channel)
        {
            channel.ExchangeDeclare(exchange: "ex.events", type: ExchangeType.Fanout);

            // we will create a queue provided by RabbitMQ, it will contain a random queue name like amq.gen-JzTY20BRgKO-HjmUJj0wLg
            // we create a non-durable, exclusive, autodelete queue
            var queueName = channel.QueueDeclare().QueueName;            

            channel.QueueBind(queue: queueName,
                exchange: "ex.events",
                routingKey: string.Empty);

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] {message}");
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}