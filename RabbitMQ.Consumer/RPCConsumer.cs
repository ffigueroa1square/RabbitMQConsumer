using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMQ.Consumer
{
    public static class RPCConsumer
    {
        public static void Consume(IModel channel)
        {
            var queueName = "rpc-queue";

            channel.QueueDeclare(queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);            

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Received Request: {e.BasicProperties.CorrelationId} with message: '{message}'");

                int dots = message.Split('.').Length - 1;
                Thread.Sleep(dots * 1000);                

                var replyMessage = $"This is your reply: { e.BasicProperties.CorrelationId} with message: '{message}'";
                var replyBody = Encoding.UTF8.GetBytes(replyMessage);

                channel.BasicPublish(exchange: string.Empty, routingKey: e.BasicProperties.ReplyTo, basicProperties: null, body: replyBody);
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}