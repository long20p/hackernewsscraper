using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using HackerNewsScraper.Models;

namespace HackerNewsScraper
{
    public class DataFetcher : IDataFetcher
    {
        private string rootUrl;
        private HttpClient httpClient;

        public DataFetcher(string rootUrl)
        {
            this.rootUrl = rootUrl;
            httpClient = new HttpClient();
        }

        public async Task<int[]> GetTopStoryIds()
        {
            var response = await httpClient.GetStringAsync($"{rootUrl}/topstories.json");
            return JsonConvert.DeserializeObject<int[]>(response);
        }

        public async Task<SourceItem> GetPostItem(int storyId)
        {
            var response = await httpClient.GetStringAsync($"{rootUrl}/item/{storyId}.json");
            return JsonConvert.DeserializeObject<SourceItem>(response);
        }
    }
}
