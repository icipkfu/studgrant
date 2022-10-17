'use strict';

angular.module("g-progress-module", [])
    //.config([])
    //.run([])
    .factory("gProgress", [
        function () {

            function mask(scope, val) {
                if (val) {
                    val = true;
                } else if (scope && scope.inProgress == undefined) {
                    scope.inProgress = true;
                }
            };

            function unmask(scope, val) {
                if (val) {
                    val = false;
                } else if (scope && scope.inProgress) {
                    scope.inProgress = undefined;
                }
            }

            return{
                mask: mask,
                unmask: unmask
            };
        }
    ]); 
        
