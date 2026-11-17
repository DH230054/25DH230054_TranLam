using System;
using System.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
namespace MyStore.Web.Services
{
    // PayPal REST Orders v2 (Sandbox), không dùng giả lập kết quả thanh toán.
    // Production cần tích hợp webhook / hoàn tiền / xử lý idempotency ở database.
    public class PayPalService
    {
        private static readonly HttpClient Client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        private const string Api = "https://api-m.sandbox.paypal.com";
        public static bool Configured => !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["PayPalClientId"])
            && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["PayPalClientSecret"]);
        public static decimal VndPerUsd
        {
            get
            {
                decimal n;
                return decimal.TryParse(ConfigurationManager.AppSettings["VndPerUSD"],NumberStyles.Number,CultureInfo.InvariantCulture,out n)&&n>0?n:25000m;
            }
        }
        public static decimal ConvertToUsd(decimal vnd)=>Math.Round(vnd/VndPerUsd,2,MidpointRounding.AwayFromZero);
        private static async Task<string> Token()
        {
            var id=ConfigurationManager.AppSettings["PayPalClientId"];
            var secret=ConfigurationManager.AppSettings["PayPalClientSecret"];
            if(!Configured)throw new InvalidOperationException("Chưa thiết lập PayPalClientId / PayPalClientSecret trong Web.config.");
            var basic=Convert.ToBase64String(Encoding.ASCII.GetBytes(id+":"+secret));
            using(var req=new HttpRequestMessage(HttpMethod.Post,Api+"/v1/oauth2/token"))
            {
                req.Headers.Authorization=new AuthenticationHeaderValue("Basic",basic);
                req.Content=new FormUrlEncodedContent(new Dictionary<string,string>{{"grant_type","client_credentials"}});
                using(var response=await Client.SendAsync(req))
                {
                    var body=await response.Content.ReadAsStringAsync();
                    if(!response.IsSuccessStatusCode)throw new InvalidOperationException("PayPal không cấp quyền: "+(int)response.StatusCode);
                    return (string)JObject.Parse(body)["access_token"];
                }
            }
        }
        private static async Task<JObject> Send(HttpMethod method,string endpoint,string content=null)
        {
            var token=await Token();
            using(var req=new HttpRequestMessage(method,Api+endpoint))
            {
                req.Headers.Authorization=new AuthenticationHeaderValue("Bearer",token);
                req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if(content!=null)req.Content=new StringContent(content,Encoding.UTF8,"application/json");
                using(var response=await Client.SendAsync(req))
                {
                    var body=await response.Content.ReadAsStringAsync();
                    if(!response.IsSuccessStatusCode)throw new InvalidOperationException("Lỗi PayPal: "+(int)response.StatusCode+" (xem thông tin tài khoản Sandbox)");
                    return JObject.Parse(body);
                }
            }
        }
        public static async Task<Tuple<string,string>> Create(decimal totalVnd,string successUrl,string cancelUrl)
        {
            var total=ConvertToUsd(totalVnd);
            if(total<0.01m)throw new InvalidOperationException("Số tiền quy đổi không hợp lệ.");
            var payload=new JObject{
                ["intent"]="CAPTURE",
                ["purchase_units"]=new JArray(new JObject{
                    ["amount"]=new JObject{{"currency_code","USD"},{"value",total.ToString("F2",CultureInfo.InvariantCulture)}}
                }),
                ["application_context"]=new JObject{
                    ["return_url"]=successUrl,["cancel_url"]=cancelUrl,["user_action"]="PAY_NOW"
                }
            };
            var response=await Send(HttpMethod.Post,"/v2/checkout/orders",payload.ToString());
            var id=(string)response["id"];
            string approval=null;
            foreach(var l in (JArray)response["links"] ?? new JArray()) if((string)l["rel"]=="approve")approval=(string)l["href"];
            if(string.IsNullOrEmpty(id)||string.IsNullOrEmpty(approval))throw new InvalidOperationException("PayPal không trả về liên kết thanh toán.");
            return Tuple.Create(id,approval);
        }
        public static async Task<bool> Capture(string paypalOrderId,decimal totalVnd)
        {
            // paymentId MUST come from server-side Session, never from user query string alone.
            if(string.IsNullOrEmpty(paypalOrderId)||paypalOrderId.Length>100) return false;
            var response=await Send(HttpMethod.Post,"/v2/checkout/orders/"+Uri.EscapeDataString(paypalOrderId)+"/capture","{}");
            var captures=response["purchase_units"]?[0]?["payments"]?["captures"] as JArray;
            if((string)response["id"]!=paypalOrderId||(string)response["status"]!="COMPLETED"||captures==null||captures.Count==0)return false;
            var capture=captures[0];
            decimal paid;
            return (string)capture["status"]=="COMPLETED" && (string)capture["amount"]?["currency_code"]=="USD"
                && decimal.TryParse((string)capture["amount"]?["value"],NumberStyles.Number,CultureInfo.InvariantCulture,out paid)
                && paid==ConvertToUsd(totalVnd);
        }
    }
}
