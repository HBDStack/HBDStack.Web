const { createProxyMiddleware } = require('http-proxy-middleware')
const { env } = require('process')

const target = env.ASPNETCORE_HTTPS_PORT
    ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
    : env.ASPNETCORE_URLS
        ? env.ASPNETCORE_URLS.split(';')[0]
        : 'http://localhost:61361'

module.exports = function (app) {
  const appProxy = createProxyMiddleware(
      ['/api/**',  '/auth/**','/docs/**','/swagger/**'],
      {
        target: target,
        secure: false,
      }
  )

  app.use(appProxy)
}
