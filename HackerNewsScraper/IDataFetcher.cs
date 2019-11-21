using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HackerNewsScraper.Models;

namespace HackerNewsScraper
{
    public interface IDataFetcher
    {
        Task<int[]> GetTopStoryIds();
        Task<SourceItem> GetPostItem(int storyId);
    }
}
