'use strict';

/**
 * @ngdoc function
 * @name EmailToolbarController
 * @module triAngularEmail
 * @kind function
 *
 * @description
 *
 * Handles all actions for email toolbar
 */

angular.module('triAngularEmail')
.controller('EmailToolbarController', function ($scope, $rootScope, $filter, $mdUtil, $mdSidenav, $state, SideMenu, EMAIL_ROUTES) {
    $scope.showSearch = false;
    $scope.toolbarMenu = [];
    $scope.menu = SideMenu.getMenu();

    for(var i = 0; i < EMAIL_ROUTES.length; i++) {
        $scope.toolbarMenu.push({
            name: $filter('translate')(EMAIL_ROUTES[i].name),
            state: EMAIL_ROUTES[i].state,
            icon: EMAIL_ROUTES[i].icon,
        });
    }

    $scope.filterEmailList = function() {
        $rootScope.$broadcast('emailSearch', $scope.emailSearch);
    };

    $scope.toggleSearch = function() {
        $scope.showSearch = !$scope.showSearch;
    };

    /**
     * Build handler to open/close a SideNav;
     */
    $scope.openSideNav = function(navID) {
        $mdUtil.debounce(function(){
            $mdSidenav(navID).toggle();
        }, 300)();
    };
});