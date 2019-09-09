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
			// baseUrl is validated in the controller,
			// so it's safe to create an Uri instance from it.
			var root = new Uri(taskInfo.BaseUrl);
			var domain = new UriBuilder(root.Scheme, root.Host).Uri;
			// sitemap is the root of the hierarchy.
			var sitemap = new NodeInfo(root);
			paths = new HashSet<string>() { root.AbsolutePath };
			DfsAsync(sitemap, 0, taskInfo.MaxDepth, domain);
			return new CrawlingResult { Sitemap = sitemap };
		}

		private void DfsAsync(NodeInfo current, int depth, int maxDepth, Uri domainUri) {
			logger.LogInformation("DfsAsync: URL: {0} Depth: {1} Thread ID: {2}", current.Url, depth, Thread.CurrentThread.ManagedThreadId);
			var childrenTasks = new List<Task>();
			if (maxDepth == 0 || depth < maxDepth) {
				var result = CrawlNode(current);
				if (result != null) {
					var nodeUris = Utils.GetAbsoluteUrisFromHtml(domainUri, result);
					// Make Uris unique by inserting them into a set structure.
					var childrenUris = new HashSet<Uri>(nodeUris);
					foreach (var uri in childrenUris) {
						var child = new NodeInfo(uri);
						current.AddChild(child);
						if (domainUri.Host == uri.Host) {
							lock (locker) {
								if (!paths.Contains(uri.AbsolutePath)) {
									childrenTasks.Add(Task.Run(() => DfsAsync(child, depth + 1, maxDepth, domainUri)));
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
