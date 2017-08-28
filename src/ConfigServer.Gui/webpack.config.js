var webpack = require('webpack');
module.exports = {
    entry: {
        'app': './App/main.ts'
    },
    output: {
        filename: './wwwroot/Assets/[name].js'
    },
    resolve: {
        extensions: ['.ts', '.js']
    },

    module: {
        rules: [
            {
                test: /\.ts$/,
                loaders: [
                    {
                        loader: 'awesome-typescript-loader',
                        options: { configFileName: './App/tsconfig.json' }
                    }
                ]
            }
        ],
        
    },
    plugins: [
        new webpack.ContextReplacementPlugin(
            // The (\\|\/) piece accounts for path separators in *nix and Windows
            /angular(\\|\/)core(\\|\/)@angular/,
            './App', // location of your src
            {} // a map of your routes
        ),
        new webpack.optimize.UglifyJsPlugin({
            sourceMap: true
        }),
    ]    
};