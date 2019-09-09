using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CrawlerService.Models;
using CrawlerService.Helpers;

namespace CrawlerService.Controllers {

	/// <summary>
	/// Controller implementation for crawler services.
	/// </summary>
	[Route("api/crawl")]
	[ApiController]
	[Produces("application/json")]
	public class CrawlController : ControllerBase {

		private readonly IConfiguration configuration;
		private readonly ILogger<CrawlController> logger;
		private readonly ICrawlerStrategy crawler;

		/// <summary>
		/// Initializes a new instance of CrawlController with default (injected) dependencies.
		/// </summary>
		public CrawlController(
			IConfiguration configuration,
			ILogger<CrawlController> logger,
			ICrawlerStrategy crawler
		) {
			this.configuration = configuration;
			this.logger = logger;
			this.crawler = crawler;
		}

		/// <summary>
		/// Start a crawling task with specific url and depth parameters.
		/// </summary>
		/// <remarks>
		/// 
		/// POST /api/crawl?url=[url]&amp;max-depth=[max-depth]
		/// 
		/// max-depth query parameter is optional.
		/// 
		/// </remarks>
		/// <response code="200">CrawlResult object containing the sitemap.</response>
		[HttpPost]
		[ProducesResponseType(200)]
		public ActionResult<CrawlingResult> Post() {
			try {
				var taskInfo = GetTaskInfo();
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
			var maxDepth = configuration.GetValue<int>("MaxDepth");
			int queryMaxDepth;
			if (queryParams.ContainsKey("max-depth") && int.TryParse(queryParams["max-depth"], out queryMaxDepth)) {
				maxDepth = queryMaxDepth;
			}
			return new CrawlerTaskInfo(url, maxDepth);
		}

	}
	
}
