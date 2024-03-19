using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using asysa_inmed_cenabast.Class;

namespace Asysa_RGM_VTEX_Servicio
{
    public class ConsultasAPI
    {
        private readonly string apiUrl;
        private readonly string appKey;
        private readonly string appToken;

        public ConsultasAPI()
        {
            this.apiUrl = "";
            this.appToken = "";
        }

        public async Task<List<Tuple<int, string>>> ObtenerOrdenesDesdeApiAsync()
        {
            List<Tuple<int, string>> resultado = new List<Tuple<int, string>>();

            try
            {
                string apiUrl = "https://190.107.178.9/api/orders";  //CAMBIAR API


                // Crear un manejador que ignore los errores de certificado SSL
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                using (HttpClient httpClient = new HttpClient(handler)) // Usar este manejador
                {
                    string appToken = "Bearer 1|yoUx5wOVC2nEG4qTBeRFb9aC9R5mTpScjGyVRnvG";  //CAMBIAR DATOS

                    // Configura el encabezado Accept
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    //httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")); 


                    // Agrega el encabezado Authorization CAMBIAR DATOS
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer 1|yoUx5wOVC2nEG4qTBeRFb9aC9R5mTpScjGyVRnvG");
                    // httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");

                    // Realiza la solicitud GET
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        var apiResponse = JObject.Parse(jsonResponse);
                        Logger.Log($"Se ha realizado la operación en la API de bicimoto");

                        var orders = apiResponse["data"]["data"];

                        foreach (var order in orders)
                        {
                            int orderId = order["id"].Value<int>();
                            string paymentStatus = order["payment_status"].Value<string>();

                            // Agrega la información a la lista
                            resultado.Add(new Tuple<int, string>(orderId, paymentStatus));
                        }

                        return resultado;
                    }
                    else
                    {
                        Logger.Log($"Error al obtener datos de la API de bicimoto. Código de estado: {response.StatusCode}");
                        return resultado; // o algún valor predeterminado en caso de error
                    }
                }
            }
            catch (Exception ex)x
            {
                Logger.Log($"Error al obtener datos de la API de bicimoto: {ex.Message}");
            }

            return resultado;
        }

        public async Task<NotaVentaWeb> ObtenerDatoOrdenDesdeApiAsync(int id)
        {
            try
            {
                string apiUrl = $"https://190.107.178.9/api/orders/{id}";


                // Crear un manejador que ignore los errores de certificado SSL
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                using (HttpClient httpClient = new HttpClient(handler)) // Usar este manejador
                {
                    // Configura el encabezado Accept
                    httpClient.DefaultRequestHeaders.Accept.Clear();

                    // Agrega el encabezado Authorization
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer 1|yoUx5wOVC2nEG4qTBeRFb9aC9R5mTpScjGyVRnvG");  //CAMBIAR API

                    // Realiza la solicitud GET
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);

                        if (apiResponse.Success && apiResponse.Data != null)
                        {
                            return apiResponse.Data;
                        }
                    }
                    else
                    {
                        Logger.Log($"Error al obtener datos de la API de bicimoto. Código de estado: {response.StatusCode}");
                        return null; // o algún valor predeterminado en caso de error
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error al obtener datos de la API de bicimoto: {ex.Message}");
            }

            return null;
        }

    }
}