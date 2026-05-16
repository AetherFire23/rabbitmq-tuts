using System.Text;
using RabbitMQ.Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        /*
         * Runs 
         *  docker run -d --hostname my-rabbit --name some-rabbit rabbitmq:3-management
        */
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(exchange: "logs", type: ExchangeType.Fanout);

        var message = GetMessage(args);
        var body = Encoding.UTF8.GetBytes(message);
        
        // Note that the routing key is empty. 
        await channel.BasicPublishAsync(exchange: "logs", routingKey: string.Empty, body: body);
        Console.WriteLine($" [x] Sent {message}");

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    static string GetMessage(string[] args)
    {
        return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
    }
}