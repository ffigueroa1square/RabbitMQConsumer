using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMQ.Consumer
{
    public static class HeadersExchangeConsumer
    {
        public static void Consume(IModel channel)
        {
            var exchangeName = "ex.headersRouted";
            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Headers);

            // we will create a queue provided by RabbitMQ, it will contain a random queue name like
            // amq.gen-JzTY20BRgKO-HjmUJj0wLg we create a non-durable, exclusive, autodelete queue
            var queueName = channel.QueueDeclare().QueueName;
            Console.WriteLine($"Queue name: {queueName}");

            Console.WriteLine("Enter a x-match followed by a comma, enter category value followed by a comma and enter type value:");

            var lineInputs = Console.ReadLine();
            if (lineInputs != null)
            {
                var inputs = lineInputs.Split(',');

                Dictionary<string, object> headersDictionary = new Dictionary<string, object>();
                headersDictionary.Add("x-match", inputs[0].Trim());
                headersDictionary.Add("categoria", inputs[1].Trim());
                headersDictionary.Add("tipo", inputs[2].Trim());

                channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: string.Empty, arguments: headersDictionary);
            }                        

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                //var routingKey = e.BasicProperties.
                Console.WriteLine($" [x] Received '{message}'");
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}