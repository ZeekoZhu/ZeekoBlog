/// <binding BeforeBuild='Run - Development' />
'use strict';
const TerserPlugin = require('terser-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const OptimizeCssAssetsPlugin = require('optimize-css-assets-webpack-plugin');

let isProd = (process.env.ASPNETCORE_ENVIRONMENT && process.env.ASPNETCORE_ENVIRONMENT.toLowerCase() === 'production') || (process.env.NODE_ENV && process.env.NODE_ENV.toLowerCase() === 'production');


let entries = {
    'zeeko': './wwwroot/ts/Zeeko.ts',
    'article': './wwwroot/ts/ArticleModule.ts',
    'theme': './wwwroot/css/theme.less'
}
function createEntries() {
    let result = {};
    let key;
    for (key in entries) {
        if (entries.hasOwnProperty(key)) {
            result[key] = entries[key];
        }
    }
    return result;
}

let rules = [
    {
        test: /\.ts$/,
        loader: 'ts-loader',
        exclude: /node_modules/
    },
    {
        test: /\.less$/,
        use: [
            {
                loader: MiniCssExtractPlugin.loader
            },
            {
                loader: 'css-loader',
                options: {
                    import: true,
                    sourceMap: !isProd
                }
            },
            {
                loader: 'postcss-loader',
                options: {
                    plugins: [
                        require('autoprefixer')
                    ]
                }
            },
            {
                loader: 'less-loader'
            }
        ]
    }
];


module.exports = {
    entry: createEntries(),
    plugins: [
        new MiniCssExtractPlugin({
            filename: "[name].css"
        }),
        new OptimizeCssAssetsPlugin({
            cssProcessorOptions: {
                map: isProd ? undefined : { inline: true }
            }
        })
    ],
    output: {
        path: __dirname + '/wwwroot/dist', //打包后的文件存放的地方
        filename: '[name].js' //打包后输出文件的文件名
    },
    module: {
        rules: rules
    },
    resolve: {
        extensions: ['.ts', '.css']
    },
    optimization: {
        splitChunks: {
            cacheGroups: {
                styles: {
                    name: 'theme',
                    test: /\.css$/,
                    chunks: 'all',
                    enforce: true
                },
                commons: {
                    test: /\.(ts|js)$/,
                    name: 'commons',
                    chunks: 'initial',
                    minChunks: 2,
                    minSize: 0
                },
            }
        },
        minimizer: [new TerserPlugin()]
    },
    devtool: isProd ? '' : 'inline-source-map',
    mode: isProd ? 'production' : 'development'
};
