﻿/// <binding Clean='clean, minify, scripts' />
/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');
var uglify = require('gulp-uglify');
var concat = require('gulp-concat');
var rimraf = require("rimraf");
var merge = require('merge-stream'); 

// Dependency Dirs
var deps = {
  "bootstrap": {
      "dist/js/*": ""
  },
  "jquery": {
    "dist/*": ""
  },
  "jquery-validation": {
      "dist/*": ""
  },
  "jquery-validation-unobtrusive": {
      "dist/*": ""
  },
  "require.js": {
      "*.js": ""
  },
  "cookieconsent": {
    "build/*": ""
  },
  "clipboard": {
      "dist/*": ""
  }
};

gulp.task("clean", function (cb) {
  return rimraf("wwwroot/lib/", cb);
});

gulp.task("scripts", function () {

  var streams = [];

  for (var prop in deps) {
    console.log("Prepping Scripts for: " + prop);
    for (var itemProp in deps[prop]) {
      streams.push(gulp.src("node_modules/" + prop + "/" + itemProp)
        .pipe(gulp.dest("wwwroot/lib/" + prop + "/" + deps[prop][itemProp])));
    }
  }

  return merge(streams);
});

gulp.task("default", ['clean', 'scripts']);