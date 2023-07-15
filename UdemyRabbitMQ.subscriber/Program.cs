using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace UdemyRabbitMQ.subscriber
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://kohrnvpd:x1R8FeHcFDzhRnSLgJ_mxsLQExZL4_gf@rat.rmq2.cloudamqp.com/kohrnvpd");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            //channel.ExchangeDeclare("logs-fanout", ExchangeType.Fanout, true);

            var randomQueueName = channel.QueueDeclare().QueueName; //random queue adı oluşturur.
            //Declare yerine bind yaparsan instance kapandığında kuyruk silinir.Declare yaparsan kuyruk hep durur.
            channel.QueueBind(randomQueueName, "logs-fanout", "", null);

            //perfetchSize boyutu ne kadar olsun? 0 dersek istediğin kadar gönder sıkıntı yok.
            //Count kaç tane mesaj gelsin subscribera?
            //global false olursa her bir subscribera tek seferde count kadar mesaj gönderir. True yaparsak count sayısını toplam subscribera göre eşit dağıtır. örneğin count 6 ve 3 subscriber var ise her birine 2 tane gider. count 5 subs 2 adet ise ilkine 3 diğerine 2 gider.
            channel.BasicQos(0, 1, true);

            //Publisher tarafında aynısı olduğu için subscriber otomatikman oluşturuyor. Burda olsa da olur olmasa da olur ama parametrelere dikkat et birebir aynı olmak zorunda.
            //channel.QueueDeclare("hello-queue", true, false, false); 

            var consumer = new EventingBasicConsumer(channel);
            //autoack true ise mesaj gönderildikten sonra silinir. Doğru mu yanlış mı oldu mu olmadı mı bakmaz.false yaparsak ben sana haber vericem silmen için demiş oluyoruz.

            channel.BasicConsume(randomQueueName, false, consumer);

            Console.WriteLine("Logları dinliyorum.");

            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine("Gelen mesaj: " + message);

                //autoAck false olduğunda yapmamız gereken method
                channel.BasicAck(e.DeliveryTag, false);
            };


            Console.ReadLine();
        }
    }
}
