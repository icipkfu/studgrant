'use strict';

/**
 * @ngdoc function
 * @name FooterController
 * @module triAngular
 * @kind function
 *
 * @description
 *
 * Handles the footer view
 */
angular.module('triAngular').
controller('FooterController', function ($scope, APP) {
    $scope.footerInfo = {
        appName: APP.name,
        appLogo: APP.logo,
        date: new Date(),
        version: APP.version
    };
});