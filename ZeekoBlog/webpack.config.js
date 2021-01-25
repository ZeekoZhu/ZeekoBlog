'use strict'
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CssMinimizerPlugin = require('css-minimizer-webpack-plugin');

let isProd = (process.env.ASPNETCORE_ENVIRONMENT && process.env.ASPNETCORE_ENVIRONMENT.toLowerCase() === 'production')
    || (process.env.NODE_ENV && process.env.NODE_ENV.toLowerCase() === 'production')


let entries = {
    'zeeko': './wwwroot/ts/Zeeko.ts',
    'style': './wwwroot/css/styles.css',
    article: './wwwroot/css/white/rendered-content.less',
}

function createEntries() {
    let result = {}
    let key
    for (key in entries) {
        if (entries.hasOwnProperty(key)) {
            result[key] = entries[key]
        }
    }
    return result
}

const cssLoaders = [
    {
        loader: MiniCssExtractPlugin.loader,
    },
    {
        loader: 'css-loader',
        options: {
            import: true,
            sourceMap: !isProd,
        },
    },
    {
        loader: 'postcss-loader',
    },
]
let rules = [
    {
        test: /\.ts$/,
        loader: 'ts-loader',
        exclude: /node_modules/,
    },
    {
        test: /\.css$/,
        use: cssLoaders,
    },
    {
        test: /\.less$/,
        use: [...cssLoaders, { loader: 'less-loader' }],
    },
]


module.exports = {
    entry: createEntries(),
    plugins: [
        new MiniCssExtractPlugin({
            filename: '[name].css',
        }),
    ],
    output: {
        path: __dirname + '/wwwroot/dist', //打包后的文件存放的地方
        filename: '[name].js', //打包后输出文件的文件名
    },
    module: {
        rules: rules,
    },
    resolve: {
        extensions: ['.ts', '.css'],
    },
    optimization: {
        minimize: isProd,
        minimizer: [
            new CssMinimizerPlugin()
        ]
    },
    devtool: isProd ? false : 'inline-source-map',
    mode: isProd ? 'production' : 'development',
}
