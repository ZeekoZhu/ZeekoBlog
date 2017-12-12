const webpack = require('webpack');
const MinifyPlugin = require("babel-minify-webpack-plugin");
const ExtractTextPlugin = require("extract-text-webpack-plugin");


let entries = {
    'zeeko.js': './wwwroot/ts/Zeeko.ts',
    'article.js': './wwwroot/ts/ArticleModule.ts',
    'theme.css': './wwwroot/css/theme.css'
}
function createEntries() {
    let result = {};
    for (key in entries) {
        if (entries.hasOwnProperty(key)) {
            // result[`${key}.min`] = entries[key];
            result[key] = entries[key];
        }
    }
    return result;
}

console.dir(createEntries())

module.exports = {
    entry: createEntries(),
    plugins: [
        new webpack.optimize.CommonsChunkPlugin({
            name: 'commons',
            filename: 'commons.js'
        }),
        new ExtractTextPlugin({
            filename: '[name]'
        }),
        new MinifyPlugin({}, {
            test: /\.js$/
        }),
    ],
    output: {
        path: __dirname + '/wwwroot/dist',//打包后的文件存放的地方
        filename: '[name]'//打包后输出文件的文件名
    },
    module: {
        rules: [
            {
                test: /\.ts$/,
                loader: 'ts-loader',
                exclude: /node_modules/
            },
            {
                test: /\.css$/,
                use: ExtractTextPlugin.extract({
                    use: {
                        loader: 'css-loader',
                        options: {
                            minimize: true,
                            import: true,
                            sourceMap: true
                        }
                    }
                })
            },
            {
                enforce: 'pre',
                test: /\.ts$/,
                use: 'source-map-loader'
            },
        ]
    },
    resolve: {
        extensions: ['.ts', '.css']
    },
    devtool: 'inline-source-map',
}
