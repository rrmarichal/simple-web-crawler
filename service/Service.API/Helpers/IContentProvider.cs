using System.Threading.Tasks;

namespace CrawlerService.Helpers {

	/// <summary>
	/// Interface to different HTML providers.
	/// </summary>
	public interface IContentProvider {

		/// <summary>
		/// Return the HTML contents in a URL.
		/// </summary>
		string GetHtmlContent(string url);

		/// <summary>
		/// Return the HTML contents in a URL using a System.Threading.Tasks.Task instance.
		/// </summary>
		Task<string> GetHtmlContentAsync(string url);

	}
	
}
