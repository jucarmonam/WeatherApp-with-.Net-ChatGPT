using Microsoft.EntityFrameworkCore;
using WeatherApp;
using WeatherApp.Data;
using WeatherApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<WeatherContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                     new MySqlServerVersion(new Version(8, 0, 26)));
});

//Add ApiKey
builder.Services.Configure<WeatherApiSettings>(builder.Configuration.GetSection("WeatherApi"));

// Add HttpClient and register WeatherServiceClient this is dependency injection because when we create an instace of the service client
// we don't need to pass it the HttpCLiente parameter
builder.Services.AddScoped<WeatherServiceClient>();
builder.Services.AddHttpClient();

// Register the WeatherDataUpdater as a hosted service
builder.Services.AddHostedService<WeatherDataUpdater>();

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapGet("/", () =>
{
    // This is the top-level route registration for the root URL
    // You can specify what should happen when the root URL is accessed
    return Results.Redirect("/Weather"); // Redirect to the Weather Razor Page
});

app.MapRazorPages();

app.Run();