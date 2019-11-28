using RabbitMQ.Client;
using System;
using System.Text;

namespace Publisher
{
    class Program
    {
        static IModel channel;
        static IConnection connection;
        static IBasicProperties properties;
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
            channel.QueueDeclare(queue: "Email_Render",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            properties = channel.CreateBasicProperties();
            properties.Persistent = true;
        }

        private static void Publish(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "",
                                routingKey: "Email_Render",
                                basicProperties: properties,
                                body: body);
            Console.WriteLine(" [x] Sent {0}", message);
        }
    }
}
