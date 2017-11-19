/// <binding BeforeBuild='build' Clean='clean' />
'use strict';
const gulp = require('gulp');
const concat = require('gulp-concat');
const rename = require('gulp-rename');
const fs = require('fs');
const path = require('path');
const cssmin = require('gulp-cssmin');
// const uglify = require('gulp-uglify');
const merge = require('merge-stream');
const del = require('del');
const autoprefixer = require('gulp-autoprefixer');
const sourcemaps = require('gulp-sourcemaps');
const ts = require('gulp-typescript');
const filter = require('gulp-filter');
const clone = require('gulp-clone');
const uglifyes = require('uglify-es');
const bundleconfig = require('./bundles.json');

// config for es minify
const composer = require('gulp-uglify/composer');
const uglify = composer(uglifyes, console);


let regex = {
    css: /\.css$/,
    js: /\.js$/,
    lib: /lib$/
};

let tsproj = ts.createProject('tsconfig.json');

gulp.task('ts', () => {
    // 编译所有 ts 文件
    let ts$ = gulp.src('wwwroot/ts/**/*.ts', { base: '.' })
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(tsproj()).js.pipe(clone());
    // 按 Bundle 设置打包
    let tasks = getBundles(regex.js).map(bundle => {
        console.log(bundle.outputFileName, '--', bundle.inputFiles.join('; '));
        return ts$.pipe(clone())
            .pipe(rename({ extname: '.ts' }))
            .pipe(filter(bundle.inputFiles))
            .pipe(rename({ extname: '.js' }))
            .pipe(concat(bundle.outputFileName))
            .pipe(gulp.dest('.'))
            .pipe(uglify())
            .pipe(rename({ extname: '.min.js' }))
            .pipe(gulp.dest('.'));
    });
    return merge(tasks);
});

gulp.task('css', () => {
    let tasks = getBundles(regex.css).map(bundle => {
        let css$ = bundle.inputFiles.map(file => {
            let file$ = gulp.src(file, { base: '.' })
                .pipe(sourcemaps.init());
            return file$;
        });
        return merge(css$)
            .pipe(concat(bundle.outputFileName))
            .pipe(gulp.dest('.'))
            .pipe(cssmin())
            .pipe(rename({ extname: '.min.css' }))
            .pipe(sourcemaps.write('.'))
            .pipe(gulp.dest('.'));
    });
    return merge(tasks);
});

gulp.task('vendor', () => {
    let libs = getBundles(regex.lib)[0];
    return gulp.src(libs.inputFiles)
        .pipe(gulp.dest(libs.outputFileName));
});

gulp.task('clean', () => {
    let files = bundleconfig.map(bundle => bundle.outputFileName);
    let mins = bundleconfig.map(bundle => {
        let minExtReg = /(\.js$)|(\.css$)/;
        return bundle.outputFileName.replace(minExtReg, (match) => '.min' + match)
    });
    let maps = mins.map(min => min + '.map');
    return del([...files, ...maps, ...mins]);
});

gulp.task('build', ['ts', 'css', 'vendor']);

gulp.task('rebuild', ['clean', 'build']);

gulp.task('watch', () => {
    getBundles(regex.js).forEach(bundle => {
        gulp.watch(bundle.inputFiles, ['ts']);
    });
    getBundles(regex.css).forEach(bundle => {
        gulp.watch(bundle.inputFiles, ['css']);
    });
});

function getBundles(regexPattern) {
    return bundleconfig.filter(function (bundle) {
        return regexPattern.test(bundle.outputFileName);
    });
}
