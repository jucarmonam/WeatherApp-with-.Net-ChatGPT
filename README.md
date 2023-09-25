# WeatherApp

WeatherApp is a .NET Core web application that allows users to retrieve weather information for cities. It utilizes an external weather API to fetch current weather data and stores it in a local database for quick access.

## Features

- Retrieve current weather data for a specific city.
- Store weather data locally to reduce API calls.
- Periodically update weather data for stored cities.

## Technologies Used

- .NET Core 5
- Entity Framework Core for database management
- HttpClient for making API requests
- Razor Pages for the web interface
- MySQL database for storing weather data
- Hosted background service for data updates

## Installation

1. Clone this repository to your local machine:

   ```bash
   git clone https://github.com/yourusername/WeatherApp.git
