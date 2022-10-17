'use strict';

angular.module("g-timeline", [])
    .config([
        "$translatePartialLoaderProvider", "$stateProvider", function(e, n) 
        {
            
            e.addPart("app/timeline"),
              n.state("admin-panel.default.g-timeline", {
                    url: "/timeline",
                    templateUrl: "app/grant/g-timeline/g-timeline.tmpl.html",
                    controller: "TimeLineController"
                });

        }
    ])
    .factory("timelineResource", [
        '$resource', "API_CONFIG", function(res, apiConfig) {
            return res(
                apiConfig.serverPath + "api/timeline", 
                {
                    'get': {method:'GET', isArray:true},
                });
        }
    ]);