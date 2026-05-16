using RabbitMQ.Client;
using System.Text;

/*
 * Sets of working program launch settings:
 *
 * Consumer : dotnet run anonymous.info
 * Producer : dotnet run
 *
 * Consumer : dotnet run anonymous.*
 * Producer : dotnet run
 *
 * Consumer : dotnet run *.info
 * Producer : dotnet run
 *
 */


var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "topic_logs", type: ExchangeType.Topic);

var routingKey = (args.Length > 0) ? args[0] : "anonymous.info";
var message = (args.Length > 1) ? string.Join(" ", args.Skip(1).ToArray()) : "Hello World!";
var body = Encoding.UTF8.GetBytes(message);
await channel.BasicPublishAsync(exchange: "topic_logs", routingKey: routingKey, body: body);
Console.WriteLine($" [x] Sent '{routingKey}':'{message}'");