const CssMinimizerPlugin = require('css-minimizer-webpack-plugin')

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

const cssLoaders = isProd => [
    {
        loader: 'file-loader',
        options: {
            name: '[name].css',
        },
    }, 'extract-loader',
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
let rules = isProd => [
    {
        test: /\.ts$/,
        loader: 'ts-loader',
        exclude: /node_modules/,
    },
    {
        test: /\.css$/,
        use: cssLoaders(isProd),
    },
    {
        test: /\.less$/,
        use: [...cssLoaders(isProd), { loader: 'less-loader' }],
    },
]


module.exports = env => {
    const isProd = env.production
    return {
        entry: createEntries(),
        output: {
            path: __dirname + '/wwwroot/dist', //打包后的文件存放的地方
            filename: '[name].js', //打包后输出文件的文件名
        },
        module: {
            rules: rules(isProd),
        },
        resolve: {
            extensions: ['.ts', '.css'],
        },
        optimization: {
            minimize: isProd,
            minimizer: [
                new CssMinimizerPlugin(),
            ],
        },
        devtool: isProd ? false : 'inline-source-map',
    }
}
