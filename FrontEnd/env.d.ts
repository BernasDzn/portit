/// <reference types="vite/client" />

// Vue shim for test runner and TS compiler — allows `import Foo from './Foo.vue'`.
declare module '*.vue' {
	import { DefineComponent } from 'vue';
	const component: DefineComponent<{}, {}, any>;
	export default component;
}

// Ensure `import.meta.env` is typed for Vite environment variables used in the app
interface ImportMetaEnv {
	readonly BASE_URL?: string;
	// add other env vars you use here
}

interface ImportMeta {
	readonly env: ImportMetaEnv;
}
