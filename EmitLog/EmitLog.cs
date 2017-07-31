using System;
using System.Text;
using RabbitMQ.Client;
using Common;

namespace EmitLog
{
    class EmitLog
    {
        public static void Main(string[] args)
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

                string message = GetMessage(args);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "logs",
                             routingKey: "",
                             basicProperties: null,
                             body: body);

                Console.WriteLine(" [x] Sent {0}", message);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();

        }

        private static string GetMessage(string[] args)
        {
            return args.Length > 0 ? string.Join(" ", args) : "Hello World!";
        }
    }
}
