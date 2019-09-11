using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CrawlerService.Controllers;
using Microsoft.Extensions.Logging;

namespace CrawlerService.Helpers {

	internal class QueueProcessor {

		private readonly IContentProvider provider;
		private readonly ILogger<CrawlController> logger;
		private readonly Uri root, domain;
		private readonly int maxDepth, queueIdleTime, failuresMaxRetries;
		private readonly ConcurrentQueue<NodeInfo> queue;
		private readonly int concurrencyLevel;
		private HashSet<string> paths;
		private readonly object locker;
		private int processing;

		public QueueProcessor(
			IContentProvider provider,
			ILogger<CrawlController> logger,
			Uri root,
			int maxDepth,
			ConcurrentQueue<NodeInfo> queue,
			int concurrencyLevel,
			int failuresMaxRetries,
			int queueIdleTime
		) {
			this.provider = provider;
			this.logger = logger;
			this.root = root;
			this.maxDepth = maxDepth;
			this.queue = queue;
			this.concurrencyLevel = concurrencyLevel;
			this.failuresMaxRetries = failuresMaxRetries;
			this.queueIdleTime = queueIdleTime;
			locker = new object();
			domain = new UriBuilder(this.root.Scheme, this.root.Host).Uri;
		}

		internal void Start() {
			processing = 0;
			paths = new HashSet<string>() { root.AbsolutePath };
			while (!queue.IsEmpty || processing > 0) {
				logger.LogInformation($"Queue count: {queue.Count} - Processing: {processing}. Found: {paths.Count}");
				NodeInfo next;
				if (processing == concurrencyLevel || !queue.TryDequeue(out next)) {
					Thread.Sleep(queueIdleTime);
				}
				else {
					Interlocked.Add(ref processing, 1);
					Task.Run(() => CrawlNodeAsync(next));
				}
			}
		}

		private async Task CrawlNodeAsync(NodeInfo node) {
			try {
				var content = await Task.Run(() => provider.GetHtmlContent(node.Url));
				if (string.IsNullOrEmpty(content)) {
					EnqueueForRetry(node);
				}
				else {
					EnqueueNodeUrls(node, content);
				}
			}
			catch (WebException e) {
				logger.LogError(e, e.Message);
				// In production, some tracing should be added to the backlog in order
				// to provide more context on the overall task execution, i.e., malformed URLs,
				// timeouts, etc.
				EnqueueForRetry(node);
			}
			finally {
				Interlocked.Add(ref processing, -1);
			}
		}

		/// <summary>
		/// Retry on this node by re-queuing it if limit hasn't been reached.
		/// </summary>
		private void EnqueueForRetry(NodeInfo node) {
			node.AddRetry();
			if (node.Retries < failuresMaxRetries) {
				queue.Enqueue(node);
				logger.LogInformation($"Node {node.Url} enqueued for retrial {node.Retries} times.");
			}
			else {
				logger.LogInformation($"Node {node.Url} reached max retries.");
			}
		}

		private void EnqueueNodeUrls(NodeInfo node, string content) {
			var nodeUris = Utils.GetAbsoluteUrisFromHtml(root, content);
			// Make Uris unique by inserting them into a set structure.
			var childrenUris = new HashSet<Uri>(nodeUris);
			foreach (var uri in childrenUris) {
				var child = new NodeInfo(uri, node.Depth + 1);
				node.AddChild(child);
				// Limit the space to in-domain URLs.
				if (root.Host == uri.Host && (maxDepth == 0 || child.Depth < maxDepth)) {
					lock (locker) {
						if (!paths.Contains(uri.AbsolutePath)) {
							queue.Enqueue(child);
							paths.Add(uri.AbsolutePath);
						}
					}
				}
			}
		}
		
	}

}
