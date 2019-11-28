using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ConsumePublisher
{
    class Program
    {
        static IModel publishChannel;
        static IConnection publishConnection;
        static IBasicProperties publishProperties;
        static void Main(string[] args)
        {
            RegisterPublisher();
            RegisterConsumer();
        }

        private static void RegisterConsumer()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "Email_Render",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                Console.WriteLine(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);

                    Publish(message);

                    Console.WriteLine(" [x] Done");

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: "Email_Render",
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        private static void RegisterPublisher()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            publishConnection = factory.CreateConnection();
            publishChannel = publishConnection.CreateModel();
            publishChannel.QueueDeclare(queue: "Emails",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            publishProperties = publishChannel.CreateBasicProperties();
            publishProperties.Persistent = true;
        }

        private static void Publish(string message)
        {
            if (publishChannel == null)
            {
                return;
            }
            var body = Encoding.UTF8.GetBytes(message);
            publishChannel.BasicPublish(exchange: "",
                                routingKey: "Emails",
                                basicProperties: publishProperties,
                                body: body);
            Console.WriteLine(" [x] Sent {0}", message);
        }
    }
}
