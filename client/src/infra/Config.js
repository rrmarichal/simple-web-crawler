require('dotenv').config()

export default class Config {

	static crawlerServiceUrl = () => process.env.REACT_APP_CRAWLER_SERVICE_URL

}
