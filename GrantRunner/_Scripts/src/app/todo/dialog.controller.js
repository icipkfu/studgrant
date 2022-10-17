'use strict';

/**
 * @ngdoc function
 * @name TodoController
 * @module triAngularTodo
 * @kind function
 *
 * @description
 *
 * Handles the todo app model and controls
 */
angular.module('triAngularTodo')
.controller('DialogController', function ($scope, $state, $mdDialog) {    
    
    $scope.item = {
        description: '',
        priority: '',
        selected: false
    };
    
    $scope.hide = function() {
        $mdDialog.hide($scope.item);
    };
    
    $scope.cancel = function() {
        $mdDialog.cancel();
    };
    
});