using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asysa_inmed_cenabast.Class
{


    public class Items
    {
        public string code { get; set; }
        public string description { get; set; }
        public int quantity { get; set; }
        public double unit_price { get; set; }
        public double total_price { get; set; }
        public string cod_u_med { get; set; }
    }

    public class NotaVentaWeb
    {
        public int id { get; set; }
        public string created_date { get; set; }
        public string seller_code { get; set; }
        public int store_code { get; set; }
        public string client_code { get; set; }
        public string client_rut { get; set; }
        public string client_name { get; set; }
        public string client_email { get; set; }
        public string payment_type { get; set; }
        public string shipping_type { get; set; }
        public string payment_status { get; set; }
        public string shipping_status { get; set; }
        public string order_comment { get; set; }
        public string courier_name { get; set; }
        public double subtotal { get; set; }
        public decimal discount { get; set; }
        public decimal tax { get; set; }
        public decimal shipping { get; set; }
        public decimal extra { get; set; }
        public decimal total { get; set; }
        public List<Items> details { get; set; }
    }

    public class ApiResponse
    {
        public bool Success { get; set; }
        public NotaVentaWeb Data { get; set; }
        public string Message { get; set; }
    }
}
