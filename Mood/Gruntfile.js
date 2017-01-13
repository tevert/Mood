/*
This file in the main entry point for defining grunt tasks and using grunt plugins.
Click here to learn more. https://go.microsoft.com/fwlink/?LinkID=513275&clcid=0x409
*/
module.exports = function (grunt) {
    grunt.initConfig({
    });

    var depLinker = require('dep-linker');

    grunt.task.registerTask('link-dep', 'Copy npm dependencies', function () {
        var done = this.async(); // <-- must be async 
        return depLinker.linkDependenciesTo('Scripts/dep')
            .then(() => done())
            .catch(() => done());
    });
};

