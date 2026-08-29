import { fileURLToPath, URL } from 'node:url'
import fs from 'node:fs'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'

export default defineConfig(({ command }) => {
  const isDevelopment = command === 'serve'

  return {
    plugins: [
      vue(),
      vueDevTools(),
    ],

    server: isDevelopment
      ? {
          https: {
            key: fs.readFileSync('../localhost+2-key.pem'),
            cert: fs.readFileSync('../localhost+2.pem'),
          },

          port: 5173,
        }
      : undefined,

    resolve: {
      alias: {
        '@': fileURLToPath(
          new URL('./src', import.meta.url)
        ),
      },
    },
  }
})