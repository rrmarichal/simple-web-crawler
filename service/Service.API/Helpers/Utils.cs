using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace CrawlerService.Helpers {

	public static class Utils {

		// Will make schema mandatory to avoid casing on schemaless URLs.
		const string UrlRegex = @"^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$";

		public static bool IsValidHttpUrl(string value) {
			return Regex.IsMatch(value, UrlRegex);
		}
		
		/// <summary>
		/// Use Html Agility Pack library to extract anchor elements from
		/// the web page.
		/// </summary>
		public static List<Uri> GetAbsoluteUrisFromHtml(Uri baseUri, string html) {
			var uris = new HashSet<Uri>();
			var doc = new HtmlDocument();
			doc.LoadHtml(html);
			var nodes = doc.DocumentNode.SelectNodes("//a[@href]");
			if (nodes != null) {
				foreach (var node in nodes) {
					var href = node.Attributes["href"].Value;
					uris.Add(GetAbsoluteUriWithPath(baseUri, href));
				}
			}
			return uris.ToList();
		}

		/// <summary>
		/// Try to parse the URL as relative. Will throw an exception if it's absolute.
		/// </summary>
		public static Uri GetAbsoluteUriWithPath(Uri baseUri, string url) {
			Uri result;
			try {
				var uri = new Uri(url, UriKind.Relative);
				result = new Uri(baseUri, uri);
			}
			catch (Exception) {
				result = new Uri(url, UriKind.Absolute);
			}
			return GetUriWithPath(result);
		}

		/// <summary>
		/// Only consider the URI up to the path. For crawling purposes, page's query string
		/// parameters aren't supposed to change its contents.
		/// </summary>
		public static Uri GetUriWithPath(Uri uri) {
			if (!uri.IsAbsoluteUri) {
				throw new ArgumentException("uri must be absolute.");
			}
			return new UriBuilder(uri.Scheme, uri.Host, uri.Port, uri.AbsolutePath).Uri;
		}

	}

}
