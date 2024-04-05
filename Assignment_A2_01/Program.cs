using System;

using Assignment_A2_01.Models;
using Assignment_A2_01.Services;

namespace Assignment_A2_01
{
    class Program
    {
        static async Task Main(string[] args)
        {
            NewsService service = new NewsService();
            NewsApiData nd = await service.GetNewsAsync();

            //printing top headlines
            await Console.Out.WriteLineAsync("Top headlines:");
            foreach (Article article in nd.Articles)
                await Console.Out.WriteLineAsync($" {article.Title}");
            
        }
    }
}
