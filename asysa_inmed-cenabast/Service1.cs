using System;
using System.ServiceProcess;
using System.Timers;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Diagnostics;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace asysa_inmed_cenabast
{
    public partial class Service1 : ServiceBase
    {
        private Timer timer;
        private readonly IConfiguration configuration;

        public Service1()
        {
            InitializeComponent();
            configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
        }

        // Este método se llama cuando el servicio se inicia.
        protected override void OnStart(string[] args)
        {
            // Configuración del temporizador para ejecutar el método TimerElapsed cada 60 segundos.
            timer = new Timer();
            timer.Interval = 60000;
            timer.Elapsed += TimerElapsed;
            timer.Start();
        }

        // Este método se llama cada vez que el temporizador alcanza su intervalo.
        private async void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                // Obtener la URL de la API y el contenido JSON desde la configuración.
                string apiUrl = configuration["ApiUrl"];
                string jsonPayload = configuration["ApiJsonPayload"];

                // Enviar la solicitud HTTP a la API.
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.PostAsync(apiUrl, new StringContent(jsonPayload));
                    response.EnsureSuccessStatusCode();
                }

                // Registro de éxito
                LogSuccess("Envío de JSON exitoso");
            }
            catch (Exception ex)
            {
                // Registro de error
                LogError($"Error al enviar JSON: {ex.Message}");
                // Envío de correo electrónico
                SendErrorEmail(ex.Message);
            }
        }

        // Este método registra mensajes de error en un archivo de registro.
        private void LogError(string message)
        {
            string logPath = configuration["LogFilePath"];
            File.AppendAllText(logPath, $"[ERROR] {DateTime.Now}: {message}\n");
        }

        // Este método registra mensajes de éxito en un archivo de registro.
        private void LogSuccess(string message)
        {
            string logPath = configuration["LogFilePath"];
            File.AppendAllText(logPath, $"[SUCCESS] {DateTime.Now}: {message}\n");
        }

        // Este método envía un correo electrónico en caso de error.
        private void SendErrorEmail(string errorMessage)
        {
            string toAddress = configuration["ToEmailAddress"];
            string subject = "Error en el servicio";
            string body = $"Se produjo un error en el servicio:\n\n{errorMessage}";

            using (SmtpClient smtpClient = new SmtpClient(configuration["SmtpServer"]))
            {
                smtpClient.Port = int.Parse(configuration["SmtpPort"]);
                smtpClient.Credentials = new System.Net.NetworkCredential(configuration["SmtpUsername"], configuration["SmtpPassword"]);
                smtpClient.EnableSsl = true;

                MailMessage mailMessage = new MailMessage(configuration["SmtpUsername"], toAddress, subject, body);
                smtpClient.Send(mailMessage);
            }
        }

        // Este método se llama cuando el servicio se detiene.
        protected override void OnStop()
        {
            // Detener y liberar el temporizador.
            timer.Stop();
            timer.Dispose();
        }
    }
}
