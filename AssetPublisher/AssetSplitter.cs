using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AssetPublisher{
    public static class SplitPublisher{
        private static IConnection CreateConnection()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            return factory.CreateConnection();
        }
        public static void Send(string message)
        {
            List<char> messages = message.ToList();
            using (var connection = CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var routingKey = "split.everything"; 
                var exchangekey = "split_exchange";

                channel.ExchangeDeclare(exchange: exchangekey,
                                        type: "topic");
                
                //Sending messages
                foreach(var m in messages){
                    channel.BasicPublish(
                    exchange: exchangekey,
                    routingKey: routingKey,
                    basicProperties: null,
                    body: Encoding.UTF8.GetBytes(m + "")
                    );
                }
            }
        }
    }
}