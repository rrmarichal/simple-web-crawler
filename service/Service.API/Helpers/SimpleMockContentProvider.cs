namespace CrawlerService.Helpers {

	/// <summary>
	/// Mock content provider. Returns careers and about page on base URL and null for others.
	/// </summary>
	public class SimpleMockContentProvider : IContentProvider {

		private readonly string baseUrl;
		
		public SimpleMockContentProvider(string baseUrl) {
			this.baseUrl = baseUrl;
		}

		public string GetHtmlContent(string url) {
			if (url == baseUrl) {
				return @"
					<a href=""/careers"">Careers</a>
					<a href=""/about"">About</a>
					<div>
						<a href=""https://google.com/accounts"">Google Accounts</a>
					</div";
			}
			return null;
		}

	}
	
}
