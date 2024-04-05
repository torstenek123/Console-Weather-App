using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Text.Json;

using Assignment_A1_03.Models;

namespace Assignment_A1_03.Services
{
    public class OpenWeatherService
    {
        HttpClient httpClient = new HttpClient();
        
        //Cache declaration
        ConcurrentDictionary<(double, double, string), Forecast> cachedGeoForecasts = new ConcurrentDictionary<(double, double, string), Forecast>();
        ConcurrentDictionary<(string, string), Forecast> cachedCityForecasts = new ConcurrentDictionary<(string, string), Forecast>();

        // Your API Key
        readonly string apiKey = "f86b8e5de4da7873c36cc9a5e4f05a93";

        //Event declaration
        public event EventHandler<string> WeatherForecastAvailable;
        public event EventHandler<string> CachedEvent;
        protected virtual void OnWeatherForecastAvailable (string message)
        {
            WeatherForecastAvailable?.Invoke(this, message);
        }
        public async Task<Forecast> GetForecastAsync(string City)
        {
            Forecast forecast;
            string TimeNow = DateTime.Now.ToString("yyyy - MM - dd HH: mm");
            //Checking if cached
            if (cachedCityForecasts.ContainsKey((TimeNow, City)))
            {
                forecast = cachedCityForecasts[(TimeNow, City)];
                CachedEvent?.Invoke(this, City);
            }
            //Caching
            else
            {
                var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={City}&units=metric&lang={language}&appid={apiKey}";

                forecast = await ReadWebApiAsync(uri);
                cachedCityForecasts.TryAdd((TimeNow, City), forecast);
                WeatherForecastAvailable?.Invoke(this, City);
            }
            return forecast;

        }
        public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
        {
            Forecast forecast;
            string TimeNow = DateTime.Now.ToString("yyyy - MM - dd HH: mm");
            //Checking if cached
            if (cachedGeoForecasts.ContainsKey((latitude, longitude, TimeNow)))
            {
                forecast = cachedGeoForecasts[(latitude, longitude, TimeNow)];
                CachedEvent?.Invoke(this, (latitude, longitude).ToString());
            }
            //Caching
            else
            {
                var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={apiKey}";

                forecast = await ReadWebApiAsync(uri);
                cachedGeoForecasts.TryAdd((latitude, longitude, TimeNow), forecast);
                WeatherForecastAvailable?.Invoke(this, (latitude, longitude).ToString());
            }

            return forecast;
        }
        private async Task<Forecast> ReadWebApiAsync(string uri)
        {
            //Read the response from the WebApi
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            WeatherApiData wd = await response.Content.ReadFromJsonAsync<WeatherApiData>();

            //Object creation
            var forecast = new Forecast();
            forecast.City = wd.city.name;
            forecast.Items = wd.list.Select(x => new ForecastItem { DateTime = UnixTimeStampToDateTime(x.dt), Temperature = x.main.temp, WindSpeed = x.wind.speed, Description = x.weather[0].description, Icon = $"https://openweathermap.org/img/w/{x.weather[0].icon}.png" }).ToList();
            return forecast;
        }
        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}
