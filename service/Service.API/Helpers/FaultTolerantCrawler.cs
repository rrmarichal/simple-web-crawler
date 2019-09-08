using CrawlerService.Controllers;
using CrawlerService.Models;
using Microsoft.Extensions.Logging;

namespace CrawlerService.Helpers {

	/// <summary>
	/// Crawler implementation that uses a DFS approach with recovery/retries/state management.
	/// </summary>
	public class FaultTolerantCrawler : ICrawlerStrategy {

		private readonly ILogger<CrawlController> logger;
		private readonly IContentProvider provider;

		/// <summary>
		/// Provide (inject) an abstraction to the actual content provider so we can mock
		/// up and test crawling functionality.
		/// </summary>
		public FaultTolerantCrawler(ILogger<CrawlController> logger, IContentProvider provider) {
			this.logger = logger;
			this.provider = provider;
		}

		/// <summary>
		/// Build the sitemap tree structure using a top-down DFS approach.
		/// Account for crash recovery, state persistance, retrial policies.
		/// </summary>
		public CrawlingResult Crawl(CrawlerTaskInfo taskInfo) {
			throw new System.NotImplementedException();
		}

	}

}
