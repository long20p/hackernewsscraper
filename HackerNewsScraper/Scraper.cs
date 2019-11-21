using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HackerNewsScraper.Models;

namespace HackerNewsScraper
{
    /// <summary>
    /// Takes care of fetching and processing story objects
    /// </summary>
    public class Scraper
    {
        private static readonly string[] AllowedUriSchemes = { Uri.UriSchemeHttp, Uri.UriSchemeHttps };
        private IDataFetcher dataFetcher;
        private SemaphoreSlim semaphore;

        public Scraper(IDataFetcher dataFetcher, int maxRequestParallelism)
        {
            this.dataFetcher = dataFetcher;
            semaphore = new SemaphoreSlim(maxRequestParallelism);
        }

        /// <summary>
        /// Gets IDs of top stories
        /// </summary>
        /// <param name="count">Defines how many IDs from the top will be returned</param>
        /// <returns>List of objects containing story ID and its rank</returns>
        public async Task<List<RankedStoryId>> GetTopStoryIds(int count)
        {
            var allIds = await dataFetcher.GetTopStoryIds();
            return allIds
                .Take(Math.Min(count, allIds.Length))
                .Select((id, index) => new RankedStoryId { StoryId = id, Rank = index + 1 })
                .ToList();
        }

        /// <summary>
        /// Gets story by ID
        /// </summary>
        /// <param name="rankedStoryId">Object containing story ID and its rank</param>
        /// <returns>A story</returns>
        public async Task<Story> GetStory(RankedStoryId rankedStoryId)
        {
            var item = await dataFetcher.GetPostItem(rankedStoryId.StoryId);

            if (!Uri.TryCreate(item.Url, UriKind.Absolute, out var uri) || !uri.IsWellFormedOriginalString() || !AllowedUriSchemes.Contains(uri.Scheme))
            {
                throw new InvalidPostItemPropertyException("Post URL is not valid");
            }

            ValidatePostItemText(item.Title, "Title");
            ValidatePostItemText(item.By, "Author name");
            ValidatePostItemNumber(item.Score, "Story points");
            ValidatePostItemNumber(item.Descendants, "Comment count");

            var story = new Story
            {
                Title = item.Title,
                Uri = item.Url,
                Author = item.By,
                Points = item.Score,
                Comments = item.Descendants,
                Rank = rankedStoryId.Rank
            };
            return story;
        }

        /// <summary>
        /// Gets top stories
        /// </summary>
        /// <param name="count">Defines how many stories will be fetched</param>
        /// <returns>List of stories</returns>
        public async Task<List<Story>> GetTopStories(int count)
        {
            var topStoryIds = await GetTopStoryIds(count);

            var stories = new List<Story>();

            // create a list of tasks - each task fetches a story
            var tasks = topStoryIds.Select(async storyId =>
            {
                // wait here if semaphore is full
                await semaphore.WaitAsync();
                try
                {
                    var story = await GetStory(storyId);
                    stories.Add(story);
                }
                catch(Exception)
                {
                    //Logging?
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);

            return stories.OrderBy(x => x.Rank).ToList();
        }

        private void ValidatePostItemText(string text, string itemName)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new InvalidPostItemPropertyException($"{itemName} is empty");
            }
            if(text.Length > 256)
            {
                throw new InvalidPostItemPropertyException($"{itemName} is longer than 256 chracters");
            }
        }

        private void ValidatePostItemNumber(int number, string itemName)
        {
            if (number <= 0)
            {
                throw new InvalidPostItemPropertyException($"{itemName} must be greater than 0");
            }
        }
    }
}
