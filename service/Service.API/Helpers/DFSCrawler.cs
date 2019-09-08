using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CrawlerService.Controllers;
using CrawlerService.Models;
using Microsoft.Extensions.Logging;

namespace CrawlerService.Helpers {

	/// <summary>
	/// Crawler implementation using a depth first search expansion from the source URL.
	/// </summary>
	public class DFSCrawler : ICrawlerStrategy {

		private readonly ILogger<CrawlController> logger;
		private readonly IContentProvider provider;
		private readonly object locker;
		private HashSet<string> paths;

		/// <summary>
		/// Provide (inject) an abstraction to the actual content provider so we can mock
		/// up and test crawling functionality.
		/// </summary>
		public DFSCrawler(ILogger<CrawlController> logger, IContentProvider provider) {
			this.logger = logger;
			this.provider = provider;
			locker = new object();
		}

		/// <summary>
		/// Build the sitemap tree structure using a top-down DFS approach.
		/// Thread safety is at the level class.
		/// </summary>
		public CrawlingResult Crawl(CrawlerTaskInfo taskInfo) {
			// baseUrl is validated, so it's safe to create an Uri instance from it.
			var baseUri = new Uri(taskInfo.BaseUrl);
			var domainUri = new UriBuilder(baseUri.Scheme, baseUri.Host).Uri;
			// sitemap is the root of the hierarchy.
			var sitemap = new NodeInfo(baseUri);
			paths = new HashSet<string>();
			AsyncDfs(sitemap, 0, taskInfo.MaxDepth, domainUri);
			return new CrawlingResult { Sitemap = sitemap };
		}

		private void AsyncDfs(NodeInfo current, int depth, int maxDepth, Uri domainUri) {
			logger.LogInformation("DfsAsync: URL: {0} Depth: {1} Thread ID: {2}", current.Url, depth, Thread.CurrentThread.ManagedThreadId);
			var childrenTasks = new List<Task>();
			if (maxDepth == 0 || depth < maxDepth) {
				var result = CrawlNode(current);
				if (result != null) {
					var nodeUris = Utils.GetAbsoluteUrisFromHtml(domainUri, result);
					var childrenUris = new HashSet<Uri>(nodeUris);
					foreach (var uri in childrenUris) {
						var child = new NodeInfo(uri);
						current.AddChild(child);
						if (domainUri.Host == uri.Host) {
							lock (locker) {
								if (!paths.Contains(uri.AbsolutePath)) {
									childrenTasks.Add(Task.Run(() => AsyncDfs(child, depth + 1, maxDepth, domainUri)));
									paths.Add(uri.AbsolutePath);
								}
							}
						}
					}
				}
			}
			Task.WaitAll(childrenTasks.ToArray());
		}

		private string CrawlNode(NodeInfo nodeInfo) {
			try {
				return provider.GetHtmlContent(nodeInfo.Url);
			}
			catch (Exception e) {
				logger.LogError(e, e.Message);
				// In production, some tracing should be added to the backlog in order
				// to provide more context on the overall task execution, i.e., malformed URLs,
				// timeouts, etc.
				// Handling retries is also important on a production environment, will keep that out
				// in this implementation.
				return null;
			}
		}
		
	}

}
