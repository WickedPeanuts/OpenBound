using Avatar_API.Data.Json;
using Avatar_API.Data.Json.Paypal;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Avatar_API.Data.Services
{
    public class PaypalService
    {
        private readonly IConfiguration _config;
        public PaypalService(IConfiguration config)
        {
            _config = config;
        }

        public string GetClientId()
        {
            return _config.GetValue<string>("Paypal:ClientId");
        }

        public string GetSecret()
        {
            return _config.GetValue<string>("Paypal:Secret");
        }

        public string GetAuthEndpoint()
        {
            if (_config.GetValue<string>("Paypal:PaypalMode") == "LIVE")
            {
                return "https://api.paypal.com/v1/oauth2/token";
            } else
            {
                return "https://api.sandbox.paypal.com/v1/oauth2/token";
            } 
        }

        private async Task<PaypalAuth> GetPaypalToken()
        {
            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{GetClientId()}:{GetSecret()}")));
            var req = new HttpRequestMessage(HttpMethod.Post, GetAuthEndpoint()) { Content = new FormUrlEncodedContent(nvc) };
            HttpResponseMessage res = await client.SendAsync(req);
            var response = await res.Content.ReadAsStringAsync();
            PaypalAuth paypalAuth = JsonConvert.DeserializeObject<PaypalAuth>(response);

            return paypalAuth;
        }

        public async Task<PaypalCaptureIntent> GetCaptureIntent(string link) 
        {
            var paypalAuth = await GetPaypalToken();

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{paypalAuth.AccessToken}");
            var req = new HttpRequestMessage(HttpMethod.Get, link);
            HttpResponseMessage res = await client.SendAsync(req);
            var json = await res.Content.ReadAsStringAsync();
            var paypalCaptureIntent = JsonConvert.DeserializeObject<PaypalCaptureIntent>(json);

            return paypalCaptureIntent;
        }

    }
}
