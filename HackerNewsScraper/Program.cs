using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace HackerNewsScraper
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            if(args.Length < 2 || !ValidateArguments(args[0], args[1], out var postCount))
            {
                Console.WriteLine(@"No arguments were provided or arguments are invalid.

Usage: dotnet HackerNewsScraper.dll --posts N

        --posts     How many posts to print. N is a positive integer <= 100.");
                return;
            }

            var config = InitializeConfig();
            var rootUrl = config["rootUrl"];
            var maxRequestParallelism = int.Parse(config["maxRequestParallelism"]);

            var scraper = new Scraper(new DataFetcher(rootUrl), maxRequestParallelism);
            var topStories = await scraper.GetTopStories(postCount);

            Console.WriteLine(JsonConvert.SerializeObject(topStories, Formatting.Indented));
        }

        static bool ValidateArguments(string first, string second, out int postCount)
        {
            postCount = -1;
            return first == "--posts" && int.TryParse(second, out postCount) && postCount >= 0 && postCount <= 100;
        }

        static IConfiguration InitializeConfig()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("settings.json", true, true)
                .Build();
        }
    }
}
