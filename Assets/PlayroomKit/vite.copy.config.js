import { defineConfig } from 'vite';
import { fileURLToPath } from 'url';
import { dirname, resolve } from 'path';
import { viteStaticCopy } from 'vite-plugin-static-copy';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

function removeEmptyBundle() {
  return {
    name: 'remove-empty-bundle',
    generateBundle(options, bundle) {
      for (const fileName in bundle) {
        const chunk = bundle[fileName];
        // If the file is generated from empty.js, remove it
        if (chunk.type === 'chunk' && chunk.code.trim() === '') {
          console.log(`Removing empty bundle: ${fileName}`);
          delete bundle[fileName];
        }
      }
    }
  };
}

export default defineConfig({
  plugins: [
    viteStaticCopy({
      targets: [
        {
          src: 'src/index.js',
          dest: './',
          rename: 'PlayroomPlugin.jslib'
        },
      ]
    }),
    removeEmptyBundle() 
  ],
  build: {
    emptyOutDir: false,
    rollupOptions: {
      input: resolve(__dirname, 'src/empty.js')
    },
    outDir: '../../Assets/Plugins/Playroom'
  },
});
