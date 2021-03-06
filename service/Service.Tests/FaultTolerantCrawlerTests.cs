using NUnit.Framework;
using CrawlerService.Helpers;
using CrawlerService.Controllers;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace CrawlerService.Tests {

	public class FaultTolerantCrawlerTests {

		[Test]
		public void SimpleCrawlingTest() {
			var taskInfo = new CrawlerTaskInfo("http://www.fiboapp.com/", 0);
			var configuration = new Mock<IConfiguration>();

			// QueueProcessorConcurrencyLevel
			var concurrencyLevelConfigurationSection = new Mock<IConfigurationSection>();
			concurrencyLevelConfigurationSection.Setup(a => a.Value).Returns("10");
			configuration
				.Setup(a => a.GetSection("QueueProcessorConcurrencyLevel"))
				.Returns(concurrencyLevelConfigurationSection.Object);

			// FailuresMaxRetries
			var maxRetriesConfigurationSection = new Mock<IConfigurationSection>();
			maxRetriesConfigurationSection.Setup(a => a.Value).Returns("5");
			configuration
				.Setup(a => a.GetSection("FailuresMaxRetries"))
				.Returns(maxRetriesConfigurationSection.Object);

			// QueueIdleTime
			var idleTimeConfigurationSection = new Mock<IConfigurationSection>();
			idleTimeConfigurationSection.Setup(a => a.Value).Returns("500");
			configuration
				.Setup(a => a.GetSection("QueueIdleTime"))
				.Returns(idleTimeConfigurationSection.Object);

			var crawler = new FaultTolerantCrawler(
				configuration.Object,
				new NullLogger<CrawlController>(),
				new SimpleMockContentProvider(taskInfo.BaseUrl)
			);
			var result = crawler.Crawl(taskInfo);
			Assert.IsNull(result.Error);
			Assert.IsNotNull(result.Sitemap);
			Assert.AreEqual(result.Sitemap.Url, "http://www.fiboapp.com/");
			Assert.IsNotNull(result.Sitemap.Children);
			Assert.AreEqual(3, result.Sitemap.Children.Count);
			Assert.AreEqual("http://www.fiboapp.com/careers", result.Sitemap.Children[0].Url);
			Assert.AreEqual("http://www.fiboapp.com/about", result.Sitemap.Children[1].Url);
			Assert.AreEqual("https://google.com/accounts", result.Sitemap.Children[2].Url);
		}

	}
	
}
