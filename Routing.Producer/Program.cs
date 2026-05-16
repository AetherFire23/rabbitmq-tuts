using System.Text;
using RabbitMQ.Client;

/*
 * Can launch this program with arugments to only send to a specific routingKey
 * ex: dotnet run info
 */
ConnectionFactory factory = new ConnectionFactory { HostName = "localhost" };
using IConnection connection = await factory.CreateConnectionAsync();
using IChannel channel = await connection.CreateChannelAsync();

/*
 * The big difference here is the Direct route option.
 * I can choose a routing key (important). Then you can submit message only to consumers that have a certain routing key. 
 */
await channel.ExchangeDeclareAsync(exchange: "direct_logs", type: ExchangeType.Direct);

string severity = (args.Length > 0) ? args[0] : "info";
string message = (args.Length > 1) ? string.Join(" ", args.Skip(1).ToArray()) : "Hello World!";
await channel.BasicPublishAsync(exchange: "direct_logs", routingKey: severity, body: Encoding.UTF8.GetBytes(message));
Console.WriteLine($" [x] Sent '{severity}':'{message}'");

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();