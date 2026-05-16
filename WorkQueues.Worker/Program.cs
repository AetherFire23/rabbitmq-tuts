using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

/*
 * You need to create multiple instances of Worker
 * Then you can create instances of Producer as you please.
 * Per the docs, the server users Robin-Round to distribute tasks.
 * https://www.rabbitmq.com/tutorials/tutorial-two-dotnet
 *
 *
 * Other Summaries:
 * By default, a worker has 30 minutes to process a message.
 * Rabbitmq will redistribute the task if the worker doesn't acknowledge.
 *
 * In our examples, we explicitly turned them off by setting the autoAck ("automatic acknowledgement mode") parameter to true.
 *
 * We can remove the flag and set a custom, personalized ackonwelsdement.
 *
 * ==== Message Durability ====
 *
 * We can set a message to durable. 
 */
ConnectionFactory factory = new ConnectionFactory { HostName = "localhost" };
using IConnection connection = await factory.CreateConnectionAsync();
using IChannel channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(queue: "hello", durable: true, exclusive: false, autoDelete: false,
    arguments: new Dictionary<string, object?> { { "x-queue-type", "quorum" } });

Console.WriteLine(" [*] Waiting for messages.");

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received {message}");

    int dots = message.Split('.').Length - 1;
    await Task.Delay(dots * 1000); // changed the delay for something a little longer.....

    Console.WriteLine(" [x] Done");
};

await channel.BasicConsumeAsync("hello", autoAck: true, consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();