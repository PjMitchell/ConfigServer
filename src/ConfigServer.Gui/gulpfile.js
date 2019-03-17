var gulp = require('gulp'),
    tsc = require('gulp-typescript'),
    tslint = require('gulp-tslint'),
    clean = require('gulp-clean');


gulp.task('BuildPackageAssets', function () {

    var source = [
        'node_modules/@angular/material/prebuilt-themes/deeppurple-amber.css'
    ];
    return gulp.src(source)
        .pipe(gulp.dest('./wwwroot/Assets/lib'));
});

gulp.task('CleanPackageAssets', function () {
    return gulp.src('../ConfigServer.Server/Assets/*')
        .pipe(clean({ force:  true }));
});

gulp.task('CleanConfigs', function () {
    return gulp.src('./FileStore/*')
        .pipe(clean());
});

gulp.task('CopySeedData', ['CleanConfigs'], function () {
    return gulp.src(['./SeedData/**/*.*'])
        .pipe(gulp.dest('./FileStore'));
});

gulp.task('BuildE2E', function () {
    var tscProject = tsc.createProject('./E2eTests/tsconfig.json');
    var tsResult = gulp.src('./E2eTests/**/*.ts')
        .pipe(tscProject(tsc.reporter.longReporter()))
        .on('error', function () { process.exit(1) });
    return tsResult.js.pipe(gulp.dest('E2eTests'));
});