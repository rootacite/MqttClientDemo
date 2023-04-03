
using System;
using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet.Server;

using static System.Console;

namespace MqttClientDemo;

internal class Program
{
    static void Main(string[] args)
    {
        var mqttClient = new MqttFactory().CreateMqttClient();

        var client_options = new MqttClientOptionsBuilder()
            .WithClientId("Demo_9")
            .WithTcpServer("59.110.225.239", 34420) // Port is optional
            .WithCredentials("root", "07211145141919")
            .Build();

        var r_cont = mqttClient.ConnectAsync(client_options, CancellationToken.None);

        mqttClient.ConnectedAsync += async (e) =>
        {
            await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("Sys/ServiceMsg").Build());
        };

        mqttClient.ApplicationMessageReceivedAsync += (e) =>
        {

            ForegroundColor = ConsoleColor.Cyan;
            WriteLine($"[ Message on Topic : {e.ApplicationMessage.Topic}]");
            WriteLine($"[ \"{Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}\" ]");
            ForegroundColor = ConsoleColor.White;

            return Task.CompletedTask;
        };

        while (!r_cont.IsCompleted) { }

        if (r_cont.IsFaulted)
        {
            WriteLine(r_cont.Exception?.Message);
            return;
        }

       

        while (true)
        {
            char cx = ReadKey().KeyChar;
            switch(cx)
            {
                case 'q':
                    return;
                case ' ':
                    mqttClient.PublishAsync(new MqttApplicationMessageBuilder()
           .WithTopic("Sys/ServiceMsg")
           .WithPayload("Hello World")
           .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
           .WithRetainFlag(false)
           .Build(), CancellationToken.None);
                    break;
                default:
                    break;
            }
        }
    }
}
