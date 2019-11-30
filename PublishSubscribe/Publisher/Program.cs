using RabbitMQ.Client;
using System;
using System.Text;

namespace Publisher
{
    class Program
    {
        private static IConnection connection;
        private static IModel channel;
        static void Main(string[] args)
        {
            RegisterPublisher();
            var message = "";
            while (message.ToLower() != "cancel")
            {
                Console.WriteLine(" Enter [cancel] to exit.");
                Console.WriteLine(" Please enter a number of message(s) will be send (n >= 1).");
                int totalMessage;
                if (int.TryParse(Console.ReadLine(), out totalMessage))
                {
                    Console.WriteLine(" Please enter a message to send.");
                    message = Console.ReadLine();
                    for (int i = 1; i <= totalMessage; i++)
                    {
                        Publish($"Message index {i}: {message}");
                    }
                }
            }
        }

        private static void RegisterPublisher()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);
        }

        private static void Publish(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "logs",
                                   routingKey: "",
                                   basicProperties: null,
                                   body: body);
            Console.WriteLine(" [x] Sent {0}", message);
        }
    }
}
