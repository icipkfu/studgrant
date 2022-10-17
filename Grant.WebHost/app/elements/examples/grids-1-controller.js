'use strict';

/**
 * @ngdoc function
 * @name GridsController
 * @module triAngularElements
 * @kind function
 *
 * @description
 *
 * Handles dialog element page
 */
angular.module('triAngularElements').
controller('Grids1Controller', function ($scope, triTheming) {


    this.colorTiles = (function() {
        var tiles = [];
        for (var i = 0; i < 46; i++) {
            var tile = {
                colspan: randomSpan(),
                rowspan: randomSpan()
            };
            setColors(tile);
            tiles.push(tile);
        }
        return tiles;
    })();

    function setColors(tile) {
        var result;
        var count = 0;
        for (var palette in triTheming.palettes) {
            if (Math.random() < 1 / ++count && palette !== 'white') {
                tile.palette = palette;
                result = triTheming.palettes[palette];
            }
        }

        tile.hue = Math.floor((Math.random() * 9) + 1) * 100;
        tile.color = triTheming.rgba(result[tile.hue].value);
    }

    function randomSpan() {
        var r = Math.random();
        if (r < 0.8) {
            return 1;
        } else if (r < 0.9) {
            return 2;
        } else {
            return 3;
        }
    }
});