import React from 'react'

const StatusView = ({ loading, error }) => {
	if (loading) {
		return <div className='loading'>Crawling...</div>
	}
	if (error) {
		return <div className='error'>{ error }</div>
	}
	return null
}

export default StatusView
