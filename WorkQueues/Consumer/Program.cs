using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "Emails",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                Console.WriteLine(" [*] Waiting for Emails.");

                var consumer = new EventingBasicConsumer(channel);
                int countRecive = 0;
                consumer.Received += (model, ea) =>
                {
                    countRecive++;
                    if(countRecive == 10)
                    {
                        countRecive = 0;
                        Console.WriteLine("Received {10} Email");
                    }
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: "Emails",
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
