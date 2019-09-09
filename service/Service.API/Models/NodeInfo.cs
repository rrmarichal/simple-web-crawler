using System;
using System.Collections.Generic;
using CrawlerService.Helpers;

namespace CrawlerService {

	/// <summary>
	/// Thread safe implementation for sitemap node objects.
	/// </summary>
	public class NodeInfo {

		private readonly Uri uri;
		private readonly object locker;
		private List<NodeInfo> children;

		/// <summary>
		/// Initializes a new instance of the NodeInfo class.
		/// </summary>
		/// <param name="uri">System.Uri instance representing this node.</param>
		/// <param name="depth">The depth of the node in the sitemap as it was first found.</param>
		public NodeInfo(Uri uri, int depth = 0) {
			this.uri = uri;
			Depth = depth;
			children = new List<NodeInfo>();
			locker = new object();
		}

		/// <summary>
		/// Node URL.
		/// </summary>
		public string Url => Utils.GetUriWithPath(uri).ToString();

		/// <summary>
		/// Node children list.
		/// </summary>
		public List<NodeInfo> Children => new List<NodeInfo>(children);

		internal int Depth { get; }

		internal int Retries { get; private set; }

		internal void AddChild(NodeInfo child) {
			lock (locker) {
				children.Add(child);
			}
		}

		internal void AddRetry() {
			Retries++;
		}

	}
	
}
