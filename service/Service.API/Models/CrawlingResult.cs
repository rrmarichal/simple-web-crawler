namespace CrawlerService.Models {

	/// <summary>
	/// Model a crawling operation on a specific website.
	/// </summary>
	public class CrawlingResult {

		public string Error { get; set; }

		public NodeInfo Sitemap { get; set; }

	}

}
