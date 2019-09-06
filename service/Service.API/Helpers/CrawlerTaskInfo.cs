namespace CrawlerService.Helpers {

	public class CrawlerTaskInfo {

		public CrawlerTaskInfo(string baseUrl, int maxDepth) {
			BaseUrl = baseUrl;
			MaxDepth = maxDepth;
		}

		public string BaseUrl { get; private set; }

		public int MaxDepth { get; private set; }

	}

}
