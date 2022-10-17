'use strict';

angular.module("g-universities")
    .controller("UniversitiesController", [
        "$scope", "universityResource", "$mdToast", "$filter", "$state", "gProgress",  'FileUploader' , 'API_CONFIG', '$http' , '$stateParams', '$mdDialog', '$window', '$timeout', '$q',
        function (e, universityResource, $mdToast, $filter, $state, gProgress,   FileUploader, API_CONFIG, $http, $stateParams, $mdDialog, $window, $timeout, $q) {

    
            var self = this;

            e.selected = null;
            e.columns = [
                {
                    title: "",
                    field: "thumb",
                    sortable: false,
                    filter: "tableImage"
                },{
                    title: "Тип",
                    field: "UniverType",
                    sortable: true
                }, 
                 {
                    title: "Зона",
                    field: "City",
                    sortable: true
                },{
                    title: "Город",
                    field: "Town",
                    sortable: true
                }, {
                    title: "Наименование",
                    field: "Name",
                    sortable: true
                }, {
                    title: "Куратор",
                    field: "Curator",
                    sortable: true
                }, {
                    title: "",
                    field: "link",
                    sortable: true
                }
            ];

            e.contents = [];
            e.students = [];






            e.loadData =  function () {



                if(e.UsersQuery){
                    e.UsersQuery += 1;
                }else{
                    e.UsersQuery = 1;
                }

                var localQueryNum = e.UsersQuery ;

                if(e.filterCity && e.filterCity.length > 0){

                } else {
                    e.filterCity = null;
                }

                var searchFilter = {
                    Type : e.filterType ? e.filterType : null,
                    Search : e.filterCity ? e.filterCity   : null
                };


                 gProgress.mask(e);
                $http.put(API_CONFIG.serverPath +'api/filteredUniversity', searchFilter)
                .success(function (data, status) {

                    
                     if(localQueryNum < e.UsersQuery ){
                        return;
                     }



                    var result = [];
                    for (var i = 0; i < data.length; i++) {
                        result.push({
                            Name:  data[i].Name ,
                            Curator: data[i].CuratorFio,
                            thumb: data[i].thumb,
                            City: data[i].City,
                            Town: data[i].Town,
                            UniverType: data[i].UniverType == 1 ? 'ВО' : 'СПО',
                            link: '<a href="/my#/universities/students/' + data[i].Id +'">Просмотр</a>' 
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




            e.applyFilter = function() {

                e.loadData();
            }


             e.setPageTitle = function(title) {

                var pagetitle = $('.admin-toolbar .breadcrumb');
               
                $(pagetitle).each(function(index, item) {

                  if( $(item).hasClass('ng-hide') == false) { 
                   $(item).text(title); 
                  }
                });
            }


            e.askdeleteUniversity = function (){

                $mdDialog.show(
                    $mdDialog.confirm()
                    .title('Удаление ВУЗа')
                    .content("Вы уверены, что хотите удалить ВУЗ?")
                    .ok("Да")
                    .cancel("Отмена")).then(function(result){

                      if(result){
                          e.deleteUniversity();
                      }  
                });   
            }



            e.deleteUniversity = function(){

                $http.delete(API_CONFIG.serverPath +'api/university/' + universityId)
                .success(function (data, status) {
                     gProgress.unmask(e);

                     $mdToast.show(
                        $mdToast.simple()
                        .content('ВУЗ успешно удален')
                        .position('top'));

                      $state.go('admin-panel.default.g-university-list');

                  }).error(function (error) {
                    var msg = "Произошла ошибка";

                    if(error.data && error.data.Message) { msg = error.error.data.Message }
                      if(error.ExceptionMessage) { msg = error.ExceptionMessage }
                       $mdToast.show(
                        $mdToast.simple().content(msg).theme("error-toast").position('top'));
                   });
            }

           

            e.student_columns = [
                {
                    title: "",
                    field: "thumb",
                    sortable: false,
                    filter: "tableImage"
                }, {
                    title: "ФИО",
                    field: "Name",
                    sortable: true
                }, {
                    title: "Факультет/Группа",
                    field: "Departament",
                    sortable: true
                },
                {
                    title: "",
                    field: "button",
                    sortable: false,
                    filter: "button"
                }
            ];


             function querySearch(value) {
                if (value.length > 1) {
                     var promise = $http.get(API_CONFIG.serverPath + 'api/students/getbyname/'+ value ),
                        deferred = $q.defer();

                    promise.then(        
                        function(resp) {
                            
                            var result = new Array(),
                                data = resp.data;
                            for (var i = 0; i < data.length; i++) {
                                result.push({
                                    value: data[i].Id,
                                    display: data[i].Name + ' ' +  data[i].LastName + ' ' + data[i].Patronymic,
                                    thumb: data[i].ImageLink
                                });
                            }
                            deferred.resolve(result);
                        },
                        function(reason) {

                        });
                    return deferred.promise;
                };
            };

            
            e.checkCuratorTrigger = null;
            e.checkCuratorField = function(){
                if(e.selectedItem) {} else {
                   $('md-autocomplete-wrap input').val('');
                }  
            }    

            function selectedItemChange(item) {
                if(item){
                    e.university.CuratorId = item.value;

                    e.university.Curator = {};
                    e.university.Curator.Id = item.value;
                }
                else
                {
                    e.university.CuratorId = 0;

                    e.university.Curator = null;

                    if(e.checkCuratorTrigger) {} else{
                        $('md-autocomplete-wrap input').blur(e.checkCuratorField);
                    }
                }
               
            };

            function selectedItemChangeBank(item) {
                if(item){
                    e.university.BankFilialId = item.value;

                    e.university.BankFilial = {};
                    e.university.BankFilial.Id = item.value;
                }
                else
                {
                    e.university.BankFilialId = 0;

                    e.university.BankFilial = null;
                }
               
            };

            self.states             = null;
            self.selectedItem       = null;
            self.searchText         = null;
            self.querySearch = querySearch;
            self.selectedItemChange = selectedItemChange;
            self.simulateQuery      = false;
            self.isDisabled         = false;

            self.statesBank            = null;
            self.selectedItemBank       = null;
            self.searchTextBank         = null;
            self.querySearchBank = querySearchBank;
            self.selectedItemChangeBank = selectedItemChangeBank;
            self.simulateQueryBank      = false;
            self.isDisabled         = false;

            e.goCreateUniversity = function(){

                $state.go('admin-panel.default.g-university-create');
            }


            function querySearchBank(value) {

                if(value=="")
                    value = "-1";

               if (value.length >= 0) {
                    var promise = $http.get(API_CONFIG.serverPath + 'api/students/bankfilialname/'+ value ),
                       deferred = $q.defer();

                   promise.then(        
                       function(resp) {                      
                           var result = new Array(),
                               data = resp.data;
                           for (var i = 0; i < data.length; i++) {
                               result.push({
                                   value: data[i].Id,
                                   display: data[i].FilialName
                               });
                           }
                           deferred.resolve(result);
                       },
                       function(reason) {

                       })
                       ;
                   return deferred.promise;
               };
           };



            e.createUniversity = function () {

                var newUniv = new universityResource({
                    Name: e.university.Name,
                    ImageFile : e.university.imagefile,
                    CuratorId : e.university.CuratorId,
                    City: e.university.City,
                    Town: e.university.Town,
                    UniverType : e.university.UniverType,
                    BankFilialId: e.university.BankFilialId

                });

                gProgress.mask(e);
                newUniv.$save(function (resp) {
                    gProgress.unmask(e);
                    $mdToast.show(
                        $mdToast.simple()
                        .content('Вуз успешно создан')
                        .position('top'));

                      $state.go('admin-panel.default.g-university-list');
					
                    e.university = {
                        name: "",
                        curator: "",
                        city: "",
                        town: ""
                    };
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
            e.select = function (content) {
                var ss = content;
                var s = ss;
            }

            e.delete = function (smth) {
                var a = smth;
                var ss = a;
            }

            e.cancel = function () {
                e.university.Name = "";
                e.university.curator = "";
                e.university.City = "";
                e.university.Town = "";
            };

             if(e.university){

             }else
             {
                e.university = {};
             }


            // страница студентов Универа

             e.peopleColumns = [
                {
                    title: "",
                    field: "thumb",
                    sortable: false,
                    filter: "tableImage"
                }, {
                    title: "Фамилия Имя Отчество",
                    field: "Fio",
                    sortable: true
                },{
                    title: "Факультет",
                    field: "Departament",
                    sortable: true
                },{
                    title: "Баллы",
                    field: "Score",
                    sortable: true
                },
                /* {
                    title: "Валидация данных паспорта",
                    field: "IsPassportValid",
                    sortable: true
                }, */
                 {
                    title: "Валидация зачетной книжки",
                    field: "IsStudentBookValid",
                    sortable: true
                },
                {
                    title: "",
                    field: "Link",
                    sortable: false
                }

            ];


            e.loadUniverStudents = function(universityId){

                gProgress.mask(e);

               $http.get(API_CONFIG.serverPath +'api/students/getbyuniversity/' + universityId)
                .success(function (data, status) {
                     gProgress.unmask(e);

                    var result = [];
                    for (var i = 0; i < data.length; i++) {
                        result.push({
                            Fio: data[i].Fio,
                            Curator: data[i].Curator,
                            thumb: data[i].thumb,
                            Departament: data[i].Departament,
                            Score: data[i].Score,
                            IsPassportValid : data[i].IsPassportValid,
                            IsStudentBookValid : data[i].IsStudentBookValid,
                            Link: '<a href="/my#/user/page/' + data[i].Id +'">Просмотр</a>' 

                        });
                    }

                     e.students = result;

                    gProgress.unmask(e);

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });  
            }


             e.makeCheckboxGrid = function(){


                var t = e;

                var quotacolumns = [];

                  $("md-table tr").each(function(index, item) {

                    if(index > 0){

                      var cols = $(item).find('td');

                     // var paspValid = $(cols[4]).text();
                      var bookValid = $(cols[4]).text(); // cols[5]

                          if(bookValid.length > 0) {

                         
                              $(cols[4]).html('<input type="checkbox"/>');
                             // $(cols[5]).html('<input type="checkbox"/>');

                              $(cols[4]).find('input').prop('checked', (bookValid == 'true')); //paspValid
                            //  $(cols[5]).find('input').prop('checked', (bookValid == 'true'));

                              $(cols[4]).find('input').attr('readonly',true);
                            //  $(cols[5]).find('input').attr('readonly',true);

                              $(cols[4]).find('input').attr('disabled','disabled');
                            //  $(cols[5]).find('input').attr('disabled','disabled');
                          }

                    }

                  });
              }

              e.univerImageClick = function() {

                 $("#univerFileInput").click();
              }

              var uploader = e.uploader = new FileUploader({
                    url: API_CONFIG.serverPath +'api/file'
                });

                // FILTERS

                uploader.filters.push({
                    name: 'imageFilter',
                    fn: function(item /*{File|FileLikeObject}*/, options) {
                        var type = '|' + item.type.slice(item.type.lastIndexOf('/') + 1) + '|';
                        return '|jpg|png|jpeg|bmp|gif|'.indexOf(type) !== -1;
                    }
                });

                // CALLBACKS

                uploader.onWhenAddingFileFailed = function(item /*{File|FileLikeObject}*/, filter, options) {
                    console.info('onWhenAddingFileFailed', item, filter, options);
                };
                
                uploader.onAfterAddingFile = function(fileItem)  {
                    var t = this;

                    if(t.queue.length > 1)
                    {
                        t.removeFromQueue(t.queue[0]);
                    }

                    fileItem.upload();
                };
                
                uploader.onCompleteItem = function(fileItem, response, status, headers) {

                    if(status == 200)
                    {
                        e.university.ImageFile =  response;
                    }
                    else
                    {
                        $mdToast.show(
                        $mdToast.simple()
                        .content(response)
                        .theme("error-toast")
                        .position('top'));
                    }
                    console.info('onCompleteItem', fileItem, response, status, headers);
                };

                uploader.onCompleteAll = function() {
                    console.info('onCompleteAll');
                };


            e.loadUniversity = function (id) {
                gProgress.mask(e);

                $http.get(API_CONFIG.serverPath +'api/University/getbyid/' + id)
                .success(function (data, status) {

                     e.university = data;
                     gProgress.unmask(e);

                     if(data.Curator) {

                         self.selectedItem = {
                            value: data.Curator.Id,
                            display: data.Curator.Name + ' ' +  data.Curator.LastName + ' ' + data.Curator.Patronymic,
                            thumb: data.Curator.thumb
                        };
                    }

                    if(data.BankFilial) {

                        self.selectedItemBank = {
                           value: data.BankFilial.Id,
                           display: data.BankFilial.FilialName
                       };
                   }

                    e.setPageTitle(e.university.Name);



                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));


                  });
              
            };

             e.updateUniversity = function (){

              var updUniv = new universityResource({
                    Id: e.university.Id,
                    Name: e.university.Name,
                    ImageFile : e.university.ImageFile,
                    CuratorId : e.university.CuratorId,
                    City : e.university.City,
                    Town : e.university.Town,
                    UniverType: e.university.UniverType,
                    BankFilialId : e.university.BankFilialId
                });

              gProgress.mask(e);

              $http.put(API_CONFIG.serverPath + "api/university/"+ e.university.Id,  updUniv).success(function (data, status) {
                     gProgress.unmask(e);

                     $mdToast.show(
                        $mdToast.simple()
                        .content('Информация о ВУЗе успешно сохранена')
                        .position('top'));

                      $state.go('admin-panel.default.g-university-students', {'param': e.university.Id});

                  }).error(function (error) {
                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
            }


            var stateName = $state.current.name;


            e.goeditUniversity = function() {

                 $state.go('admin-panel.default.g-university-create', {'param': e.university.Id});

            }

             e.loadRole = function () {

                $http.get(API_CONFIG.serverPath + "api/students/getroleinfo")
                .then(function(resp){
                    e.roleInfo = resp.data;

                    if(e.roleInfo) {
                       e.isAdmin = (e.roleInfo.Role == 5);
                    }

                    if(e.roleInfo) {
                       e.isModerator = (e.roleInfo.Role == 4);
                    }


                    if( window.universityId && e.roleInfo.UniversCurator.indexOf(parseInt(window.universityId)) >= 0){
                        e.isCurator = true;
                    }

                    if(e.roleInfo.UniversCurator.length > 0){
                        e.isAnyCurator = true;
                    }

                    e.checkCurPageAccess();

               });

            }

            e.loadGrantBankFilial = function()
            {

               $http.get(API_CONFIG.serverPath +'api/bankfilial')
                .success(function (data, status) {
                    gProgress.unmask(e);

                     e.FilialBankList = data;

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });

            }


            e.changeAddFilialBank = function(){
               // e.loadGrantBankFilial();
            }

            e.checkCurPageAccess = function(){

              if(e.isAdmin)
              {
                  return;
              }

              var stateName = $state.current.name;

              if(stateName == 'admin-panel.default.g-university-list'){

                    if(e.isAnyCurator || e.isModerator){
                       return; 
                    }
              }

               if(stateName == 'admin-panel.default.g-university-students'){

                  if(e.isCurator || e.isModerator){
                       return; 
                    }
               }

                if(stateName == 'admin-panel.default.g-university-create'){

                  if(window.universityId && e.isCurator){
                       return; 
                    }
               }

               $mdToast.show(
                  $mdToast.simple()
                  .content('У Вас нет доступа к данному разделу')
                  .theme("error-toast")
                  .position('top'));


               $state.go('admin-panel.default.g-user-profile');
            }


            if($stateParams.param){
                window.universityId = $stateParams.param;
            }
            else
            {
                window.universityId = null;
            }

            e.loadRole();

            switch (stateName) {


                      case "admin-panel.default.g-university-create":

                      if(window.universityId){
                        e.loadUniversity(universityId);
                      }
                      e.loadGrantBankFilial();

                      case "admin-panel.default.g-university-list":

                      e.loadData();

                      break

                      case "admin-panel.default.g-university-students":


                      if(window.universityId) {

                          e.loadUniverStudents(universityId);
                          e.loadUniversity(universityId);

                           e.$on('ngRepeatFinished', function(ngRepeatFinishedEvent) {
                                   e.makeCheckboxGrid();
                                });
                        } else {
                             $state.go('admin-panel.default.g-university-list');
                        }

                      break
                  }
        }
    ]);