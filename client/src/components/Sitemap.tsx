import React from 'react'

export interface SitemapNode {
	url: string
	children?: SitemapNode[]
}

const renderTree = (node: SitemapNode): React.ReactElement => {
	if (node.children && node.children.length) {
		return (
			<ul>
				<li><a href={node.url} target='blank'>{node.url}</a></li>
				{node.children.map((child, i) => (
					<React.Fragment key={i}>{renderTree(child)}</React.Fragment>
				))}
			</ul>
		)
	}
	return <ul><li><a href={node.url} target='blank'>{node.url}</a></li></ul>
}

interface Props {
	sitemap?: SitemapNode
}

const Sitemap: React.FC<Props> = ({ sitemap }) => {
	if (!sitemap) return null
	return renderTree(sitemap)
}

export default Sitemap
