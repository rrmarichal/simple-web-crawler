using System;
using NUnit.Framework;
using CrawlerService.Helpers;
using CrawlerService.Controllers;
using Microsoft.Extensions.Logging.Abstractions;

namespace CrawlerService.Tests {

	public class DFSCrawlerTests {

		[Test]
		public void SimpleCrawlingTest() {
			var taskInfo = new CrawlerTaskInfo("http://www.fiboapp.com/", 0);
			var crawler = new DFSCrawler(
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

		[Test]
		public void RandomCrawlingTest() {
			var taskInfo = new CrawlerTaskInfo("http://www.fiboapp.com/", 4);
			var crawler = new DFSCrawler(
				new NullLogger<CrawlController>(),
				new RandomMockContentProvider()
			);
			var result = crawler.Crawl(taskInfo);
			Assert.IsNull(result.Error);
			Assert.IsNotNull(result.Sitemap);
			Assert.AreEqual(result.Sitemap.Url, "http://www.fiboapp.com/");
		}

	}

}
