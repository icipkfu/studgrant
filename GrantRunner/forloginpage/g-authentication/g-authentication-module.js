'use strict';

/**
 * @ngdoc module
 * @name triangular.authentication
 * @description
 *
 * The `triangular.authentication` module handles all the login and signup pages
 */
angular.module('g-authentication', [])
    .factory("authenticationResource", [
        '$resource', "API_CONFIG", function (res, apiConfig) {
            return res(apiConfig.serverPath + "api/Account");
        }
    ])
.config(function ($translatePartialLoaderProvider, $stateProvider) {
    $translatePartialLoaderProvider.addPart('app/authentication');

    $stateProvider
    .state('authentication', {
        abstract: true,
        templateUrl: 'app/grant/g-authentication/layouts/authentication.tmpl.html',
    })
    .state('authentication.root', {
        url: '',
        templateUrl: 'app/grant/g-authentication/login/login.tmpl.html',
        controller: 'LoginController'
    })
    .state('authentication.login', {
        url: '/login/:email',
        templateUrl: 'app/grant/g-authentication/login/login.tmpl.html',
        controller: 'LoginController'
    })
    .state('authentication.signup', {
        url: '/signup',
        templateUrl: 'app/grant/g-authentication/signup/signup.tmpl.html',
        controller: 'SignupController'
    })
    .state('authentication.forgot', {
        url: '/forgot',
        templateUrl: 'app/grant/g-authentication/forgot/forgot.tmpl.html',
        controller: 'ForgotController'
    })



})
.run(function(SideMenu) {
    //SideMenu.addMenu({
    //    name: 'MENU.AUTH.AUTH',
    //    icon: 'icon-person',
    //    type: 'dropdown',
    //    priority: 4.1,
    //    children: [{
    //        name: 'MENU.AUTH.LOGIN',
    //        state: 'authentication.login',
    //        type: 'link',
    //    },{
    //        name: 'MENU.AUTH.SIGN_UP',
    //        state: 'authentication.signup',
    //        type: 'link',
    //    },{
    //        name: 'MENU.AUTH.FORGOT',
    //        state: 'authentication.forgot',
    //        type: 'link',
    //    },{
    //        name: 'MENU.AUTH.LOCK',
    //        state: 'authentication.lock',
    //        type: 'link',
    //    },{
    //        name: 'MENU.AUTH.PROFILE',
    //        state: 'admin-panel.default.profile',
    //        type: 'link',
    //    }]
    //});
});

