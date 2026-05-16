using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

/*
 * Submit an argument to the program.
 */
if (args.Length < 1)
{
    Console.Error.WriteLine("Usage: {0} [binding_key...]",
        Environment.GetCommandLineArgs()[0]);
    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
    Environment.ExitCode = 1;
    return;
}

var factory = new ConnectionFactory { HostName = "localhost" };

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();
/*
 * Create a new exchange. of type TOPIC
 */
await channel.ExchangeDeclareAsync(exchange: "topic_logs", type: ExchangeType.Topic);
/*
 * Also will declare a new custom queue !
 * amq.gen-jQ4Mfi43eohxLdqwSaP5Jw
 */
QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync();
string queueName = queueDeclareResult.QueueName;

/*
 * CLI args are routing keys. ( when binded) 
 */
foreach (string? bindingKey in args)
{
    await channel.QueueBindAsync(queue: queueName, exchange: "topic_logs", routingKey: bindingKey);
}

Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var routingKey = ea.RoutingKey;
    Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
    return Task.CompletedTask;
};

await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();