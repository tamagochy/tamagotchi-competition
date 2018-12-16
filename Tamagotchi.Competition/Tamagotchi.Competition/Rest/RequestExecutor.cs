//using RestSharp;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Tamagotchi.Competition.Rest
{

    public static class RequestExecutor
    {
        //public static async Task<string> ExecuteRequest(string address, IRestRequest request)
        //{
        //    var cookieContainer = new CookieContainer();
        //    var client = new RestClient(address)
        //    {
        //        CookieContainer = cookieContainer
        //    };
        //    var response = await client.ExecuteTaskAsync((RestRequest)request);
        //    return response.Content;
        //}

    }

    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(
            this HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return httpClient.PostAsync(url, content);
        }

        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
        {
            var dataAsString = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(dataAsString);
        }
    }

}
