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
using CaixaDeRemedios.Controllers;

namespace CaixaDeRemedios
{
    public class Monitoramento
    {
        public Monitoramento() {

        }

        public static async Task VerificaRemedioAsync(string texto)
        {
            UsuarioController usuarioController = new UsuarioController();

            string[] textoSeparado = texto.Split('/');
            
            //Ação de retirada de remedio do recipiente
            if (textoSeparado[0] == "0")
            {
                var remedios = (await usuarioController.client.Child("Remedios").OnceAsync<RemedioModel>()).ToList();

                var usuarios = (await usuarioController.client.Child("Users").OnceAsync<UsuarioModel>()).ToList();

                var usuario = usuarios.Where(x => x.Object.ChaveEsp32 == textoSeparado[2]).FirstOrDefault();

                if (usuario != null)
                {

                    int Recipiente = Int32.Parse(textoSeparado[1]);

                    var remedio = remedios.Where(x => x.Object.Recipiente == Recipiente && x.Object.IdUsuario == usuario.Object.Id).FirstOrDefault();

                    if (remedio != null)
                    {
                        var alarmes = (await usuarioController.client.Child("Alarmes").OnceAsync<AlarmeModel>()).ToList();

                        var alarmesAtivos = alarmes.Where(x => x.Object.IdRemedio == remedio.Object.Id && x.Object.Ativo == true).ToList();

                        foreach (var alarmeMonitoramento in alarmesAtivos)
                        {
                            var alarme = alarmeMonitoramento.Object;

                            alarme.Ativo = false;
                            await usuarioController.client.Child("Alarmes").Child(alarmeMonitoramento.Key).PutAsync(alarme);
                        }
                    }
                }
            }
        }
    }
}
