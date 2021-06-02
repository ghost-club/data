// Note this only includes basic configuration for development mode.
// For a more comprehensive configuration check:
// https://github.com/fable-compiler/webpack-config-template

var CONFIG = {
  fsharpEntry: './src/App.fs.js',
  outputDir: './output',
}

var path = require("path");
var fs = require('fs');

console.log("Bundling for cli...");

var resolve = path.resolve;

var nodeModulesDir = resolve("node_modules");

var nodeExternals = {};
fs.readdirSync(nodeModulesDir)
  .filter(function(x) {
    return ['.bin'].indexOf(x) === -1;
  })
  .forEach(function(mod) {
    nodeExternals[mod] = 'commonjs ' + mod;
  });

var isProduction = !process.argv.find(v => v.indexOf('serve') !== -1);
console.log("Bundling for " + (isProduction ? "production" : "development") + "...");

const CopyWebpackPlugin = require('copy-webpack-plugin')

module.exports = {
  mode: (isProduction ? "production" : "development"),
  devtool: "inline-source-map",
  entry: CONFIG.fsharpEntry,
  target: "node",
  externals: nodeExternals,
  output: {
    filename: 'app.js',
    path: path.join(__dirname, CONFIG.outputDir),
    devtoolModuleFilenameTemplate: info =>
      path.resolve(info.absoluteResourcePath).replace(/\\/g, '/'),
  },
  resolve: {
    modules: [nodeModulesDir]
  },
  module: {
    rules: [
      {
        test: /\.node$/,
        use: 'node-loader'
      },
    ]
  }
};
