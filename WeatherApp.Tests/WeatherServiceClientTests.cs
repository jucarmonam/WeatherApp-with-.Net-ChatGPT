using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using WeatherApp.Data;
using WeatherApp.Services;

using Moq.Protected;

namespace WeatherApp.Tests
{
    [TestClass]
    public class WeatherServiceClientTests
    {
        private WeatherContext _context;
        private DbContextOptions<WeatherContext> _options;

        [TestInitialize]
        public void Initialize()
        {
            // Create an in-memory database with a unique name
            _options = new DbContextOptionsBuilder<WeatherContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Initialize the database context
            _context = new WeatherContext(_options);

            // Ensure the database is created and seeded (if necessary)
            _context.Database.EnsureCreated();
        }

        [TestMethod]
        public async Task GetWeatherByCity_WithValidData_ReturnsWeatherData()
        {
            // Arrange
            var httpClientServiceMock = new Mock<IHttpClientFactory>();
            var optionsMock = new Mock<IOptions<WeatherApiSettings>>();
            // Create an instance of WeatherApiSettings with the API key you want to use
            var weatherApiSettings = new WeatherApiSettings
            {
                ApiKey = "your-api-key-here"
            };

            // Set up the optionsMock to return the WeatherApiSettings instance
            optionsMock.Setup(x => x.Value).Returns(weatherApiSettings);

            var sampleJson = "{ \"location\": { \"name\": \"SampleCity\" }, \"current\": { \"temp_c\": 20.0, \"humidity\": 50, \"last_updated\": \"2023-09-25 22:00\" } }";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(sampleJson),
            };

            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var httpClient = new HttpClient(handler.Object);
            httpClientServiceMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var service = new WeatherServiceClient(httpClientServiceMock.Object, optionsMock.Object, _context);

            // Act
            var result = await service.GetWeatherByCity("SampleCity");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("SampleCity", result.City);
            Assert.AreEqual(20.0, result.Temperature);
            Assert.AreEqual(50, result.Humidity);
        }

        // Add more test methods for other scenarios (e.g., invalid data, exceptions, etc.)

        [TestCleanup]
        public void Cleanup()
        {
            // Dispose of the database context and delete the in-memory database
            _context.Dispose();
        }
    }
}
