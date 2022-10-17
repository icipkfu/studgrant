'use strict';

/**
 * @ngdoc filter
 * @name capitalize
 * @module triAngularCalendar
 * @kind filter
 *
 * Capitalizes words
 */
angular.module('triAngularCalendar')
.filter('capitalize', function () {
    return function(input, all) {
        return (!!input) ? input.replace(/([^\W_]+[^\s-]*) */g, function(txt) {
            return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase();
        }) : '';
    };
});
