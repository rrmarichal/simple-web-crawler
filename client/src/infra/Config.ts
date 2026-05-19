export default class Config {
	static crawlerServiceUrl = (): string | undefined =>
		import.meta.env.VITE_CRAWLER_SERVICE_URL
}
