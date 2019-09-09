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

### BFSCrawler

Breath first search approach, going one level of the tree at the time. At each step we spawn crawl tasks in parallel which will generate nodes for the next step.

### DFSCrawler

Depth first search approach, going from the root down in the sitemap tree recursively.

### FaultTolerantCrawler

A curated approach to `DFSCrawler` where we account for error and retries. A queue processor implementation maintains a list of processing tasks limited by a maximum concurrenly level parameter. When a node fails to crawl, it is re-queued up to a maximum number of times.

### Unit Tests
A test project (<code>Service.Tests</code>) contains:

- <code>UtilsTests</code> - Unit tests for the <code>Utils</code> class, which contains helper methods on URL validation, parsing	and transformation.
- <code>BFSCrawlerTests</code>, <code>DFSCrawlerTests</code> and <code>FaultTolerantCrawlerTests</code> - Using a mock implementation of the <code>IContentProvider</code> interface, we test basic functionality for the different crawler strategies implementations.

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

- Crawler strategy - Fault tolerance

	Independently of the strategies implemented to make the crawling process more resource efficient, scenarios like node crash, network outages, should be addressed by providing state management and recovery.

- Logs

	Errors should be all tracked in a logging system with relevant context information like time type/description, number of retries, etc.
