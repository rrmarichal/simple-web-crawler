import React from 'react'
import {
	AppBar,
	Toolbar,
	Typography,
} from '@material-ui/core'

import InputForm from './InputForm'
import Sitemap from './Sitemap'
import StatusView from './StatusView'
import ApiClient from '../infra/ApiClient'

export default class App extends React.Component {

	state = { loading: false }

	handleCrawlClicked = async (url, maxDepth) => {
		// URL validation will happen in the service,
		// just to keep this as simple as possible.
		this.setState({ loading: true, sitemap: null })
		try {
			const crawlResponse = await ApiClient.crawl(url, maxDepth)
			if (crawlResponse.data.error) {
				this.setState({
					loading: false,
					error: crawlResponse.data.error
				})
			}
			else {
				this.setState({
					loading: false,
					error: null,
					sitemap: crawlResponse.data.sitemap
				})
			}
		}
		catch (err) {
			// For production ready code, we should trace errors
			// for monitoring tools, etc.
			this.setState({
				loading: false,
				error: "An error occurred while retrieving the sitemap."
			})
		}
		
	}

	render() {
		const { sitemap, error, loading } = this.state
		return (
			<React.Fragment>
				<AppBar position='static'>
					<Toolbar>
						<Typography variant='body1' noWrap={true}>
							Crawler Client
						</Typography>
					</Toolbar>
				</AppBar>
				<div className='content'>
					<InputForm
						loading={loading}
						onCrawlClicked={this.handleCrawlClicked} />
					<div className='divider' />
					<StatusView
						loading={loading}
						error={error} />
					<Sitemap sitemap={sitemap} />
				</div>
			</React.Fragment>
		)
	}

}
