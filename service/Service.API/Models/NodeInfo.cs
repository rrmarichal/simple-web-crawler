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

		public NodeInfo(Uri uri) {
			this.uri = uri;
			children = new List<NodeInfo>();
			locker = new object();
		}
		
		public string Url => Utils.GetUriWithPath(uri).ToString();

		public List<NodeInfo> Children => new List<NodeInfo>(children);

		public void AddChild(NodeInfo child) {
			lock (locker) {
				children.Add(child);
			}
		}

	}

}
