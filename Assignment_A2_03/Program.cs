using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Assignment_A2_03.Models;
using Assignment_A2_03.Services;

namespace Assignment_A2_03
{
    class Program
    {
        static async Task Main(string[] args)
        {
            NewsService service = new NewsService();
            service.NewsAvailable += NewsHandler;
            service.CachedNewsAvailable += CachedNewsHandler;
            //business, entertainment, general, health, science, sports, technology
            Task<News>[] tasks = { null, null, null, null, null, null, null, null, null, null, null, null, null, null };
            AggregateException exception = null;
            try
            {

                tasks[0] = service.GetNewsAsync(NewsCategory.business);
                tasks[1] = service.GetNewsAsync(NewsCategory.entertainment);
                tasks[2] = service.GetNewsAsync(NewsCategory.science);
                tasks[3] = service.GetNewsAsync(NewsCategory.general);
                tasks[4] = service.GetNewsAsync(NewsCategory.health);
                tasks[5] = service.GetNewsAsync(NewsCategory.sports);
                tasks[6] = service.GetNewsAsync(NewsCategory.technology);
                Task.WaitAll(tasks[0], tasks[1], tasks[2], tasks[3], tasks[4], tasks[5], tasks[6]);
                //string TimeNow = DateTime.Now.ToString("yyyy - MM - dd HH: mm");
                tasks[7] = service.GetNewsAsync(NewsCategory.business);
                tasks[8] = service.GetNewsAsync(NewsCategory.entertainment);
                tasks[9] = service.GetNewsAsync(NewsCategory.science);
                tasks[10] = service.GetNewsAsync(NewsCategory.general);
                tasks[11] = service.GetNewsAsync(NewsCategory.health);
                tasks[12] = service.GetNewsAsync(NewsCategory.sports);
                tasks[13] = service.GetNewsAsync(NewsCategory.technology);
                Task.WaitAll(tasks[7], tasks[8], tasks[9], tasks[10], tasks[11], tasks[12], tasks[13]);

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
                    Console.WriteLine(new string('-', 100) + $"\nNews in category {task.Result.Category}");
                    foreach (var item in task.Result.Articles.OrderBy(x => x.DateTime))
                    {
                        Console.Out.WriteLine($"   - {item.DateTime}: {item.Title}");
                    }
                }
                else
                {
                    TaskCount++;
                    await Console.Out.WriteLineAsync(new string('-', 100) + $"\nNews service error\n{exception.Message}");
                }
            }

            await Console.Out.WriteLineAsync(new string('-', 100) + $"\nNumber of failed tasks: {TaskCount}");




        }
        static public void NewsHandler<T>(object sender, T packet)
        {
            Console.WriteLine($"Event message from news service: News in category is available: {packet}");
        }
        static public void CachedNewsHandler<T>(object sender, T packet)
        {
            Console.WriteLine($"Event message from news service: Cached News in category is available: {packet}");
        }
    }
}