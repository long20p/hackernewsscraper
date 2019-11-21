using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using HackerNewsScraper.Models;
using System.Linq;

namespace HackerNewsScraper.Tests
{
    public class ScraperTests
    {
        private Scraper scraper;
        private Mock<IDataFetcher> dataFetcher;
        private SourceItem postItem;

        [SetUp]
        public void Setup()
        {
            dataFetcher = new Mock<IDataFetcher>();
            scraper = new Scraper(dataFetcher.Object, 5);
            postItem = new SourceItem
            {
                By = "adams",
                Title = "hitchhiker's guide to the galaxy",
                Descendants = 10,
                Score = 123,
                Url = "http://thesite.com/2019/11/article"
            };
            dataFetcher.Setup(x => x.GetPostItem(It.IsAny<int>())).Returns(Task.FromResult(postItem));
        }

        [Test]
        public async Task GetTopStoryIdsReturnsExpectedIds()
        {
            dataFetcher.Setup(x => x.GetTopStoryIds()).Returns(Task.FromResult(new int[] { 42693, 6544, 79963, 456 }));
            var storyIds = await scraper.GetTopStoryIds(2);

            Assert.AreEqual(2, storyIds.Count);
            Assert.AreEqual(1, storyIds[0].Rank);
            Assert.AreEqual(42693, storyIds[0].StoryId);
            Assert.AreEqual(2, storyIds[1].Rank);
            Assert.AreEqual(6544, storyIds[1].StoryId);
        }

        [Test]
        public async Task ReturnStoryForValidSourceItem()
        {
            var story = await scraper.GetStory(new RankedStoryId { Rank = 5 });

            Assert.AreEqual("adams", story.Author);
            Assert.AreEqual("hitchhiker's guide to the galaxy", story.Title);
            Assert.AreEqual(10, story.Comments);
            Assert.AreEqual(123, story.Points);
            Assert.AreEqual("http://thesite.com/2019/11/article", story.Uri);
            Assert.AreEqual(5, story.Rank);
        }

        [TestCase("")]
        [TestCase(null)]
        public void ThrowExceptionWhenPostTitleIsEmpty(string title)
        {
            postItem.Title = title;

            Assert.That(async () => await scraper.GetStory(new RankedStoryId()),
                Throws.InstanceOf<InvalidPostItemPropertyException>().With.Message.EqualTo("Title is empty"));
        }

        [Test]
        public void ThrowExceptionWhenPostTitleIsTooLong()
        {
            postItem.Title = new string(Enumerable.Repeat('m', 260).ToArray());

            Assert.That(async () => await scraper.GetStory(new RankedStoryId()),
                Throws.InstanceOf<InvalidPostItemPropertyException>().With.Message.EqualTo("Title is longer than 256 chracters"));
        }

        [TestCase("")]
        [TestCase(null)]
        public void ThrowExceptionWhenPostAuthorIsEmpty(string author)
        {
            postItem.By = author;

            Assert.That(async () => await scraper.GetStory(new RankedStoryId()),
                Throws.InstanceOf<InvalidPostItemPropertyException>().With.Message.EqualTo("Author name is empty"));
        }

        [Test]
        public void ThrowExceptionWhenPostAuthorNameIsTooLong()
        {
            postItem.By = new string(Enumerable.Repeat('l', 257).ToArray());

            Assert.That(async () => await scraper.GetStory(new RankedStoryId()),
                Throws.InstanceOf<InvalidPostItemPropertyException>().With.Message.EqualTo("Author name is longer than 256 chracters"));
        }

        [TestCase(@"\\path\to\some\place")]
        [TestCase("htt://site.org/map")]
        [TestCase("NotAUrl")]
        public void ThrowExceptionWhenPostUriIsNotValid(string uri)
        {
            postItem.Url = uri;

            Assert.That(async () => await scraper.GetStory(new RankedStoryId()),
                Throws.InstanceOf<InvalidPostItemPropertyException>().With.Message.EqualTo("Post URL is not valid"));
        }

        [TestCase(0)]
        [TestCase(-3)]
        public void ThrowExceptionWhenPostScoreIsInvalid(int score)
        {
            postItem.Score = score;

            Assert.That(async () => await scraper.GetStory(new RankedStoryId()),
                Throws.InstanceOf<InvalidPostItemPropertyException>().With.Message.EqualTo("Story points must be greater than 0"));
        }

        [TestCase(0)]
        [TestCase(-74)]
        public void ThrowExceptionWhenCommentCountIsInvalid(int comments)
        {
            postItem.Descendants = comments;

            Assert.That(async () => await scraper.GetStory(new RankedStoryId()),
                Throws.InstanceOf<InvalidPostItemPropertyException>().With.Message.EqualTo("Comment count must be greater than 0"));
        }
    }
}
