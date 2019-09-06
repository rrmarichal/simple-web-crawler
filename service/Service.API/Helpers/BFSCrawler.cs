using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using CrawlerService.Controllers;
using CrawlerService.Models;
using Microsoft.Extensions.Logging;

namespace CrawlerService.Helpers {

	/// <summary>
	/// Crawler implementation using a breath first search expansion from the source URL.
	/// </summary>
	public class BFSCrawler : ICrawler {

		private readonly ILogger<CrawlController> logger;
		private readonly IContentProvider provider;

		/// <summary>
		/// Provide (inject) an abstraction to the actual content provider so we can mock
		/// up and test crawling functionality.
		/// </summary>
		public BFSCrawler(ILogger<CrawlController> logger, IContentProvider provider) {
			this.logger = logger;
			this.provider = provider;
		}

		/// <summary>
		/// Spawn tasks to crawl each level of the tree at the time. Maintain a list
		/// of previously found URLs to avoid infinite loops.
		/// </summary>
		public CrawlingResult Crawl(CrawlerTaskInfo taskInfo) {
			// baseUrl is validated, so it's safe to create an Uri instance from it.
			var baseUri = new Uri(taskInfo.BaseUrl);
			var domainUri = new UriBuilder(baseUri.Scheme, baseUri.Host).Uri;
			// sitemap is the root of the hierarchy.
			var sitemap = new NodeInfo(baseUri);
			var queue = new List<NodeInfo> { sitemap };
			var paths = new HashSet<string> { baseUri.AbsolutePath };
			var depth = 0;
			// MaxDepth equals zero means all the way down in the tree.
			while ((taskInfo.MaxDepth == 0 || ++depth <= taskInfo.MaxDepth) && queue.Count > 0) {
				// Spawn a thread (task) for each node in this level.
				var tasks = new List<Task<string>>();
				foreach (var node in queue) {
					tasks.Add(Task<string>.Run(() => CrawlNode(node)));
				}
				Task.WaitAll(tasks.ToArray());
				var next = new List<NodeInfo>();
				for (var j = 0; j < queue.Count; j++) {
					if (tasks[j].Result != null) {
						var nodeUris = Utils.GetAbsoluteUrisFromHtml(domainUri, tasks[j].Result);
						var childrenUris = new HashSet<Uri>(nodeUris);
						foreach (var uri in childrenUris) {
							var child = new NodeInfo(uri);
							queue[j].AddChild(child);
							// Add it to the next iteration iff it hasn't been found and it
							// is in the same domain.
							if (domainUri.Host == uri.Host && !paths.Contains(uri.AbsolutePath)) {
								paths.Add(uri.AbsolutePath);
								next.Add(child);
							}
						}
					}
				}
				queue = next;
			}
			return new CrawlingResult { Sitemap = sitemap };
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
