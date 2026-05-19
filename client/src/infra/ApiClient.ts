import axios, { AxiosResponse } from 'axios'
import config from './Config'
import type { SitemapNode } from '../components/Sitemap'

interface CrawlResponse {
	sitemap?: SitemapNode
	error?: string
}

export default class ApiClient {
	static crawl = (url: string, maxDepth: number): Promise<AxiosResponse<CrawlResponse>> => {
		const client = axios.create({
			baseURL: config.crawlerServiceUrl()
		})
		return client.post<CrawlResponse>(`/api/crawl?url=${encodeURIComponent(url)}&max-depth=${maxDepth}`)
	}
}
