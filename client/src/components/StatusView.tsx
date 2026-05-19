import React from 'react'

interface Props {
	loading: boolean
	error?: string
}

const StatusView: React.FC<Props> = ({ loading, error }) => {
	if (loading) {
		return <div className='loading'>Crawling...</div>
	}
	if (error) {
		return <div className='error'>{error}</div>
	}
	return null
}

export default StatusView
