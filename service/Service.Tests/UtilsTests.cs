using System;
using NUnit.Framework;
using CrawlerService.Helpers;

namespace CrawlerService.Tests {

	public class UtilsTests {

		[Test]
		[TestCase("http://www.fiboapp.com", ExpectedResult = true)]
		[TestCase("https://www.fiboapp.com", ExpectedResult = true)]
		[TestCase("www.fiboapp.com", ExpectedResult = false)]
		[TestCase("fiboapp.com", ExpectedResult = false)]
		[TestCase("htt://someweb.in", ExpectedResult = false)]
		[TestCase("123", ExpectedResult = false)]
		[TestCase("someweb.12-3", ExpectedResult = false)]
		[TestCase("someweb.in1", ExpectedResult = false)]
		[TestCase("ftp://fiboapp.com", ExpectedResult = false)]
		public bool ValidHttpUrlTests(string url) {
			return Utils.IsValidHttpUrl(url);
		}

		[Test]
		public void GetUrlsRelativeUrlsTest() {
			var html = 
				@"<div width=""50%"">
					<a href=""/about""> About </a>
				</div>
				<div width=""50%"">
					<a href=""/careers""> Careers </a>
				</div>";
			var urls = Utils.GetAbsoluteUrisFromHtml(new Uri("https://www.fiboapp.com"), html);
			Assert.AreEqual(2, urls.Count);
			Assert.AreEqual("https://www.fiboapp.com/about", urls[0].ToString());
			Assert.AreEqual("https://www.fiboapp.com/careers", urls[1].ToString());
		}

		[Test]
		public void GetAbsoluteUrisFromHtmlNullContentTest() {
			var uris = Utils.GetAbsoluteUrisFromHtml(new Uri("https://www.fiboapp.com"), null);
			Assert.AreEqual(0, uris.Count);
		}

		[Test]
		public void GetAbsoluteUrisFromHtmlEmptyContentTest() {
			var uris = Utils.GetAbsoluteUrisFromHtml(new Uri("https://www.fiboapp.com"), string.Empty);
			Assert.AreEqual(0, uris.Count);
		}

		[Test]
		public void GetAbsoluteUrisFromHtmlNonHtmlTest() {
			var nonHtml = @"{ ""A"": ""JSON instead"" }";
			var uris = Utils.GetAbsoluteUrisFromHtml(new Uri("https://www.fiboapp.com"), nonHtml);
			Assert.AreEqual(0, uris.Count);
		}

		[Test]
		public void GetAbsoluteUrisFromHtmlSimpleTest() {
			var html = 
				@"<div>
					<a href=""http://www.google.com?q=fiboapp""> About </a>
					<div width=""50%"">
						<a href=""/faq""> About </a>
					</div>
					<a href=""http://www.bing.com?q=fiboapp""> About </a>
				</div>";
			var uris = Utils.GetAbsoluteUrisFromHtml(new Uri("https://www.fiboapp.com"), html);
			Assert.AreEqual(3, uris.Count);
			Assert.AreEqual("http://www.google.com/", uris[0].ToString());
			Assert.AreEqual("https://www.fiboapp.com/faq", uris[1].ToString());
			Assert.AreEqual("http://www.bing.com/", uris[2].ToString());
		}

		[Test]
		public void UrlComponentsTest() {
			var uri0 = new Uri("http://community.fiboapp.com/posts?q=today");
			Assert.AreEqual("community.fiboapp.com", uri0.Host);
			Assert.AreEqual("/posts", uri0.AbsolutePath);
			Assert.AreEqual("?q=today", uri0.Query);
			// Hash based URLs.
			var uri1 = new Uri("http://fiboapp.com/#/blog");
			Assert.AreEqual("fiboapp.com", uri1.Host);
			Assert.AreEqual("/", uri1.AbsolutePath);
		}

		[Test]
		public void GetAbsoluteUriFromRelativeUriTest0() {
			var baseUri = new Uri("https://www.fiboapp.com");
			var u = Utils.GetAbsoluteUriWithPath(baseUri, "/careers");
			Assert.AreEqual("https://www.fiboapp.com/careers", u.ToString());
		}

		[Test]
		public void GetAbsoluteUriFromRelativeUriTest1() {
			var baseUri = new Uri("https://www.fiboapp.com");
			var u = Utils.GetAbsoluteUriWithPath(baseUri, "about");
			Assert.AreEqual("https://www.fiboapp.com/about", u.ToString());
		}

		[Test]
		public void GetAbsoluteUriFromAbsoluteUriTest() {
			var baseUri = new Uri("https://www.fiboapp.com");
			var u = Utils.GetAbsoluteUriWithPath(baseUri, "http://www.google.com/?q=a");
			Assert.AreEqual("http://www.google.com/", u.ToString());
		}

	}

}
