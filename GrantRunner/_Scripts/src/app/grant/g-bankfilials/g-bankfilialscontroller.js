'use strict';

        angular.module("g-bankfilials")
        .controller("BankFilialsController", 
            [ "$scope", "grantResource","FilialBankResource", "$mdToast", "$filter", "$state", "gProgress", 'FileUploader' , 'API_CONFIG', '$http' , '$stateParams', '$mdDialog', '$window', '$timeout', 
            function (e, grantResource, FilialBankResource, $mdToast, $filter, $state, gProgress, FileUploader, API_CONFIG, $http, $stateParams, $mdDialog, $window, $timeout) 
                {

                  e.columns = [
                    {
                        title: "Код",
                        field: "Code",
                        sortable: true
                    }, 
                     {
                        title: "Имя",
                        field: "FilialName",
                        sortable: true
                    },{
                        title: "Адрес",
                        field: "Address",
                        sortable: true
                    }
                ];

                   
            e.loadData =  function () {


               gProgress.mask(e);
              $http.get(API_CONFIG.serverPath +'api/bankfilial')
              .success(function (data, status) {

                 var result = [];
                  for (var i = 0; i < data.length; i++) {
                      result.push({
                          Code:  data[i].Code ,
                          FilialName: data[i].FilialName,
                          Address: data[i].Address
                      });

                      //e.setPageTitle('');
                  }
                 
                      e.contents = result;
                      gProgress.unmask(e);
                  

                }).error(function (error) {

                  $mdToast.show(
                      $mdToast.simple()
                      .content(error.data.Message)
                      .theme("error-toast")
                      .position('top'));
                });  
          };

                       e.loadData();

                       e.goCreateBankFilial = function(){

                        $state.go('admin-panel.default.g-bankfilials-create');
                       }


                    e.createBankFilial = function () {

                      var newFilial = new FilialBankResource({
                        Code : e.bankfilial.Code,
                        FilialName: e.bankfilial.FilialName,
                        Address : e.bankfilial.Address
                      });
      
                      gProgress.mask(e);
                      newFilial.$save(function (resp) {
                          gProgress.unmask(e);
                          $mdToast.show(
                              $mdToast.simple()
                              .content('Филиал успешно создан')
                              .position('top'));
      
                            $state.go('admin-panel.default.g-bankfilials');
                
                          
                      }, function (error) {
                          // failure
                
                $mdToast.show(
                              $mdToast.simple()
                              .content(error.data.Message)
                  .theme("error-toast")
                              .position('top'));
                  
                      });
                      //universityResource.query(function(resp) {
                      //    //alert(resp.data);
                      //});
                  }
            
                } 
            ]);