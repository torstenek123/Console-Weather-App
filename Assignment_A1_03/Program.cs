using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

using Assignment_A1_03.Models;
using Assignment_A1_03.Services;

namespace Assignment_A1_03
{
    class Program
    {
        static async Task Main(string[] args)
        {
            OpenWeatherService service = new OpenWeatherService();

            service.WeatherForecastAvailable += WeatherHandler;
            service.CachedEvent += CacheHandler;

            Task<Forecast>[] tasks = { null, null, null, null };
            AggregateException exception = null;
            try
            {
                double latitude = 59.5086798659495;
                double longitude = 18.2654625932976;

                //Create the two tasks and wait for completion
                tasks[0] = service.GetForecastAsync(latitude, longitude);
                tasks[1] = service.GetForecastAsync("Miami");

                Task.WaitAll(tasks[0], tasks[1]);

                tasks[2] = service.GetForecastAsync(latitude, longitude);
                tasks[3] = service.GetForecastAsync("Miami");

                //Wait and confirm we get an event showing cahced data avaialable
                Task.WaitAll(tasks[2], tasks[3]);
            }
            catch (AggregateException ex)
            {
                exception = ex;
            }

            //Handling tasks
            int TaskCount = 0;

            foreach (var task in tasks)
            {   
                //Check if task caught exception or not
                if (task.Exception == null)
                {
                    Console.WriteLine(new string('-', 100) + $"\nWeather forecast for {task.Result.City}");
                    foreach (var item in task.Result.Items.OrderBy(x => x.DateTime))
                    {
                        await Console.Out.WriteLineAsync($"{item.DateTime} : {item.Description}, temperature: {item.Temperature} degC, wind: {item.WindSpeed} m/s ");
                    }
                }
                else
                {
                    TaskCount++;
                    await Console.Out.WriteLineAsync(new string('-', 100) + $"\nCity weather service error\n{exception.Message}");
                }
            }

            await Console.Out.WriteLineAsync(new string('-', 100) + $"\nNumber of failed tasks: {TaskCount}");
        }


        static public void WeatherHandler<T>(object sender, T packet)
        {
            Console.WriteLine($"Event message from weather service: New weather forecast for {packet} available");
        }
        static public void CacheHandler<T>(object sender, T packet)
        {
            Console.WriteLine($"Event message from weather service: Cached forecast for {packet} available");
        }
    }
}
