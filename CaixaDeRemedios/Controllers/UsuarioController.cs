
                
  using CaixaDeRemedios.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Firebase.Database;
using System;
using Firebase.Database.Query;
using System.Xml.Linq;
using CaixaDeRemedios;
using HiveMQtt.Client;
using HiveMQtt.MQTT5.Types;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace CaixaDeRemedios.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        public FirebaseClient client;

        public UsuarioController()
        {
            client = new FirebaseClient("https://caixainteligentedemedicamentos-default-rtdb.firebaseio.com/");
        }

        [HttpGet]

        public async Task<ActionResult<List<RemedioModel>>> VerificaAlarmeAsync()
        {
            try
            {
                //Cria conecxão com o mqtt
                HiveMQClient mqtt = await Program.CriaConexaoMQTTAsync();


                //Faz consulta pelos remedios cadastrados
                var remedios = (await client.Child("Remedios").OnceAsync<RemedioModel>()).ToList();

                //Faz consulta pelos usuarios cadastrados
                var usuarios = (await client.Child("Users").OnceAsync<UsuarioModel>()).ToList();

                foreach (var remedioHorario in remedios)
                {
                    var remedio = remedioHorario.Object;
                    DateTime dataHoraRemedio = DateTime.Parse(remedio.HorarioProximoRemedio);



                    var usuario = usuarios.Where(x => x.Object.Id == remedio.IdUsuario).FirstOrDefault();

                    if (usuario != null)
                    {
                        var usuarioEsp = usuario.Object;

                        if (dataHoraRemedio.Hour == DateTime.Now.Hour && dataHoraRemedio.Minute == DateTime.Now.Minute)
                        {
                            //Envia comando de alarme ativo para o ESP
                            var msg = "1/" + remedio.Recipiente.ToString();
                            //Publish MQTT messages


                            var resultado = await mqtt.PublishAsync(usuarioEsp.ChaveEsp32, msg, QualityOfService.AtLeastOnceDelivery).ConfigureAwait(false);


                            //Inserir o alarme na tabela
                            FirebaseObject<AlarmeModel> result = await client.Child("Alarmes")
                            .PostAsync(new AlarmeModel()
                            {
                                Id = Guid.NewGuid().ToString() + remedio.NomeRemedio,
                                IdUsuario = remedio.IdUsuario,
                                IdRemedio = remedio.Id,
                                Ativo = true,
                                DataHoraCadastro = DateTime.Now

                            });

                            //Mandar para o respectivo esp 32 e recipiente o alarme

                            dataHoraRemedio = dataHoraRemedio.AddHours(remedio.Frequencia);
                            remedio.HorarioProximoRemedio = (dataHoraRemedio.Hour.ToString() + ":" + dataHoraRemedio.Minute.ToString());
                            await client.Child("Remedios").Child(remedioHorario.Key).PutAsync(remedio);
                        }
                    }
                }


                var alarmes = (await client.Child("Alarmes").OnceAsync<AlarmeModel>()).ToList();

                alarmes = alarmes.Where(x => x.Object.Ativo == true).ToList();

                foreach (var alarmeLimite in alarmes)
                {
                    var alarme = alarmeLimite.Object;

                    var remedio = remedios.Where(x => x.Object.Id == alarme.IdRemedio).FirstOrDefault();

                    if (remedio != null)
                    {

                        TimeSpan diferencaTempo = DateTime.Now.Subtract(alarme.DataHoraCadastro);

                        if (diferencaTempo.Minutes >= 5 || diferencaTempo.Hours > 1 || diferencaTempo.Days > 1)
                        {
                            var user = usuarios.Where(x => x.Object.Id == remedio.Object.IdUsuario).FirstOrDefault();

                            if (user != null)
                            {
                                //Envia comando de alarme ativo para o ESP
                                var msg = "0/" + remedio.Object.Recipiente.ToString();
                                //Publish MQTT messages
                                var resultado = await mqtt.PublishAsync(user.Object.ChaveEsp32, msg, QualityOfService.AtLeastOnceDelivery).ConfigureAwait(false);

                                alarme.Ativo = false;
                                await client.Child("Alarmes").Child(alarmeLimite.Key).PutAsync(alarme);

                            }


                        }
                    }
                }
            }
            catch
            {
                throw new Exception("Erro subscribe");
            }
            

            return Ok();
        }
    }
}



