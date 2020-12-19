using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Producer
{
    class Program
    {
        static string[] messages = new string[] { "Your opinion is not a fact",  "The virus isn't gone just because the year is finish", "Someone developed the internet without help of internet", "Noob means new player and not bad player", "Your coupon code is expired", "Wearing a mask under your nose is stupid", "Social security cards are just bad versions of ID"};
        static Random rnd = new Random();

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

                    //Send 100 message
                    for(int i = 0; i < 100; i++)
                    {
                        string message = CreateRandomMessage() + " - " + (i + 1);

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

        static string CreateRandomMessage()
        {

            return messages[rnd.Next(0, messages.Length)];
        }
    }
}
