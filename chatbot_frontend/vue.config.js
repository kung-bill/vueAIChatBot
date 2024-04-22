const { defineConfig } = require('@vue/cli-service')
module.exports = defineConfig({
  transpileDependencies: true,
  devServer: {
    proxy: {
      '/api': {
        target: 'https://localhost:7139',
        changeOrigin: true,
        pathRewrite: { // pathRewrite 的作用是把实际Request Url中的'/api'用""代替
          '^/api': "https://localhost:7139/api/chat" 
      }
      }
    }
  }
})
