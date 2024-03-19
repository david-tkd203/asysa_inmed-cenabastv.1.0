using Asysa_RGM_VTEX_Servicio;
using Microsoft.Win32;
using asysa_inmed_cenabast.Class;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asysa_inmed_cenabast.DB
{
    public class DatabaseHelper
    {
        private string connectionString;

        public DatabaseHelper()
        {   //RECORDAR CONFIGURAR: server, database,user, password
            this.connectionString = "Server=RGM-ERP;Database=AsysaNotaVentaWeb;User=sa;Password=RGM.Asysa.486;"; // Reemplaza con tu cadena de conexión
        }

        public int ConsultarBaseDeDatosExisteNotaVenta(int nvNumero)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Ejemplo de consulta: selecciona todos los registros de una tabla llamada EjemploTabla
                    string query = $"SELECT COUNT(NVNumero) as Cantidad FROM [AsysaNotaVentaWeb].[dbo].[Cabecera] where NVNumero = '{nvNumero}'";

                    int cant = 0;

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Logger.Log($"La consulta SQL {query}");
                            while (reader.Read())
                            {
                                // Procesa los datos obtenidos
                                cant = reader.GetInt32(reader.GetOrdinal("Cantidad"));

                                // Utiliza la clase Logger para registrar la información en el archivo de registro
                                Logger.Log($"Cantidad SKU: {cant}");
                            }
                        }
                    }

                    connection.Close();

                    // Devuelve la cantidad SKU obtenida
                    return cant;
                }
            }
            catch (Exception ex)
            {
                // Utiliza la clase Logger para registrar errores
                Logger.Log($"Error: {ex.Message}");

                // Devuelve un valor predeterminado en caso de error
                return -1;
            }
        }

        public string ConsultarBaseDeDatosGuardarNotaVentaCabecera(int nvNumero, string fecha, string client_code, double subtotal, string observacion, string seller_code)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Ejemplo de consulta: selecciona todos los registros de una tabla llamada EjemploTabla
                    string cabeceraQuery = @"
                                INSERT INTO [AsysaNotaVentaWeb].[dbo].[Cabecera] (
                                    [NVNumero], [nvFem], [nvEstado], [nvEstFact], [nvEstDesp], [nvEstRese], [nvEstConc],
                                    [CotNum], [NumOC], [nvFeEnt], [codaux],
                                    [VenCod], [CodMon], [CodLista],
                                    [nvObser], [nvCanalNV], [NomCon],
                                    [CodiCC], [nvSubTotal], [nvPorcDesc01],
                                    [nvPorcDesc02], [nvPorcDesc03], [nvPorcDesc04],[nvPorcDesc05],
                                    [nvMonto], [NumGuiaRes],
                                    [nvPorcFlete], [nvValflete],
                                    [nvPorcEmb], [nvValEmb], [nvEquiv],
                                    [nvNetoExento], [nvNetoAfecto], [nvTotalDesc],
                                    [ConcAuto], [CheckeoPorAlarmaVtas], [EnMantencion],
                                    [UsuarioGeneraDocto], [FechaHoraCreacion], [Sistema],
                                    [ConcManual], [proceso], [TotalBoleta], [NumReq]
                                )
                                VALUES (
                                    @NVNumero, @nvFem, @nvEstado, @nvEstFact, @nvEstDesp, @nvEstRese, @nvEstConc,
                                    @CotNum, @NumOC, @nvFeEnt,@codaux,
                                    @VenCod, @CodMon, @CodLista,
                                    @nvObser, @nvCanalNV, @NomCon,
                                    @CodiCC, @nvSubTotal, @nvPorcDesc01,
                                    @nvPorcDesc02, @nvPorcDesc03, @nvPorcDesc04,
                                    @nvPorcDesc05, @nvMonto,
                                    @NumGuiaRes, @nvPorcFlete,
                                    @nvValflete, @nvPorcEmb, @nvValEmb,
                                    @nvEquiv, @nvNetoExento, @nvNetoAfecto,
                                    @nvTotalDesc, @ConcAuto, @CheckeoPorAlarmaVtas,
                                    @EnMantencion, @UsuarioGeneraDocto, GETDATE(),
                                    @Sistema, @ConcManual, @proceso,
                                    @TotalBoleta, @NumReq
                                )";

                    string success = "";

                    using (SqlCommand command = new SqlCommand(cabeceraQuery, connection))
                    {
                        // Establece los valores de los parámetros en el comando SQL
                        command.Parameters.AddWithValue("@NVNumero", nvNumero);
                        DateTimeOffset fechaOffset = DateTimeOffset.ParseExact(fecha, "yyyy-MM-ddTHH:mm:ss.ffffffZ", System.Globalization.CultureInfo.InvariantCulture);

                        command.Parameters.AddWithValue("@nvFem", fechaOffset.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        command.Parameters.AddWithValue("@nvEstado", 'A');
                        command.Parameters.AddWithValue("@nvEstFact", 0);
                        command.Parameters.AddWithValue("@nvEstDesp", 0);
                        command.Parameters.AddWithValue("@nvEstRese", 0);
                        command.Parameters.AddWithValue("@nvEstConc", 0);
                        command.Parameters.AddWithValue("@CotNum", 0);
                        command.Parameters.AddWithValue("@NumOC", 0);// viene en 0 en todas las ultimas ventas
                        command.Parameters.AddWithValue("@nvFeEnt", fechaOffset.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        command.Parameters.AddWithValue("@codaux", client_code);
                        command.Parameters.AddWithValue("@VenCod", seller_code);
                        command.Parameters.AddWithValue("@CodMon", "01");
                        command.Parameters.AddWithValue("@CodLista", "VEN");
                        command.Parameters.AddWithValue("@nvObser", string.IsNullOrEmpty(observacion) ? DBNull.Value : (object)observacion);
                        command.Parameters.AddWithValue("@nvCanalNV", "C1");
                        command.Parameters.AddWithValue("@NomCon", "VENTA");
                        command.Parameters.AddWithValue("@CodiCC", 70);
                        command.Parameters.AddWithValue("@nvSubTotal", subtotal);
                        command.Parameters.AddWithValue("@nvPorcDesc01", 0);
                        command.Parameters.AddWithValue("@nvPorcDesc02", 0);
                        command.Parameters.AddWithValue("@nvPorcDesc03", 0);
                        command.Parameters.AddWithValue("@nvPorcDesc04", 0);
                        command.Parameters.AddWithValue("@nvPorcDesc05", 0);
                        command.Parameters.AddWithValue("@nvMonto", Math.Round(subtotal * 1.19));
                        command.Parameters.AddWithValue("@NumGuiaRes", 0);
                        command.Parameters.AddWithValue("@nvPorcFlete", 0);
                        command.Parameters.AddWithValue("@nvValflete", 0);
                        command.Parameters.AddWithValue("@nvPorcEmb", 0);
                        command.Parameters.AddWithValue("@nvValEmb", 0);

                        // ... Continuar asignando parámetros para los  envío, pago, facturación, etc.

                        // Asignación de parámetros para la sección de envío
                        command.Parameters.AddWithValue("@nvEquiv", 1);
                        command.Parameters.AddWithValue("@nvNetoExento", 0);
                        command.Parameters.AddWithValue("@nvNetoAfecto", subtotal);
                        command.Parameters.AddWithValue("@nvTotalDesc", 0);
                        command.Parameters.AddWithValue("@ConcAuto", 'N');
                        command.Parameters.AddWithValue("@CheckeoPorAlarmaVtas", 'N');
                        command.Parameters.AddWithValue("@EnMantencion", 0);
                        command.Parameters.AddWithValue("@UsuarioGeneraDocto", "ASYSA WEB");
                        command.Parameters.AddWithValue("@Sistema", "NVW");
                        command.Parameters.AddWithValue("@ConcManual", 'N');
                        command.Parameters.AddWithValue("@proceso", "Captura de Notas de Ventas");
                        command.Parameters.AddWithValue("@TotalBoleta", 0);
                        command.Parameters.AddWithValue("@NumReq", 0);


                        // Ejecuta el comando
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Logger.Log("Inserción exitosa de Cabecera.");
                            return "Exito";
                        }
                        else
                        {
                            Logger.Log("No se insertaron filas de Cabecera.");
                            return "Error";
                        }
                    }

                    // Devuelve la cantidad SKU obtenida
                    return success;
                }
            }
            catch (Exception ex)
            {
                // Utiliza la clase Logger para registrar errores
                Logger.Log($"Error: {ex.Message}");

                // Devuelve un valor predeterminado en caso de error
                return "Error";
            }
        }

        public string ConsultarBaseDeDatosGuardarNotaVentaItem(int nvNumero, int linea, string fecha, string codProd, int quantity, double unit_price,
            double total_price, string description, string codumed, int store_code)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // Ejemplo de consulta: selecciona todos los registros de una tabla llamada EjemploTabla
                    string itemQuery = @"
                            INSERT INTO [AsysaNotaVentaWeb].[dbo].[items] (
                                [NVNumero],[nvLinea],[nvCorrela],[nvFecCompr],[CodProd]
                                ,[nvCant],[nvPrecio],[nvEquiv],[nvSubTotal],[nvDPorcDesc01]
                                ,[nvDDescto01],[nvDPorcDesc02],[nvDDescto02],[nvDPorcDesc03],[nvDDescto03]
                                ,[nvDPorcDesc04],[nvDDescto04],[nvDPorcDesc05],[nvDDescto05],[nvTotLinea]
                                ,[nvCantDesp],[nvCantProd],[nvCantFact],[nvCantDevuelto],[nvCantNC]
                                ,[nvCantOC],[DetProd],[CheckeoMovporAlarmaVtas],[CodUMed],
                                [CantUVta]
                                )
                            VALUES (
                                @NVNumero,@nvLinea, @nvCorrela, @nvFecCompr, @CodProd, @nvCant, @nvPrecio, @nvEquiv,
                                @nvSubTotal, @nvDPorcDesc01, @nvDDescto01,
                                @nvDPorcDesc02, @nvDDescto02,@nvDPorcDesc03, @nvDDescto03,
                                @nvDPorcDesc04, @nvDDescto04,@nvDPorcDesc05, @nvDDescto05,
                                @nvTotLinea, @nvCantDesp, @nvCantProd,@nvCantFact,@nvCantDevuelto,@nvCantNC,
                                @nvCantOC,@DetProd,@CheckeoMovporAlarmaVtas,@CodUMed,
                                @CantUVta
                            )";

                    string success = "";

                    using (SqlCommand command = new SqlCommand(itemQuery, connection))
                    {
                        command.Parameters.AddWithValue("@NVNumero", nvNumero);
                        command.Parameters.AddWithValue("@nvLinea", linea);
                        command.Parameters.AddWithValue("@nvCorrela", 0);
                        DateTimeOffset fechaOffset = DateTimeOffset.ParseExact(fecha, "yyyy-MM-ddTHH:mm:ss.ffffffZ", System.Globalization.CultureInfo.InvariantCulture);
                        command.Parameters.AddWithValue("@nvFecCompr", fechaOffset.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        command.Parameters.AddWithValue("@CodProd", codProd);
                        command.Parameters.AddWithValue("@nvCant", quantity);
                        command.Parameters.AddWithValue("@nvPrecio", unit_price);
                        command.Parameters.AddWithValue("@nvEquiv", store_code);
                        command.Parameters.AddWithValue("@nvSubTotal", total_price);
                        command.Parameters.AddWithValue("@nvDPorcDesc01", 0);
                        command.Parameters.AddWithValue("@nvDDescto01", 0);
                        command.Parameters.AddWithValue("@nvDPorcDesc02", 0);
                        command.Parameters.AddWithValue("@nvDDescto02", 0);
                        command.Parameters.AddWithValue("@nvDPorcDesc03", 0);
                        command.Parameters.AddWithValue("@nvDDescto03", 0);
                        command.Parameters.AddWithValue("@nvDPorcDesc04", 0);
                        command.Parameters.AddWithValue("@nvDDescto04", 0);
                        command.Parameters.AddWithValue("@nvDPorcDesc05", 0);
                        command.Parameters.AddWithValue("@nvDDescto05", 0);
                        command.Parameters.AddWithValue("@nvTotLinea", total_price);
                        command.Parameters.AddWithValue("@nvCantDesp", 0);
                        command.Parameters.AddWithValue("@nvCantProd", 0);
                        command.Parameters.AddWithValue("@nvCantFact", 0);
                        command.Parameters.AddWithValue("@nvCantDevuelto", 0);
                        command.Parameters.AddWithValue("@nvCantNC", 0);
                        command.Parameters.AddWithValue("@nvCantOC", 0);
                        command.Parameters.AddWithValue("@DetProd", description);
                        command.Parameters.AddWithValue("@CheckeoMovporAlarmaVtas", 'N');
                        command.Parameters.AddWithValue("@CodUMed", string.IsNullOrEmpty(codumed) ? DBNull.Value : (object)codumed);
                        command.Parameters.AddWithValue("@CantUVta", quantity);


                        // Ejecuta el comando
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Logger.Log("Inserción exitosa de ITEM.");
                            return "Exito";
                        }
                        else
                        {
                            Logger.Log("No se insertaron filas de ITEM.");
                            return "Error";
                        }
                    }

                    // Devuelve la cantidad SKU obtenida
                    return success;
                }
            }
            catch (Exception ex)
            {
                // Utiliza la clase Logger para registrar errores
                Logger.Log($"Error: {ex.Message}");

                // Devuelve un valor predeterminado en caso de error
                return "Error";
            }
        }


        public string ConsultarBaseDeDatosCodUMed(string codProd)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Usar parámetros para evitar inyección SQL
                    string query = "SELECT CodUMed FROM BICIMOTO.softland.iw_tprod WHERE CodProd = @CodProd";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Agregar el parámetro CodProd
                        command.Parameters.AddWithValue("@CodProd", codProd);

                        // Usar ExecuteScalar para obtener un solo valor
                        object result = command.ExecuteScalar();

                        // Verificar si se obtuvo un valor
                        if (result != null)
                        {
                            // Convertir el resultado a string
                            string codumed = result.ToString();

                            // Registrar información en el archivo de registro
                            Logger.Log($"CodUMed: {codumed}");

                            return codumed;
                        }
                        else
                        {
                            Logger.Log("No se encontró el CodUMed.");
                            return string.Empty;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Manejar excepciones específicas de SQL
                Logger.Log($"Error SQL: {ex.Message}");
                return string.Empty;
            }
            catch (Exception ex)
            {
                // Manejar otras excepciones
                Logger.Log($"Error: {ex.Message}");
                return string.Empty;
            }
        }
    }
}