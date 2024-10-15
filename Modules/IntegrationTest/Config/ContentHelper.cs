using Infra.CrossCutting.Controllers;
using Infra.CrossCutting.UoW.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTest.Config
{
    public static class ContentHelper<T>
    {
        public static StringContent FormatStringContent(object obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.Default, "application/json");
        }

        public static async Task<Result<T>> GetResponse(HttpResponseMessage response)
        {
            var stringResponse = await response.Content.ReadAsStringAsync();
            //if(response.RequestMessage.RequestUri.ToString().Contains("auth/profile"))
            //{
            //    stringResponse = stringResponse.Replace("_", "");
            //}
            var result = JsonConvert.DeserializeObject<Result<T>>(stringResponse);
            return result;
        }


        
    }
}
