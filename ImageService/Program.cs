using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ImageService
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var routingKey = "convert.img";
                var exchangekey = "asset_coverter";

                channel.ExchangeDeclare(
                    exchange: exchangekey,
                    durable: true,
                    type: "topic");

                var queueName = "imageQ";
                channel.QueueDeclare(
                    queue: queueName,
                    durable: true
                    );

                channel.QueueBind(queue: queueName,
                                  exchange: exchangekey,
                                  routingKey: routingKey
                                  );


                Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;
                    Thread.Sleep(1000); //Fake work
                    channel.BasicPublish(
                        exchange: exchangekey,
                        routingKey: ea.BasicProperties.ReplyTo,
                        basicProperties: null,
                        body: Encoding.UTF8.GetBytes("DoneWithImage")
                    );
                    Console.WriteLine(" [x] Received '{0}':'{1}'",
                                      routingKey,
                                      message);
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);


                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
