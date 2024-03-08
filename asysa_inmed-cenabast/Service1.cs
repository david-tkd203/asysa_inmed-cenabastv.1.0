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
        private DateTime lastLogDate;
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
            // Inicializa la fecha del último registro al día actual.
            lastLogDate = DateTime.Now.Date;

            // Registra el inicio del servicio en el archivo de registro.
            LogStart("Inicio del servicio API_ASYSA_CENABAST");

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
                // Verifica si el día actual es diferente al día del último registro.
                if (DateTime.Now.Date != lastLogDate)
                {
                    // Si es diferente, crea un nuevo archivo de registro.
                    lastLogDate = DateTime.Now.Date;
                    CreateNewLogFile();
                }

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
            string logPath = GetLogFilePath();
            File.AppendAllText(logPath, $"[ERROR] {DateTime.Now}: {message}\n");
        }

        // Este método registra mensajes de éxito en un archivo de registro.
        private void LogSuccess(string message)
        {
            string logPath = GetLogFilePath();
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

        // Este método registra el inicio del servicio en el archivo de registro.
        private void LogStart(string message)
        {
            string logPath = GetLogFilePath();
            File.AppendAllText(logPath, $"[SERVICE START] {DateTime.Now}: {message}\n");
        }

        // Este método crea un nuevo archivo de registro con el nombre log_asysa_cenabast_fecha.
        private void CreateNewLogFile()
        {
            string logFileName = $"log_asysa_cenabast_{DateTime.Now:yyyyMMdd}.txt";
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logFileName);
            File.WriteAllText(logPath, string.Empty); // Crea un archivo vacío.
        }

        // Este método devuelve la ruta del archivo de registro actual.
        private string GetLogFilePath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"log_asysa_cenabast_{lastLogDate:yyyyMMdd}.txt");
        }
    }
}


