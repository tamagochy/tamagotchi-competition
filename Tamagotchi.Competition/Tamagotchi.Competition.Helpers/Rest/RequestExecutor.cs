using RestSharp;
using System.Net;
using System.Threading.Tasks;

namespace Tamagotchi.Competition.Helpers.Rest
{
    public static class RequestExecutor
    {
        public static async Task<string> ExecuteRequest(string address, IRestRequest request)
        {
            var cookieContainer = new CookieContainer();
            var client = new RestClient(address)
            {
                CookieContainer = cookieContainer
            };
            var response = await client.ExecuteTaskAsync((RestRequest)request);
            return response.Content;
        }

    }
}
