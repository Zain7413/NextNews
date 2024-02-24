using Azure;
using Newtonsoft.Json;
using NextNews.Models;
using NextNews.ViewComponents;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NextNews.Services
{
    public class WeatherService : IWheatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("forecast");
          
        }

       

        public async Task<Wheather> GetWeatherReport(string chosenCity)
        {
            var client = new HttpClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"http://weatherapi.dreammaker-it.se/forecast?city={chosenCity}&lang=en"),
            };
            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Wheather>(content) ?? new Wheather() { Summary = "No data available" };
        }
       
    }
}
