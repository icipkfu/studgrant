'use strict';

/**
 * @ngdoc function
 * @name LoginCtrl
 * @module triAngularAuthentication
 * @kind function
 *
 * @description
 *
 * Handles login form submission and response
 */
angular.module('triAngularAuthentication')
.controller('LoginController', function ($scope, $state) {
    // create blank user variable for login form
    $scope.user = {
        email: 'info@oxygenna.com',
        password: 'demo'
    };

    $scope.socialLogins = [{
        icon: 'fa-twitter',
        color: '#5bc0de',
        url: '#'
    },{
        icon: 'fa-facebook',
        color: '#337ab7',
        url: '#'
    },{
        icon: 'fa-google-plus',
        color: '#e05d6f',
        url: '#'
    },{
        icon: 'fa-linkedin',
        color: '#337ab7',
        url: '#'
    }];

    // controller to handle login check
    $scope.loginClick = function() {
        $state.go('admin-panel.default.introduction');
    };
});