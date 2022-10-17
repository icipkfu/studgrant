'use strict';

/**
 * @ngdoc function
 * @name ToastsController
 * @module triAngularElements
 * @kind function
 *
 * @description
 *
 * Handles toasts element page
 */
angular.module('triAngularElements').
controller('Toast1Controller', function ($scope, $mdToast) {
    $scope.showToast = function($event, position) {
        var $button = angular.element($event.currentTarget);
        $mdToast.show({
            template: '<md-toast><span flex>I\'m a toast</span></md-toast>',
            position: position,
            hideDelay: 3000,
            parent: $button.parent()
        });
    };
});