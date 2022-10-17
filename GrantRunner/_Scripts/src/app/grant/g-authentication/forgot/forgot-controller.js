'use strict';

/**
 * @ngdoc function
 * @name ForgotController
 * @module triAngularAuthentication
 * @kind function
 *
 * @description
 *
 * Handles forgot password form submission and response
 */
angular.module('g-authentication')
.controller('ForgotController', function ($scope, $state, $mdToast, $filter, $http, API_CONFIG, gProgress) {
    // create blank user variable for login form
    $scope.user = {
        email: '',
    };

    // controller to handle login check
    $scope.resetClick = function() {

        gProgress.mask($scope);

        $http({
            method: 'POST',
            url: API_CONFIG.serverPath +'api/Account/ResetPasswordRequest/',
            data: $scope.user
        }).
        success(function(data) {
            gProgress.unmask($scope);

            $mdToast.show(
                $mdToast.simple()
                .content($filter('translate')('Письмо c новым паролем отправлено Вам на почту. Проверьте также папку СПАМ!') + ' ' + $scope.user.email)
                .position('top center'));

            $scope.restored = true;
        }).
        error(function(data) {
            gProgress.unmask($scope);

            $mdToast.show(
                $mdToast.simple()
                .content($filter('translate')(data.Message))
                .position('top center')
                .hideDelay(5000)
            );
        });
    };
});