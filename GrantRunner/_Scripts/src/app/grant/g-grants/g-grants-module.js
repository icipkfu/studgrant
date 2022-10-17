'use strict';

angular.module("g-grants", [])
    .config([
        "$translatePartialLoaderProvider", "$stateProvider", function(e, n) {
            
            e.addPart("app/grant/g-grants"),
                n.state("admin-panel.default.g-grants-list", {
                    url: "/grant/list",
                    templateUrl: "app/grant/g-grants/g-grants-list.tmpl.html",
                    controller: "GrantsController"
                }),
                n.state("admin-panel.default.g-grants-create", {
                    url: "/grant/edit/:param",
                    templateUrl: "app/grant/g-grants/g-grant-create.tmpl.html",
                    controller: "GrantsController"
                }),
                n.state("admin-panel.default.g-grants-timeline", {
                    url: "/grant/timeline/:param",
                    templateUrl: "app/grant/g-grants/g-grant-timeline.tmpl.html",
                    controller: "GrantsController"
                }),
                n.state("admin-panel.default.g-grants-stage", {
                    url: "/grant/stage/:param",
                    templateUrl: "app/grant/g-grants/g-grant-stage.tmpl.html",
                    controller: "GrantsController"
                }),
                n.state("admin-panel.default.g-grants-admins", {
                    url: "/grant/admins/:param",
                    templateUrl: "app/grant/g-grants/g-grant-admins.tmpl.html",
                    controller: "GrantsController"
                }),
                n.state("admin-panel.default.g-grants-unv", {
                    url: "/grant/university/:param",
                    templateUrl: "app/grant/g-grants/g-grant-unv.tmpl.html",
                    controller: "GrantsController"
                }),
                n.state("admin-panel.default.g-grants-people", {
                    url: "/grant/people/:param/:univerId",
                    templateUrl: "app/grant/g-grants/g-grant-people.tmpl.html",
                    controller: "GrantsController"
                }),
                n.state("admin-panel.default.g-grants-people-active", {
                    url: "/grant/active/:param/:univerId",
                    templateUrl: "app/grant/g-grants/g-grant-people-active.tmpl.html",
                    controller: "GrantsController"
                }),
                n.state("admin-panel.default.g-grants-people-social", {
                    url: "/grant/social/:param/:univerId",
                    templateUrl: "app/grant/g-grants/g-grant-people-social.tmpl.html",
                    controller: "GrantsController"
                }),

                n.state("admin-panel.default.g-grants-additional", {
                    url: "/grant/additional/:param",
                    templateUrl: "app/grant/g-grants/g-grant-additional.tmpl.html",
                    controller: "GrantsController"
                }),
                n.state("admin-panel.default.g-grants-description", {
                    url: "/grant/description/:param",
                    templateUrl: "app/grant/g-grants/g-grant-description.tmpl.html",
                    controller: "GrantsController"
                }),
                n.state("admin-panel.default.g-grants-stat", {
                    url: "/grant/stat/:param",
                    templateUrl: "app/grant/g-grants/g-grant-stat.tmpl.html",
                    controller: "GrantsController"
                }),
                n.state("admin-panel.default.g-grants-reports", {
                    url: "/grant/reports/:param",
                    templateUrl: "app/grant/g-grants/g-grant-reports.tmpl.html",
                    controller: "GrantsController"
                }),
                n.state("admin-panel.default.g-grants-participate", {
                    url: "/grant/participate/:param/:code",
                    templateUrl: "app/grant/g-grants/g-grant-participate.tmpl.html",
                    controller: "GrantsController"
                });


        }
    ])
    .factory("grantResource", [
        '$resource', "API_CONFIG", function(res, apiConfig) {
            return res(
                apiConfig.serverPath + "api/grant", 
                {
                    'get': {method:'GET', isArray:true},
                });
        }
    ]);