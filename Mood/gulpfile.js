/// <binding BeforeBuild='build' Clean='clean' ProjectOpened='build' />
"use strict";

var gulp = require("gulp"),
    filter = require('gulp-filter'),
    gnf = require("gulp-npm-files"),
    rimraf = require('rimraf');

gulp.task("build:js", function () {
    return gulp.src(gnf(), { base: '.' })
        .pipe(filter(['**/*.js']))
        .pipe(gulp.dest("Scripts"));
});
gulp.task("build:css", function () {
    return gulp.src(gnf(), { base: '.' })
        .pipe(filter(['**/*.css', '**/*.woff', '**/*.woff2', '**/*.ttf', '**/*.svg', '**/*.eot']))
        .pipe(gulp.dest("Content"));
});
gulp.task("build", ['build:js', 'build:css']);


gulp.task("clean:js", function (cb) {
    rimraf("Scripts/node_modules", cb);
});
gulp.task("clean:css", function (cb) {
    rimraf("Content/node_modules", cb);
});
gulp.task("clean", ['clean:js', 'clean:css']);