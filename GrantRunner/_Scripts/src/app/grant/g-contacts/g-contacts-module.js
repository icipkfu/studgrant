'use strict';

angular.module("g-contacts", [])
    .config([
        "$translatePartialLoaderProvider", "$stateProvider", function(e, n) 
        {
            
            e.addPart("app/contacts"),
              n.state("admin-panel.default.g-contacts", {
                    url: "/contacts",
                    templateUrl: "app/grant/g-contacts/g-contacts.tmpl.html",
                    controller: "ContactsController"
                });

        }
    ]);