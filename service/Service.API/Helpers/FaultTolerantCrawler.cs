using System;
using System.Collections.Concurrent;
using CrawlerService.Controllers;
using CrawlerService.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CrawlerService.Helpers {

	/// <summary>
	/// Crawler implementation that uses a DFS approach with recovery/retries/state management.
	/// </summary>
	public class FaultTolerantCrawler : ICrawlerStrategy {

		private readonly IConfiguration configuration;
		private readonly ILogger<CrawlController> logger;
		private readonly IContentProvider provider;

		/// <summary>
		/// Initializes a new instance of the class FaultTolerantCrawler.
		/// </summary>
		public FaultTolerantCrawler(
			IConfiguration configuration,
			ILogger<CrawlController> logger,
			IContentProvider provider
		) {
			this.configuration = configuration;
			this.logger = logger;
			this.provider = provider;
		}

		/// <summary>
		/// Build the sitemap tree structure using a top-down DFS approach using a supervisor
		/// that controls concurrency and accounts for error recovery and
		/// retrials.
		/// </summary>
		public CrawlingResult Crawl(CrawlerTaskInfo taskInfo) {
			// baseUrl is validated in the controller,
			// so it's safe to create an Uri instance from it.
			var root = new Uri(taskInfo.BaseUrl);
			// sitemap is the root of the hierarchy.
			var sitemap = new NodeInfo(root);
			var queue = new ConcurrentQueue<NodeInfo>();
			queue.Enqueue(sitemap);
			var queueProcessor = new QueueProcessor(
				provider,
				logger,
				root,
				taskInfo.MaxDepth,
				queue,
				configuration.GetValue<int>("QueueProcessorConcurrencyLevel"),
				configuration.GetValue<int>("FailuresMaxRetries"),
				configuration.GetValue<int>("QueueIdleTime")
			);
			queueProcessor.Start();
			return new CrawlingResult { Sitemap = sitemap };
		}

	}
	
}
