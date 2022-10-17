'use strict';

/**
 * @ngdoc function
 * @name SeedPageController
 * @module triSeedModule
 * @kind function
 *
 * @description
 *
 * Creates some dummy test data for the test seed page
 */

// get the triangular seed module
angular.module('triSeedModule')
// create a controller for the seed page and inject the scope directive
.controller('SeedPageController', function ($scope) {
    // add some test data to the scope
    $scope.testData = ['triangular', 'is', 'great'];
});