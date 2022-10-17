'use strict';

/**
 * @ngdoc function
 * @name SidebarsController
 * @module triAngularElements
 * @kind function
 *
 * @description
 *
 * Handles sidebar element page
 */
angular.module('triAngularElements').
controller('SidebarsController', function ($scope, $mdSidenav) {
    $scope.openSidebar = function(id) {
        $mdSidenav(id).toggle();
    };
});