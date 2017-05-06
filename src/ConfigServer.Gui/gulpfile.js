var gulp = require('gulp'),
    tsc = require('gulp-typescript'),
    tslint = require('gulp-tslint'),
    clean = require('gulp-clean'),
    systemjsBuilder = require('gulp-systemjs-builder');


gulp.task('BuildUi', ['CleanUi'], function () {
    var tsProject = tsc.createProject('./App/tsconfig.json');
    var tsResult = tsProject.src()
        .pipe(tsProject());

    return tsResult.js.pipe(gulp.dest('wwwroot/Assets/app'));
});

gulp.task('BuildPackageAssets', ['BuildTsAssets', 'CopyWwwRootAssets'])

gulp.task('BuildTsAssets', function () {
    var tscProject = tsc.createProject('./App/tsconfig.json');
    var tsResult = gulp.src('./App/**/*.ts')
        .pipe(tscProject(tsc.reporter.longReporter()))
        .on('error', function () { process.exit(1) });

    return tsResult.js.pipe(gulp.dest('../ConfigServer.Server/Assets/app'));
});

gulp.task('build-sjs',  function  ()  {
    var builder = systemjsBuilder();
    builder.loadConfigSync('./systemjs.config.js');

    builder.buildStatic('app',  {
                minify:  false,
                mangle: false
    })
    .pipe(gulp.dest('./build'));
})

gulp.task('CopyWwwRootAssets', function () {
    
    return gulp.src(['./wwwroot/Assets/*.css', './wwwroot/Assets/systemjs.config.js'])
    .pipe(gulp.dest('../ConfigServer.Server/Assets'));
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