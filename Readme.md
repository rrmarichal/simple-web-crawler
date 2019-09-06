# Simple web crawler challenge

Given a URL, it should output a simple textual sitemap, showing the links between pages. The crawler should be limited to one subdomain - so when you start with *https://example.com/*, it would crawl all pages within *example.com*, but not follow external links, for example to facebook.com or blog.example.com.

## Client

The crawler client is a ReactJS SPA. The interface contains a text input for the URL and a numeric input to limit the maximum depth to crawl.

The sitemap is rendered using an unordered list (<code>ul</code>) of items (<code>li</code>) using indentation to show the hierarchy/relationship between them.

## Service

The crawler service (<code>Service.API</code>) is a .NET Core web API written in C#. The API contains a single route:

```
- POST ~/crawl?url=[url]&max-depth=[max-depth]
```

The crawler implementation is inside the <code>BFSCrawler</code> class. This is a breath first search approach, going one level of the tree at the time. At each step we spawn crawl tasks in parallel which will generate nodes for the next step.

There is also a test project (<code>Service.Tests</code>) which contains:

- <code>UtilsTests</code> - Unit tests for the <code>Utils</code> class, which contains helper methods on URL validation, parsing	and transformation.
- <code>BFSCrawlerTests</code> - Using a mock implementation of the <code>IContentProvider</code> interface, we test basic functionality for the <code>BFSCrawler</code> implementation.

## Launch

The crawler client will start on port 3000 by default (<code>react-scripts</code>) using:

```
cd crawler-client && yarn install && yarn start
```

The crawler service will start on port 3001 (<code>crawler-service/Service.API/Properties/launchSettings.json</code>) using:

```
cd crawler-service && dotnet run --project Service.API
```

## Production

In order to deliver this implementation on a reasonable time, some trade-offs were made. To make it production-ready, some other details should be addressed:

- Thread pool - Concurrency, Parallelism

	The concurrency level is managed by the .NET <code>ThreadPool</code>. For a more deterministic/configurable approach, we should control the level of parallelism.

- Crawler strategy - Fault tolerance

	Independently of the strategies implemented to make the crawling process more resource efficient, scenarios like node crash, network outages, low bandwidth, should be addressed by providing state management and recovery.

- Error handling - Errors, Timeouts, Retries, Logs

	Pages may be temporarily unavailable or the phisical network may fail on a high number of requests. Production ready apps may introduce retry mechanisms to enqueue failed requests.

	Errors should be all tracked in a logging system with relevant context information like time, type/description, number of retries, etc.
