'use strict';

/**
 * @ngdoc filter
 * @name tableImage
 * @module triAngular
 * @kind filter
 *
 * Used for table pagination
 */
angular.module('triAngular')
.filter('startFrom',function () {
    return function (input, start) {
        start = +start;
        return input.slice(start);
    };
});