using NextNews.Models;
using NextNews.ViewComponents;

namespace NextNews.Services
{
    public interface IWheatherService
    {
        Task<Wheather> GetWeatherReport(string chosenCity);

    }
}
