namespace WeatherApp.Services
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using System;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using WeatherApp.Data;
    using WeatherApp.Models;
    using WeatherApp.Services;

    public class WeatherDataUpdater : IHostedService, IDisposable
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<WeatherDataUpdater> _logger;
        private Timer _timer;

        public WeatherDataUpdater(IServiceProvider services, ILogger<WeatherDataUpdater> logger)
        {
            _services = services;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("WeatherDataUpdater is starting.");

            // Set up a timer to run the update task periodically
            _timer = new Timer(UpdateWeatherDataCallback, null, TimeSpan.Zero, TimeSpan.FromMinutes(5)); // Update every 5 minutes

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("WeatherDataUpdater is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private void UpdateWeatherDataCallback(object state)
        {
            UpdateWeatherData(state).Wait(); // You can use .Wait() here to run the async method synchronously in this context
        }

        private async Task UpdateWeatherData(object state)
        {
            try
            {
                using var scope = _services.CreateScope();
                var weatherService = scope.ServiceProvider.GetRequiredService<WeatherServiceClient>();
                IEnumerable<string> citiesToUpdate = await weatherService.GetAllCities(); // Add the cities you want to update

                foreach (var city in citiesToUpdate)
                {
                    var weatherData = await weatherService.GetWeatherByCity(city, true);
                    if (weatherData != null)
                    {
                        // Update the database with the fetched weather data
                        await UpdateWeatherDataInDatabase(weatherData);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating weather data.");
            }
        }

        private async Task UpdateWeatherDataInDatabase(WeatherData updatedWeatherData)
        {
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<WeatherContext>();

            var existingWeatherData = await dbContext.WeatherData.FirstOrDefaultAsync(w => w.City == updatedWeatherData.City);

            if (existingWeatherData != null)
            {
                // Update the properties of the existing record
                existingWeatherData.Date = updatedWeatherData.Date;
                existingWeatherData.Temperature = updatedWeatherData.Temperature;
                existingWeatherData.Humidity = updatedWeatherData.Humidity;

                // Save the changes to the database
                await dbContext.SaveChangesAsync();
            }
        }

    public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
