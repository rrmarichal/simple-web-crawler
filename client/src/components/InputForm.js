import React from 'react'
import PropTypes from 'prop-types'
import {
	TextField,
	Button
} from '@material-ui/core'

export default class InputForm extends React.Component {

	static propTypes = {
		loading: PropTypes.bool,
		onCrawlClicked: PropTypes.func.isRequired
	}
	
	state = { url: '', maxDepth: 0 }

	handleUrlChanged = event => {
		this.setState({ url: event.target.value })
	}

	handleMaxDepthChanged = event => {
		this.setState({ maxDepth: parseInt(event.target.value) })
	}

	render() {
		const { url, maxDepth } = this.state
		const { loading, onCrawlClicked } = this.props
		return (
			<div className='form'>
				<TextField
					autoFocus={true}
					fullWidth={true}
					disabled={loading}
					label='Enter URL'
					value={url}
					onChange={this.handleUrlChanged}
					type='text' />
				<div className='divider' />
				<TextField
					label='Crawling max depth'
					helperText='Enter 0 to fully crawl the website'
					fullWidth={true}
					disabled={loading}
					value={maxDepth}
					onChange={this.handleMaxDepthChanged}
					type='number' />
				<div className='divider' />
				<Button
					color='primary'
					variant='contained'
					disabled={loading || !url}
					onClick={() => onCrawlClicked(url, maxDepth)}>
					Crawl
				</Button>
			</div>
		)
	}

}
