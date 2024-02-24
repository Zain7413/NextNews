using Newtonsoft.Json;

namespace NextNews.Models
{
    public class Wheather
    {


        [JsonProperty("summary")]
        public string Summary { get; set; } = string.Empty;

        [JsonProperty("city")]
        public string City { get; set; } = string.Empty;


        [JsonProperty("lang")]
        public object Langauage { get; set; } = string.Empty;

        [JsonProperty("temperatureC")]
        public int TemperatureCelsius { get; set; }


        [JsonProperty("temperatureF")]
        public int TemperatureFahrenheit { get; set; }


        [JsonProperty("humidity")]
        public int humidity { get; set; }


        [JsonProperty("windSpeed")]
        public int windSpeed { get; set; }


        [JsonProperty("date")]
        public DateTime date { get; set; }


        [JsonProperty("unixTime")]
        public int unixTime { get; set; }

        public Icon icon { get; set; }


    }

    public class Icon
    {
        public string url { get; set; } = string.Empty;
        public string code { get; set; } = string.Empty;
    }

}

