using CrawlerService.Models;

namespace CrawlerService.Helpers {

	public interface ICrawler  {

		CrawlingResult Crawl(CrawlerTaskInfo taskInfo);

	}

}
