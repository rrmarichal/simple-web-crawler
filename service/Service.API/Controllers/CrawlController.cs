using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using CrawlerService.Models;
using CrawlerService.Helpers;
using Microsoft.Extensions.Logging;

namespace CrawlerService.Controllers {

	[Route("api/crawl")]
	[ApiController]
	public class CrawlController : ControllerBase {
		
		private readonly IConfiguration configuration;
		private readonly ILogger<CrawlController> logger;

		public CrawlController(IConfiguration configuration, ILogger<CrawlController> logger) {
			this.configuration = configuration;
			this.logger = logger;
		}

		/// <summary>
		/// POST /?url=[url]&max-depth=[max-depth]
		/// <code>url</code> query parameter is mandatory, the others are optional.
		/// </summary>
		[HttpPost]
		public ActionResult<CrawlingResult> Post() {
			try {
				var taskInfo = GetTaskInfo();
				var timeout = int.Parse(configuration["Timeout"]);
				// var crawler = new BFSCrawler(logger, new WebContentProvider(timeout));
				var crawler = new DFSCrawler(logger, new WebContentProvider(timeout));
				return crawler.Crawl(taskInfo);
			}
			catch (Exception e) {
				logger.LogError(e, e.Message);
				return new CrawlingResult { Error = e.Message };
			}
		}

		private CrawlerTaskInfo GetTaskInfo() {
			if (!Request.QueryString.HasValue) {
				throw new Exception("Missing URL query string parameter.");
			}
			var queryParams = QueryHelpers.ParseQuery(Request.QueryString.Value);
			if (!queryParams.ContainsKey("url")) {
				throw new Exception("Missing URL query string parameter.");
			}
			var url = queryParams["url"];
			if (!Utils.IsValidHttpUrl(url)) {
				throw new Exception("Invalid URL query string parameter.");
			}
			// Default to MaxDepth configuration value.
			var maxDepth = int.Parse(configuration["MaxDepth"]);
			int queryMaxDepth;
			if (queryParams.ContainsKey("max-depth") && int.TryParse(queryParams["max-depth"], out queryMaxDepth)) {
				maxDepth = queryMaxDepth;
			}
			return new CrawlerTaskInfo(url, maxDepth);
		}

	}

}
