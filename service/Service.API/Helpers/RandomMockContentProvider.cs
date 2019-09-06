using System;
using System.Text;
using System.Threading;

namespace CrawlerService.Helpers {

	/// <summary>
	/// Mock content provider. Returns careers and about page on base URL and null for others.
	/// </summary>
	public class RandomMockContentProvider : IContentProvider {

		public string GetHtmlContent(string url) {
			var r = new Random();
			var children = 1 + r.Next(5);
			var content = new StringBuilder();
			for (int j = 0; j < children; j++) {
				var id = r.Next();
				content.Append($"<a href=\"/careers-{id}\">Careers</a>");
			}
			// Simulate a delay in the request.
			Thread.Sleep(1000 + new Random().Next(2000));
			return content.ToString();
		}
		
	}

}
