'use strict';

/**
 * @ngdoc function
 * @name ListsController
 * @module triAngularElements
 * @kind function
 *
 * @description
 *
 * Handles dialog element page
 */
angular.module('triAngularElements').
controller('ListsController', function ($scope, emails) {
    $scope.emails = emails.data.splice(0, 5);
});