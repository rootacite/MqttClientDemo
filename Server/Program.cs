
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System.Text;
using static System.Console;

namespace Server;

internal class Program
{
    static void Main(string[] args)
    {
        ////////////////
        ///Setup Mqtt Server
        ///////////////
        var options = new MqttServerOptionsBuilder()

        //Set endpoint to localhost
        .WithDefaultEndpoint()
        .WithDefaultEndpointPort(34420);

        var server = new MqttFactory().CreateMqttServer(options.Build());

        server.InterceptingPublishAsync += Server_InterceptingPublishAsync;
        server.ValidatingConnectionAsync += Server_ValidatingConnectionAsync;
        server.ClientDisconnectedAsync += Server_ClientDisconnectedAsync;

        server.StartAsync();
    }

    private static Task Server_ClientDisconnectedAsync(ClientDisconnectedEventArgs e)
    {
        ForegroundColor = ConsoleColor.Green;
        WriteLine($"[ Client Id : {e.ClientId} Has Disconnected from Server ]");
        ForegroundColor = ConsoleColor.White;

        return Task.CompletedTask;
    }


    private static Task Server_ValidatingConnectionAsync(ValidatingConnectionEventArgs e)
    {
        if (e.Password != "07211145141919" || e.UserName != "root")
        {
            e.ReasonCode = MqttConnectReasonCode.NotAuthorized;

            ForegroundColor = ConsoleColor.Yellow;
            WriteLine($"[ A Connection from Client : {e.ClientId} Has been Refused Because of Invalid Authentication Information ]");
            WriteLine($"[ IP Address : {e.Endpoint} ]");
            ForegroundColor = ConsoleColor.White;

            return Task.FromException(new Exception("User not authorized"));
        }

        ForegroundColor = ConsoleColor.Green;
        WriteLine($"[ New Client : {e.ClientId} Has Connected Successfully with UserName ： {e.UserName} and Password : {e.Password} ]");
        WriteLine($"[ IP Address : {e.Endpoint} ]");
        ForegroundColor = ConsoleColor.White;

        return Task.CompletedTask;
    }


    private static Task Server_InterceptingPublishAsync(InterceptingPublishEventArgs arg)
    {
        // Convert Payload to string
        var payload = arg.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(arg.ApplicationMessage?.Payload);


        WriteLine(
            " TimeStamp: {0} -- Message: ClientId = {1}, Topic = {2}, Payload = {3}, QoS = {4}, Retain-Flag = {5}",

            DateTime.Now,
            arg.ClientId,
            arg.ApplicationMessage?.Topic,
            payload,
            arg.ApplicationMessage?.QualityOfServiceLevel,
            arg.ApplicationMessage?.Retain);
        return Task.CompletedTask;
    }
}
