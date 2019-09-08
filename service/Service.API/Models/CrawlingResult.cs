namespace CrawlerService.Models {

	/// <summary>
	/// Model a crawling operation on a specific website.
	/// </summary>
	public class CrawlingResult {

		/// <summary>
		/// Error description.
		/// </summary>
		public string Error { get; set; }

		/// <summary>
		/// Tree structure starting at the root (request url).
		/// </summary>
		public NodeInfo Sitemap { get; set; }

	}

}
