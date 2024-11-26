import { sveltekit } from '@sveltejs/kit/vite';
import { defineConfig } from 'vite';

export default defineConfig({
	plugins: [sveltekit(), MDHmr()],
	test: {
		include: ['static/**/*.{test,spec}.{js,ts}']
	},
	
});

function MDHmr() {
	return {
		name: 'md-hmr',
		enforce: 'post',
		handleHotUpdate({ file, server }) {
			if (file.endsWith('.md')) {
				server.ws.send({
					type: 'full-reload',
					path: '*'
				});
			}
		}
	};
}
