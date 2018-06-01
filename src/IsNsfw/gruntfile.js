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
                src: ['wwwroot/assets/scss/isnsfw.scss'],
                dest: 'wwwroot/assets/css/isnsfw.css'
            }
        },
        //typescript: {
        //    base: {
        //        src: ['wwwroot/assets/ts/isnsfw.ts'],
        //        dest: 'wwwroot/assets/js/',
        //        options: {
        //            module: 'amd', //or commonjs
        //            target: 'es5', //or es3
        //            basePath: 'wwwroot/assets/ts/',
        //            sourceMap: true,
        //            declaration: true
        //        }
        //    }
        //},
        watch: {
            css: {
                files: 'wwwroot/assets/scss/**/*.scss',
                tasks: ['sass']
            },
            js: {
                files: 'wwwroot/assets/ts/**/*.ts',
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