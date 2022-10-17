'use strict';

/**
 * @ngdoc function
 * @name SkinsUIController
 * @module triAngularUI
 * @kind function
 *
 * @description
 *
 * Handles the toolbar ui page
 */
angular.module('triAngularUI').
controller('SkinsUIController', function ($scope, $cookies, $window, triSkins, triTheming) {
    $scope.skins = triSkins.getSkins();
    $scope.selectedSkin = triSkins.getCurrent();

    $scope.trySkin = function() {
        if($scope.selectedSkin !== triSkins.getCurrent()) {
            $cookies.put('triangular-skin',angular.toJson({
                skin: $scope.selectedSkin.id
            }));            
            $window.location.reload();
        }
    };

    $scope.elementColors = {
        logo: '',
        sidebar: '',
        content: '',
        toolbar: ''
    };

    $scope.updatePreview = function() {
        for(var element in $scope.elementColors) {
            var theme = triTheming.getTheme($scope.selectedSkin.elements[element]);
            var hue = theme.colors.primary.hues.default === undefined ? '500' : theme.colors.primary.hues.default;
            var color = triTheming.getPaletteColor(theme.colors.primary.name, hue);
            $scope.elementColors[element] = triTheming.rgba(color.value);
        }
    };

    $scope.updatePreview();
});