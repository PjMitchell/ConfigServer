/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');
var tsc = require('gulp-typescript');

gulp.task('BuildUi', function () {
    // place code for your default task here
    var tsProject = tsc.createProject('./App/tsconfig.json');
    var tsResult = tsProject.src()
        .pipe(tsProject());

    return tsResult.js.pipe(gulp.dest('wwwroot/Assets/app'));
});

gulp.task('BuildPackageAssets', ['BuildTsAssets', 'CopyWwwRootAssets', 'CopyNPMAssets'])

gulp.task('BuildTsAssets', function () {
    var tsProject = tsc.createProject('./App/tsconfig.json');
    var tsResult = tsProject.src()
        .pipe(tsProject());

    return tsResult.js.pipe(gulp.dest('../ConfigServer.Server/Assets/app'));
});

gulp.task('CopyWwwRootAssets', function () {
    
    return gulp.src(['./wwwroot/Assets/*.css', './wwwroot/Assets/systemjs.config.js'])
    .pipe(gulp.dest('../ConfigServer.Server/Assets'));
});

gulp.task('CopyNPMAssets', function () {
    return gulp.src(['./node_modules/bootstrap/dist/css/bootstrap.min.css'])
        .pipe(gulp.dest('./wwwroot/Assets/css'));
});