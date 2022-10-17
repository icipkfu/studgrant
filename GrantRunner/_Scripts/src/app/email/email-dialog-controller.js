'use strict';

/**
 * @ngdoc function
 * @name ComposeController
 * @module triAngularEmail
 * @kind function
 *
 * @description
 *
 * Handles actions in compose dialog
 */

angular.module('triAngularEmail')
.controller('EmailDialogController', function ($scope, $timeout, $mdDialog, $filter, triSkins, textAngularManager, title, email, contacts, getFocus) {
    $scope.title = title;
    $scope.email = email;
    $scope.contacts = contacts.data;
    $scope.showCCSIcon = 'icon-arrow-drop-down';
    $scope.showCCS = false;

    $scope.triSkin = triSkins.getCurrent();

    $scope.queryContacts = function($query) {
        var lowercaseQuery = angular.lowercase($query);
        return $scope.contacts.filter(function(contact) {
            var lowercaseName = angular.lowercase(contact.name);
            if (lowercaseName.indexOf(lowercaseQuery) !== -1) {
                return contact;
            }
        });
    };

    $scope.toggleCCS = function() {
        $scope.showCCS = !$scope.showCCS;
        $scope.showCCSIcon = $scope.showCCS ? 'icon-arrow-drop-up' : 'icon-arrow-drop-down';
    };

    $scope.send = function() {
        $mdDialog.hide($scope.email);
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };

    if(getFocus) {
        $timeout(function() {
            // Retrieve the scope and trigger focus
            var editorScope = textAngularManager.retrieveEditor('emailBody').scope;
            editorScope.displayElements.text.trigger('focus');
        }, 500);
    }
});