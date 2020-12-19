using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Producer
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
                    //AutoDelete: Kuyrukta bulunan bütün mesajlar kuyruktan çıkınca kuyruk silinsin mi silinmesin mi ?
                    channel.QueueDeclare("hard_to_swallow_pills", true, false, false, null);

                    //Send 10 message
                    for(int i = 0; i < 10; i++)
                    {
                        string message = "Your opinion is not a fact - " + (i+1);

                        var bodyByte = Encoding.UTF8.GetBytes(message);

                        var properties = channel.CreateBasicProperties();

                        //Mesajın kalıcı olmasını sağladık. QueueDeclare methodundaki durable parametresininde true değerini alması gerekiyor.
                        properties.Persistent = true;

                        channel.BasicPublish("", "hard_to_swallow_pills", null, bodyByte);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Message-{i+1} sent!");
                    }
                }
            }

            Console.ReadLine();
        }
    }
}
