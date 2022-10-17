'use strict';

        angular.module("g-contacts")
        .controller("ContactsController", 
            [ "$scope", "grantResource", "$mdToast", "$filter", "$state", "gProgress", 'FileUploader' , 'API_CONFIG', '$http' , '$stateParams', '$mdDialog', '$window', '$timeout', 
            function (e, grantResource, $mdToast, $filter, $state, gProgress, FileUploader, API_CONFIG, $http, $stateParams, $mdDialog, $window, $timeout) 
                {

          

                    e.loadcontactInfo =  function()
                      {
                        gProgress.mask(e);

                        $http.get(API_CONFIG.serverPath + 'api/students/getcontactinfo')
                          .success(function (data, status) {
                             gProgress.unmask(e);

                              e.contactInfo = data.Data;

                            });
                      }





                       e.loadcontactInfo();
                   

            
                } 
            ]);