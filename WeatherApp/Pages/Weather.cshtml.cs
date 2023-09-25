using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.Pages
{
    public class WeatherModel : PageModel
    {
        private readonly WeatherServiceClient _weatherService;

        public WeatherModel(WeatherServiceClient weatherService)
        {
            _weatherService = weatherService;
        }

        [BindProperty(SupportsGet = true)] // Bind the 'city' parameter from query string
        public string City { get; set; }

        public WeatherData WeatherData { get; set; }

        public async Task<IActionResult> OnGet()
        {
            if (!string.IsNullOrWhiteSpace(City))
            {
                try
                {
                    // Implement logic to retrieve weather data by city using your WeatherService
                    WeatherData = await _weatherService.GetWeatherByCity(City);

                    if (WeatherData == null)
                    {
                        // Weather data not found
                        return NotFound();
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., network errors, data not available)
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }
            }

            return Page();
        }
    }
}
