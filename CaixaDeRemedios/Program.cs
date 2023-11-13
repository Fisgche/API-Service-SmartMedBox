using FirebaseAdmin.Messaging;
using MQTTnet.Server;
using static System.Net.Mime.MediaTypeNames;

using HiveMQtt.Client;
using HiveMQtt.Client.Options;
using HiveMQtt.MQTT5.ReasonCodes;
using HiveMQtt.MQTT5.Types;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;

namespace CaixaDeRemedios
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Cria conecxão com o mqtt
            HiveMQClient mqtt = await Program.CriaConexaoMQTTAsync();

            MQTT cliente = new MQTT("test.mosquitto.org");

            cliente.Connect();

            cliente.Subscribe("TOPICO_PUBLISH_MED_BOX_SMART");

            cliente.MessageReceived += async (t, m) =>
            {
                //Envia comando de alarme ativo para o ESP     
                //Publish MQTT messages
                try
                {
                    _ = Monitoramento.VerificaRemedioAsync(m);
                }
                catch
                {
                    throw new Exception("Erro publish");
                }
            };


            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();

        }


        public static void VerificaMensagem(string mensagem)
        {
            
        }
        

        public static async Task<HiveMQClient> CriaConexaoMQTTAsync()
        {
            var options = new HiveMQClientOptions
            {
                Host = "test.mosquitto.org",
                Port = 1883,
                UseTLS = false,
                UserName = "",
                Password = "",
            };

            var client = new HiveMQClient(options);


            Console.WriteLine($"Connecting to {options.Host} on port {options.Port} ...");

            // Connect
            HiveMQtt.Client.Results.ConnectResult connectResult;
            try
            {
                connectResult = await client.ConnectAsync().ConfigureAwait(false);
                if (connectResult.ReasonCode == ConnAckReasonCode.Success)
                {
                    Console.WriteLine($"Connect successful: {connectResult}");
                }
                else
                {
                    // FIXME: Add ToString
                    Console.WriteLine($"Connect failed: {connectResult}");
                    Environment.Exit(-1);
                }
            }
            catch (System.Net.Sockets.SocketException e)
            {
                Console.WriteLine($"Error connecting to the MQTT Broker with the following socket error: {e.Message}");
                Environment.Exit(-1);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error connecting to the MQTT Broker with the following message: {e.Message}");
                Environment.Exit(-1);
            }

            return client;
        }
    }
}