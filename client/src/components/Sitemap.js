import React from 'react'
import PropTypes from 'prop-types'

const renderTree = node => {
	if (node.children && node.children.length) {
		const ctree = node.children.map(child => renderTree(child))
		return (
			<ul>
				<li><a href={node.url} target='blank'>{ node.url }</a></li>
				{ ctree }
			</ul>
		)
	}
	return <ul><li><a href={node.url} target='blank'>{ node.url }</a></li></ul>
}

const Sitemap = ({ sitemap }) => {
	if (!sitemap) {
		return null
	}
	return renderTree(sitemap)
}

Sitemap.propTypes = {
	sitemap: PropTypes.object
}

export default Sitemap
