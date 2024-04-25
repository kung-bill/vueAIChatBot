const { defineConfig } = require('@vue/cli-service')
module.exports = defineConfig({
  transpileDependencies: true,
  devServer: {
    proxy: {
      '/api': {
        target: 'https://localhost:7152',
        changeOrigin: true,
        pathRewrite: {
          '^/api': "https://localhost:7152/api/OpenAIchat"
        }
      }
    }
  }
})
