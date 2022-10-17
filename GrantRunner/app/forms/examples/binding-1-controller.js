'use strict';

/**
 * @ngdoc function
 * @name BindingController
 * @module triAngularElements
 * @kind function
 *
 * @description
 *
 * Handles binding forms page
 */
angular.module('triAngularForms').
controller('Binding1Controller', function ($scope) {
    $scope.user = {
        username: 'Morris',
        password: '',
        description: '',
        favouriteColor: ''
    };
});