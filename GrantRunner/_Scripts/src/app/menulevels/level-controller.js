'use strict';

/**
 * @ngdoc function
 * @name LevelController
 * @module triAngular
 * @kind function
 *
 * @description
 *
 * Handles basic Menu Level Template
 */
angular.module('triAngular')
.controller('LevelController', function ($scope, $stateParams) {
    $scope.level = $stateParams.level;
});