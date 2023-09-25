using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;
using WeatherApp.Data;
using WeatherApp.Models;

namespace WeatherApp.Services
{
    public class WeatherServiceClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey;
        private const string ApiBaseUrl = "https://api.weatherapi.com";
        private readonly WeatherContext _context;

        //Here we are receiving the parameter as construcotr injection from the dependecy injection of the startup.cs (program.cs)
        public WeatherServiceClient(IHttpClientFactory httpClientFactory, IOptions<WeatherApiSettings> apiSettings, WeatherContext context)
        {
            _httpClientFactory = httpClientFactory;
            _apiKey = apiSettings.Value.ApiKey;
            _context = context; // Initialize the WeatherContext
        }

        public async Task<IEnumerable<string>> GetAllCities()
        {
            var existingCities = await _context.WeatherData.Select(w => w.City).ToListAsync();
            if (!existingCities.Any())
            {
                return Enumerable.Empty<string>();
            }
            return existingCities;
        }

        public async Task<WeatherData?> GetWeatherByCity(string city, bool update = false)
        {
            using var httpClient = _httpClientFactory.CreateClient();
            var existingWeatherData = await GetWeatherDataFromDatabase(city);

            if (existingWeatherData != null && !update)
            {
                return existingWeatherData;
            }

            var jsonContent = await FetchWeatherDataFromAPI(city, httpClient);

            if (jsonContent == null)
            {
                return null;
            }

            var weatherData = ParseAndConvertToWeatherData(jsonContent);

            if (weatherData != null && !update)
            {
                await SaveWeatherDataAsync(weatherData);
            }

            return weatherData;
        }

        private async Task<WeatherData?> GetWeatherDataFromDatabase(string city)
        {
            var existingWeatherData = await _context.WeatherData.FirstOrDefaultAsync(w => w.City == city);
            return existingWeatherData;
        }

        private async Task<string?> FetchWeatherDataFromAPI(string city, HttpClient httpClient)
        {
            try
            {
                var apiUrl = $"{ApiBaseUrl}/v1/current.json?key={_apiKey}&q={city}&aqui=no";
                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Console.WriteLine($"API request failed with status code: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }


        private WeatherData? ParseAndConvertToWeatherData(string jsonContent)
        {
            try
            {
                var weatherResponse = JsonSerializer.Deserialize<WeatherResponse>(jsonContent);

                if (weatherResponse != null)
                {
                    return new WeatherData
                    {
                        City = weatherResponse.Location.Name,
                        Date = DateTime.Parse(weatherResponse.Current.LastUpdated),
                        Temperature = weatherResponse.Current.TempC,
                        Humidity = weatherResponse.Current.Humidity
                        // Map other properties as needed
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while parsing JSON: {ex.Message}");
                return null;
            }
        }

        public async Task SaveWeatherDataAsync(WeatherData weatherData)
        {
            var existingWeatherData = await _context.WeatherData.FirstOrDefaultAsync(w => w.City == weatherData.City);

            if (existingWeatherData != null)
            {
                // Update the existing record with the new weather data
                existingWeatherData.Date = weatherData.Date;
                existingWeatherData.Temperature = weatherData.Temperature;
                existingWeatherData.Humidity = weatherData.Humidity;
            }
            else
            {
                // Insert a new record for the city
                _context.WeatherData.Add(weatherData);
            }

            await _context.SaveChangesAsync();
        }

    }
}
