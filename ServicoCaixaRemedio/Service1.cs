using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Net.Http;

namespace ServicoCaixaRemedio
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer(); // name space(using System.Timers;)  
        public Service1()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            WriteToFileAsync("Service is started at " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 5000; //number in milisecinds  
            timer.Enabled = true;
        }
        protected override void OnStop()
        {
            WriteToFileAsync("Service is stopped at " + DateTime.Now);
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            WriteToFileAsync("Service is recall at " + DateTime.Now);
        }
        public async Task WriteToFileAsync(string Message)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    // Especifique a URL da API que você deseja chamar
                    string apiUrl = "http://localhost:8082/api/Usuario";

                    // Faça uma chamada GET à API
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                    // Verifique se a solicitação foi bem-sucedida
                    if (response.IsSuccessStatusCode)
                    {
                        // Leia o conteúdo da resposta como uma string
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Resposta da API: " + responseBody);
                    }
                    else
                    {
                        Console.WriteLine("A chamada à API não foi bem-sucedida. Status Code: " + response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ocorreu um erro: " + ex.Message);
                }
            }


            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}