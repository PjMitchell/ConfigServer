var gulp = require('gulp'),
    tsc = require('gulp-typescript'),
    tslint = require('gulp-tslint'),
    clean = require('gulp-clean');


gulp.task('BuildTs', function () {
    var tsProject = tsc.createProject('./App/tsconfig.json');
    var tsResult = tsProject.src()
        .pipe(tsProject());

    return tsResult.js.pipe(gulp.dest('./App'));
});

gulp.task('CopyWwwRootAssets', function () {
    
    return gulp.src(['./wwwroot/Assets/*.css', './wwwroot/Assets/**/*.js'])
    .pipe(gulp.dest('../ConfigServer.Server/Assets'));
});

gulp.task('BuildPackageAssets', function () {

    var source = [
        'node_modules/zone.js/dist/zone.min.js',
        'node_modules/core-js/client/shim.min.js',
        'node_modules/systemjs/dist/system.js',
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
gulp.task('CleanUi', function () {
    return gulp.src('./wwwroot/Assets/app/*')
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

gulp.task('TsLint', function () {
    gulp.src(['./E2eTests/**/*.ts', './App/**/*.ts'])
        .pipe(tslint())
        .pipe(tslint.report({ summarizeFailureOutput: true }))
});

gulp.task('Watch', function () {
    gulp.watch('./E2eTests/**/*.ts', ['BuildE2E']);
    gulp.watch('./App/**/*.ts', ['BuildTsAssets']);
});