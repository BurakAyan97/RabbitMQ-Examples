using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace UdemyRabbitMQ.publisher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://kohrnvpd:x1R8FeHcFDzhRnSLgJ_mxsLQExZL4_gf@rat.rmq2.cloudamqp.com/kohrnvpd");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            //channel.QueueDeclare("hello-queue", true, false, false);
            //Fanout exchange :
            channel.ExchangeDeclare("logs-fanout", ExchangeType.Fanout, true);

            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                string message = $"log {x}";

                var messageBody = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish("logs-fanout", "", null, messageBody);

                Console.WriteLine($"Mesaj gönderilmiş: {message}");
            });

            Console.ReadLine();

        }
    }
}
