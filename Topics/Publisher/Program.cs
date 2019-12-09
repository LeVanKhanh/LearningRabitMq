using RabbitMQ.Client;
using System;
using System.Text;

namespace Publisher
{
    class Program
    {

        static IModel channel;
        static IConnection connection;

        static void Main()
        {
            RegisterPublisher();

            while (true)
            {
                Console.WriteLine(" Do you want to continue?(y/n):");
                var continueKey = Console.ReadLine();

                if (string.Equals(continueKey, "n", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                Console.WriteLine(" Please enter a routingKey.");
                var routingKey = Console.ReadLine();
                Console.WriteLine(" Please enter a message.");
                var message = Console.ReadLine();
                Publish(routingKey, message);
            }
        }

        private static void RegisterPublisher()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "topic_logs", type: "topic");
        }

        private static void Publish(string routingKey, string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "topic_logs",
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: body);
            Console.WriteLine(" [x] Sent '{0}':'{1}'", routingKey, message);
        }
    }
}
