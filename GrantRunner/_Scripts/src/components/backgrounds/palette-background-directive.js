'use strict';

/**
* @ngdoc directive
* @name paletteBackground
* @restrict A
* @scope
*
* @description
*
* Adds a palette colour and contrast CSS to an element
*
* @usage
* ```html
* <div palette-background="green:500">Coloured content</div>
* ```
*/
angular.module('triAngular')
.directive('paletteBackground', function (triTheming) {
    return {
        restrict: 'A',
        link: function ($scope, $element, attrs) {
            var splitColor = attrs.paletteBackground.split(':');
            var color = triTheming.getPaletteColor(splitColor[0], splitColor[1]);

            if(color !== undefined) {
                $element.css({
                    'background-color': triTheming.rgba(color.value),
                    'border-color': triTheming.rgba(color.value),
                    'color': triTheming.rgba(color.contrast)
                });
            }
        }
    };
});
