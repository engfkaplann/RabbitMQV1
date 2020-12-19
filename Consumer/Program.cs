using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory();

            //Connection string
            factory.Uri = new Uri("amqp://guest:guest@localhost:5672");

            var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            //Durable:Eğer false ise rabbitmq restart atıldığında kuyruktaki tüm veriler memory haricinde bir yere kayıt edilmediği için kaybedilir.
            //Exclusive: Bu kuyrağa bir kanal mı bağlansın yoksa başka kanallarda bağlanabilsin mi ? False ise başka kanallarda bağlanabilir.
            //AutoDelete: Kuydukta bulunan bütün mesajlar kuyruktan çıkınca kuyruk silinsin mi silinmesin mi ?
            //Producer tarafında bulunan parametreler ile aynı olmalı
            channel.QueueDeclare("hard_to_swallow_pills", true, false, false, null);

            //PrefetchCount: Consumer'ın alacağı maksimum mesaj sayısı
            //Global: Birden fazla consumer'ın olduğu bir yapıda global alanı false olan her bir consumer prefetchCount kadar mesaj alabilir. Global alanı true olanlar toplamda prefetchCount kadar mesaj alabilir.
            channel.BasicQos(0, 2, false);

            var consumer = new EventingBasicConsumer(channel);

            //AutoAck: true ise queue içerisinde yer alan veriler consume edildiğin anda silinir
            channel.BasicConsume("hard_to_swallow_pills", false, consumer);

            //Alınmış her mesaj için çalışacak consume methodu
            consumer.Received += (model, ea) =>
            {
                ConsumeMethod(args, model, ea, channel);
            };

            Console.ReadLine();
        }

        static void ConsumeMethod(string[] args, object model, BasicDeliverEventArgs ea, IModel channel)
        {
            var bodyByte = ea.Body.ToArray();

            var message = Encoding.UTF8.GetString(bodyByte);

            int interval = GetParameter<int>(args, 0);
            Thread.Sleep(interval);

            //Mesajın consume edildi bilgisini yollayıp mesajı queue'dan kaldırıyoruz
            channel.BasicAck(ea.DeliveryTag, false);

            Console.WriteLine(message);
        }

        /// <summary>
        /// Returns start parameter
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static T GetParameter<T>(string[] args, int index)
        {
            if(index < 0 || index >= args.Length)
            {
                return (T)Convert.ChangeType(GetDefault(typeof(T)), typeof(T));
            }

            return (T)Convert.ChangeType(args[index], typeof(T));
        }

        /// <summary>
        /// Returns default value type instance 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}
