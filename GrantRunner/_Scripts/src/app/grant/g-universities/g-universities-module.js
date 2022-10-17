'use strict';

angular.module("g-universities", [])
    .config([
        "$translatePartialLoaderProvider", "$stateProvider", function(e, n) {
            e.addPart("app/grant/g-universities"),
                n.state("admin-panel.default.g-university-list", {
                    url: "/universities/list",
                    templateUrl: "app/grant/g-universities/g-universities-list.tmpl.html",
                    controller: "UniversitiesController"
                }),
                n.state("admin-panel.default.g-university-create", {
                    url: "/universities/create/:param",
                    templateUrl: "app/grant/g-universities/g-university-create.tmpl.html",
                    controller: "UniversitiesController"
                }),
                n.state("admin-panel.default.g-university-students", {
                    url: "/universities/students/:param",
                    templateUrl: "app/grant/g-universities/g-university-students.tmpl.html",
                    controller: "UniversitiesController"
                });

        }
    ])
    .factory("universityResource", [
        '$resource', "API_CONFIG", function(res, apiConfig) {
            return res(apiConfig.serverPath + "api/university");
        }
    ]);