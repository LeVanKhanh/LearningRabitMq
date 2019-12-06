using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Publisher
{
    class Program
    {
        static IModel channel;
        static IConnection connection;

        static void Main()
        {
            var severities = new List<string> { "error", "infor", "warning" };
            RegisterPublisher();
            while (true)
            {
                Console.WriteLine(" Do you want to continue?(y/n):");
                var continueKey = Console.ReadLine();

                if (string.Equals(continueKey, "n", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                Console.WriteLine(" Please enter a severity(error, infor, warning).");
                var severity = Console.ReadLine();
                if (severities.Contains(severity.ToLower()))
                {
                    Console.WriteLine(" Please enter a message.");
                    var message = Console.ReadLine();
                    Publish(severity, message);
                }
                else
                {
                    Console.WriteLine($"Then severity {severity} won't be sent.");
                }
            }

        }

        private static void RegisterPublisher()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "direct_logs", type: "direct");
        }

        private static void Publish(string severity, string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "direct_logs",
                                    routingKey: severity,
                                    basicProperties: null,
                                    body: body);

            Console.WriteLine(" [x] Sent '{0}':'{1}'", severity, message);
        }

    }
}
