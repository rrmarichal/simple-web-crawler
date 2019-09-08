using CrawlerService.Models;

namespace CrawlerService.Helpers {

	public interface ICrawlerStrategy  {

		CrawlingResult Crawl(CrawlerTaskInfo taskInfo);

	}

}
