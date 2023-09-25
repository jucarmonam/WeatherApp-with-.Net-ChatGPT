# WeatherApp

WeatherApp is a .NET Core web application that allows users to retrieve weather information for cities. It utilizes an external weather API to fetch current weather data and stores it in a local database for quick access.

## Features

- Retrieve current weather data for a specific city.
- Store weather data locally to reduce API calls.
- Periodically update weather data for stored cities.

## Technologies Used

- .NET Core 7
- Entity Framework Core for database management
- HttpClient for making API requests
- Razor Pages for the web interface
- MySQL database for storing weather data
- Hosted background service for data updates

## Installation

1. Clone this repository to your local machine:

   ```bash
   git clone https://github.com/jucarmonam/WeatherApp-with-.Net-ChatGPT.git

2. Navigate to the project directory (yes twice):
   
   ```bash
   cd WeatherApp
   cd WeatherApp

3. Create or edit the appsettings.json file in the project root and configure your MySQL connection string. Replace <your_connection_string> with your MySQL database connection details:
    ```bash
    {
     "ConnectionStrings": {
       "DefaultConnection": "YourConnectionString"
     },
     "WeatherApi": {
       "ApiKey": "YourApiKey"
     }
   }

4. Install the required packages and dependencies::
   ```bash
     dotnet restore

5. apply database migrations:
   ```bash
     dotnet ef database update

## Usage

1. Run the application:
   ```bash
     dotnet run
   
2. Open your web browser and navigate to http://localhost:5020 to access the WeatherApp.

3. Enter a city name to retrieve weather data or explore the available features.

## Background Data Updates

The WeatherApp includes a background service that periodically updates weather data for stored cities. This service is automatically started when you run the application and executes every 5 minutes.

## short feedback
- was it easy to complete the task using AI?:
In some cases, using the AI was really useful to help generate code and configuration, but sometimes the steps that the AI gave me weren't complete, what I mean is that chatgpt forgot to tell me some previous configuration or packages that I needed to have previously installed.
- How long did task take you to complete?
around 16 hours
- Was the code ready to run after generation? What did you have to change to make it usable?
Most of the time, the code provided wasn't really functional, for example, the unit tests I had to change a lot to make them work.
- Which challenges did you face during completion of the task?
thougth the solution that chatgpt gave me was really useful, I had to be really cautious, test the things, see why it didn't work, change it and things like that. In the end I think I could have done it faster without the help of chatgpt. But chatGPT was also really helpful to solve some issues.
- Which specific prompts you learned as a good practice to complete the task?
Give him a role, be clear and concise, and provide feedback and ask him to give me prompts to ask.
