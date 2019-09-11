using System.Net;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CrawlerService.Helpers {

	/// <summary>
	/// Web request content provider implementation. Supports timeout on tasks execution.
	/// </summary>
	internal class WebContentProvider : IContentProvider {

		private int timeout;

		public WebContentProvider(IConfiguration configuration) {
			this.timeout = configuration.GetValue<int>("Timeout");
		}

		public string GetHtmlContent(string url) {
			var client = WebRequest.Create(url);
			client.Timeout = timeout;
			var response = client.GetResponse();
			return new StreamReader(response.GetResponseStream()).ReadToEnd();
		}

		public async Task<string> GetHtmlContentAsync(string url) {
			var client = WebRequest.Create(url);
			client.Timeout = timeout;
			var response = await client.GetResponseAsync();
			return await new StreamReader(response.GetResponseStream()).ReadToEndAsync();
		}

	}
	
}
