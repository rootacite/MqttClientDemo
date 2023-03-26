
using System;

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
         .WithTcpServer("127.0.0.1", 34420) // Port is optional
         .WithCredentials("root", "07211145141919")
        .Build();

        var r_cont =  mqttClient.ConnectAsync(client_options, CancellationToken.None);


        while (!r_cont.IsCompleted) { }

        if(r_cont.IsFaulted)
        {
            WriteLine(r_cont.Exception.Message);
        }
        ReadKey();
    }
}
