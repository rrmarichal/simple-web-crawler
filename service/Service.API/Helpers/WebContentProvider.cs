using System.Net;
using System.IO;

namespace CrawlerService.Helpers {

	/// <summary>
	/// Web request content provider implementation. Supports timeout on tasks execution.
	/// </summary>
	public class WebContentProvider : IContentProvider {

		private int timeout;

		public WebContentProvider(int timeout) {
			this.timeout = timeout;
		}

		public string GetHtmlContent(string url) {
			var client = WebRequest.Create(url);
			client.Timeout = timeout;
			var response = client.GetResponse();
			return new StreamReader(response.GetResponseStream()).ReadToEnd();
		}

	}

}
