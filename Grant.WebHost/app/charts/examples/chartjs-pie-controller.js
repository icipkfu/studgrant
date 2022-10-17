'use strict';

/**
 * @ngdoc function
 * @name GoogleBarController
 * @module triAngularElements
 * @kind function
 *
 * @description
 *
 * Handles bar chart
 */
angular.module('triAngularElements').
controller('ChartJsPieController', function ($scope, $interval) {
    $scope.labels = ['Download Sales', 'Instore Sales', 'Mail Order'];

    $scope.options = {
        datasetFill: false
    };

    function randomData() {
        $scope.data = [];
        for(var label = 0; label < $scope.labels.length; label++) {
            $scope.data.push(Math.floor((Math.random() * 100) + 1));
        }
    }

    randomData();

    // Simulate async data update
    $interval(randomData, 5000);
});
