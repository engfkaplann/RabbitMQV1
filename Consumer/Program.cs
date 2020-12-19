using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory();

            //Connection string
            factory.Uri = new Uri("amqp://guest:guest@localhost:5672");

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //Durable:Eğer false ise rabbitmq restart atıldığında kuyruktaki tüm veriler memory haricinde bir yere kayıt edilmediği için kaybedilir.
                    //Exclusive: Bu kuyrağa bir kanal mı bağlansın yoksa başka kanallarda bağlanabilsin mi ? False ise başka kanallarda bağlanabilir.
                    //AutoDelete: Kuydukta bulunan bütün mesajlar kuyruktan çıkınca kuyruk silinsin mi silinmesin mi ?
                    //Producer tarafında bulunan parametreler ile aynı olmalı
                    channel.QueueDeclare("hard_to_swallow_pills", true, false, false, null);

                    var consumer = new EventingBasicConsumer(channel);

                    //AutoAck: true ise queue içerisinde yer alan veriler consume edildiğin anda silinir
                    channel.BasicConsume("hard_to_swallow_pills", false, consumer);

                    consumer.Received += (model, ea) =>
                    {
                        ConsumeMethod(model, ea);
                    };
                }
            }

            Console.ReadLine();
        }

        static void ConsumeMethod(object? model, BasicDeliverEventArgs ea)
        {
            var bodyByte = ea.Body.ToArray();

            var message = Encoding.UTF8.GetString(bodyByte);

            Console.WriteLine(message);
        }
    }
}
