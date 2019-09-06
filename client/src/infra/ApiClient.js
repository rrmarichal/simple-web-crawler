import axios from 'axios'

import config from './Config'

export default class ApiClient {

	static crawl = async (url, maxDepth) => {
		const client = axios.create({
			baseURL: config.crawlerServiceUrl()
		})
		return client.post(`/api/crawl?url=${escape(url)}&max-depth=${maxDepth}`)
	}

}
