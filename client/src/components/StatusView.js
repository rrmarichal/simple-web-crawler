import React from 'react'

export default ({ loading, error }) => {
	if (loading) {
		return <div className='loading'>Crawling...</div>
	}
	if (error) {
		return <div className='error'>{ error }</div>
	}
	return null
}
