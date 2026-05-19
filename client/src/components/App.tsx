import React from 'react'
import { AppBar, Toolbar, Typography } from '@mui/material'
import InputForm from './InputForm'
import Sitemap, { SitemapNode } from './Sitemap'
import StatusView from './StatusView'
import ApiClient from '../infra/ApiClient'

interface State {
	loading: boolean
	sitemap?: SitemapNode
	error?: string
}

export default class App extends React.Component<object, State> {

	state: State = { loading: false }

	handleCrawlClicked = async (url: string, maxDepth: number) => {
		this.setState({ loading: true, sitemap: undefined })
		try {
			const crawlResponse = await ApiClient.crawl(url, maxDepth)
			if (crawlResponse.data.error) {
				this.setState({
					loading: false,
					error: crawlResponse.data.error
				})
			} else {
				this.setState({
					loading: false,
					error: undefined,
					sitemap: crawlResponse.data.sitemap
				})
			}
		} catch (err) {
			this.setState({
				loading: false,
				error: 'An error occurred while retrieving the sitemap.'
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
