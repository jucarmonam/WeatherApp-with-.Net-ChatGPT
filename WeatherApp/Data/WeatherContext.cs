﻿using Microsoft.EntityFrameworkCore;
using WeatherApp.Models;

namespace WeatherApp.Data
{
    public class WeatherContext : DbContext
    {
        public WeatherContext(DbContextOptions<WeatherContext> options) : base(options)
        {
        }

        public DbSet<WeatherData> WeatherData { get; set; }
    }
}
