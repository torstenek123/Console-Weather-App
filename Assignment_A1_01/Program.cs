using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

using Assignment_A1_01.Models;
using Assignment_A1_01.Services;

namespace Assignment_A1_01
{
    class Program
    {
        static async Task Main(string[] args)
        {
            double latitude = 59.5086798659495;
            double longitude = 18.2654625932976;

            Forecast forecast = await new OpenWeatherService().GetForecastAsync(latitude, longitude);

            //Your Code to present each forecast item in a grouped list
            Console.WriteLine($"Weather forecast for {forecast.City}");
            foreach(var item in forecast.Items.OrderBy(x => x.DateTime)) 
            {
                Console.WriteLine($"{item.DateTime} : {item.Description}, temperature: {item.Temperature} degC, wind: {item.WindSpeed} m/s ");

            }
        }
    }
}
