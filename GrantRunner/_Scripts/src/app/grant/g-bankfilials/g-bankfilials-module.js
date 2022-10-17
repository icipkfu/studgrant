'use strict';

angular.module("g-bankfilials", [])
    .config([
        "$translatePartialLoaderProvider", "$stateProvider", function(e, n) 
        {
            
            e.addPart("app/contacts"),
              n.state("admin-panel.default.g-bankfilials", {
                    url: "/bankfilials",
                    templateUrl: "app/grant/g-bankfilials/g-bankfilials.tmpl.html",
                    controller: "BankFilialsController"
                }),
                n.state("admin-panel.default.g-bankfilials-create", {
                    url: "/bankfilials/create/:param",
                    templateUrl: "app/grant/g-bankfilials/g-bankfilials-create.tmpl.html",
                    controller: "BankFilialsController"
                });

        }
    ]).factory("FilialBankResource", [
        '$resource', "API_CONFIG", function(res, apiConfig) {
            return res(apiConfig.serverPath + "api/bankfilial");
        }
    ]);
