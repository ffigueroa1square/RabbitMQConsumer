using RabbitMQ.Client;
using RabbitMQ.Consumer;

internal static class Program
{
    public static void Main(string[] args)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri("amqp://guest:guest@localhost:5672")
        };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        /*
        // uncomment the rabbitmq pattern you want to use
        */

        //SimpleQueueConsumer.Consume(channel);
        //DirectExchangeConsumer.Consume(channel);
        //WorkQueuesConsumer.Consume(channel);
        //FanoutExchangeConsumer.Consume(channel);
        //RoutingConsumer.Consume(channel);
        TopicsExchangeConsumer.Consume(channel);
    }
}
