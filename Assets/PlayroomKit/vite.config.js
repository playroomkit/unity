import { defineConfig } from 'vite';
import { resolve } from 'path';
import {viteStaticCopy} from 'vite-plugin-static-copy';

export default defineConfig({
  define: {
    "process.env": {NODE_ENV: "production"},
  },
  plugins: [
    viteStaticCopy({
      targets: [
        {
          src: 'src/PlayroomPlugin.jslib.meta',
          dest: './'
        },
        {
          src: 'src/index.js',
          dest: './',
          rename: 'PlayroomPlugin.jslib'
        },
        {
          src: 'src/PlayroomFrameworks.jslib.meta',
          dest: './'
        }
      ]
    })
  ],
  build: {
    lib: {
      name: "PlayroomPluginFrameworks", 
      entry: resolve(__dirname, 'src/frameworks.js'), 
      fileName: (format) => `PlayroomFrameworks.jspre`, 
      formats: ['umd']
    },
    outDir: '../../Assets/Plugins/Playroom',
    minify: false,
    rollupOptions: {}
  },
});