/// <binding ProjectOpened='watch' />
module.exports = function (grunt) {
    'use strict';

    // Project configuration.
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),

        // Sass
        sass: {
            options: {
                sourceMap: true, // Create source map
                outputStyle: 'compressed' // Minify output
            },
            dev: {
                src: ['wwwroot/scss/isnsfw.scss'],
                dest: 'wwwroot/css/isnsfw.css'
            }
        },
        //typescript: {
        //    base: {
        //        src: ['wwwroot/ts/isnsfw.ts'],
        //        dest: 'wwwroot/js/',
        //        options: {
        //            module: 'amd', //or commonjs
        //            target: 'es5', //or es3
        //            basePath: 'wwwroot/ts/',
        //            sourceMap: true,
        //            declaration: true
        //        }
        //    }
        //},
        watch: {
            css: {
                files: 'wwwroot/scss/**/*.scss',
                tasks: ['sass']
            },
            js: {
                files: 'wwwroot/ts/**/*.ts',
                tasks: ['typescript']
            }
        }
    });

    //// Load the plugin
    grunt.loadNpmTasks('grunt-sass');
    //grunt.loadNpmTasks('grunt-typescript');
    grunt.loadNpmTasks('grunt-contrib-watch');

    //// Default task(s).
    grunt.registerTask('default', ['sass', 'watch']);
};