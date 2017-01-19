/// <binding BeforeBuild='build' Clean='clean' ProjectOpened='build' />
"use strict";

var gulp = require("gulp"),
    filter = require('gulp-filter'),
    flatten = require('gulp-flatten'),
    gnf = require("gulp-npm-files"),
    rimraf = require('rimraf');

gulp.task("build:js", function () {
    return gulp.src(gnf(), { base: '.' })
        .pipe(filter(['**/*.js']))
        .pipe(flatten({ newPath: 'vendor', subPath: [1, 2] }))
        .pipe(gulp.dest("Scripts"));
});
gulp.task("build:css", function () {
    return gulp.src(gnf(), { base: '.' })
        .pipe(filter(['**/*.css']))
        .pipe(flatten({ newPath: 'vendor' }))
        .pipe(gulp.dest("Content"));
});
gulp.task("build:fonts", function () {
    return gulp.src(gnf(), { base: '.' })
        .pipe(filter(['**/*.woff', '**/*.woff2', '**/*.ttf', '**/*.svg', '**/*.eot']))
        .pipe(flatten())
        .pipe(gulp.dest("Content/fonts"));
});
gulp.task("build", ['build:js', 'build:css', 'build:fonts']);


gulp.task("clean:js", function (cb) {
    rimraf("Scripts/vendor/*", cb);
});
gulp.task("clean:css", function (cb) {
    rimraf("Content/vendor/*", cb);
});
gulp.task("clean:fonts", function (cb) {
    rimraf("Content/fonts/*", cb);
});
gulp.task("clean", ['clean:js', 'clean:css', 'clean:fonts']);