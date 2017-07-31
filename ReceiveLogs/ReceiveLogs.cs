using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Common;

namespace ReceiveLogs
{
    class ReceiveLogs
    {
        public static void Main()
        {
            var factory = new ConnectionFactory()
            {
                HostName = Constants.RABBIT_HOSTNAME,
                UserName = Constants.USERNAME,
                Password = Constants.PASSWORD
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "logs", type: "fanout");

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName, exchange: "logs", routingKey: "");

                Console.WriteLine(" [*] Waiting for logs.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("QueueName:{0} [x] {1}", queueName, message);
                };
                channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
