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
                src: ['wwwroot/assets/scss/bootstrap.scss'],
                dest: 'wwwroot/assets/css/isnsfw.css'
            }
        },
        watch: {
            css: {
                files: 'wwwroot/assets/scss/**/*.scss',
                tasks: ['sass']
            }
        }
    });

    //// Load the plugin
    grunt.loadNpmTasks('grunt-sass');
    grunt.loadNpmTasks('grunt-contrib-watch');

    //// Default task(s).
    grunt.registerTask('default', ['sass', 'watch']);
};