'use strict';

/**
 * @ngdoc function
 * @name ColorsController
 * @module triAngularUI
 * @kind function
 *
 * @description
 *
 * Handles the colors ui page
 */
angular.module('triAngularUI').
controller('ColorsController', function ($scope, $mdDialog, triTheming) {
    $scope.palettes = triTheming.palettes;

    $scope.colourRGBA = function(value) {
        var rgba = triTheming.rgba(value);
        return {
            'background-color': rgba
        };
    };

    $scope.selectPalette = function($event, name, palette) {
        $mdDialog.show({
            controller: 'ColorDialogController',
            templateUrl: 'app/ui/color-dialog.tmpl.html',
            targetEvent: $event,
            locals: {
                name: name,
                palette: palette
            },
            clickOutsideToClose: true
        })
        .then(function(answer) {
            $scope.alert = 'You said the information was "' + answer + '".';
        }, function() {
            $scope.alert = 'You cancelled the dialog.';
        });
    };
});