'use strict';

angular.module("g-user-profile", [])
    .config([
        "$translatePartialLoaderProvider", "$stateProvider", function(e, n) {
            e.addPart("app/grant/g-user-profile"),
            n.state("admin-panel.default.g-user-profile", {
                url: "/user-profile/profile/:tab",
                templateUrl: "app/grant/g-user-profile/g-user-profile.tmpl.html",
                controller: "UserProfileController"
            }),
            n.state("admin-panel.default.g-user-moderators", {
                url: "/user/moderators",
                templateUrl: "app/grant/g-user-profile/g-user-profile-moderators.tmpl.html",
                controller: "UserProfileController"
            }),
             n.state("admin-panel.default.g-user-users", {
                url: "/user/allusers",
                templateUrl: "app/grant/g-user-profile/g-user-profile-users.tmpl.html",
                controller: "UserProfileController"
            }),
              n.state("admin-panel.default.g-user-page", {
                url: "/user/page/:param",
                templateUrl: "app/grant/g-user-profile/g-user-page.tmpl.html",
                controller: "UserProfileController"
            }),
              n.state("admin-panel.default.g-user-settings", {
                url: "/user/settings",
                templateUrl: "app/grant/g-user-profile/g-user-settings.tmpl.html",
                controller: "UserProfileController"
            });


        }
    ])
    .factory("studentResource", [
        '$resource', "API_CONFIG", function(res, apiConfig) {
            //  return res(apiConfig.serverPath + "api/Students/:id");
            return res(apiConfig.serverPath + "api/Students/", null, {
                update: { method: "PUT" }
            });
        }
    ])
    .service("UserProfileService", ["$timeout", function($timeout) {
            this.goTab = function(num) {
                var tabConditions = $(".md-tab")[num];

             $timeout(function(){
               $(tabConditions).trigger('click')
             })
            }
        }
    ])
    .controller("UserProfileController", [
        "$scope", "studentResource", "universityResource", "API_CONFIG", "$mdDialog", "$http", 'FileUploader', '$state', '$q', 'gProgress', '$mdToast',  "$filter",
        '$stateParams', '$window', '$timeout', "UserProfileService",
function (e, studentResource, universityResource, API_CONFIG, $mdDialog, $http, FileUploader, $state, $q, gp, $mdToast,  $filter, $stateParams, $window, $timeout, userProfileService) {


      e.contents = [];


      var dateFormat = function () {
                var token = /d{1,4}|m{1,4}|yy(?:yy)?|([HhMsTt])\1?|[LloSZ]|"[^"]*"|'[^']*'/g,
                    timezone = /\b(?:[PMCEA][SDP]T|(?:Pacific|Mountain|Central|Eastern|Atlantic) (?:Standard|Daylight|Prevailing) Time|(?:GMT|UTC)(?:[-+]\d{4})?)\b/g,
                    timezoneClip = /[^-+\dA-Z]/g,
                    pad = function (val, len) {
                        val = String(val);
                        len = len || 2;
                        while (val.length < len) val = "0" + val;
                        return val;
                    };

                // Regexes and supporting functions are cached through closure
                return function (date, mask, utc) {
                    var dF = dateFormat;

                    // You can't provide utc if you skip other args (use the "UTC:" mask prefix)
                    if (arguments.length == 1 && Object.prototype.toString.call(date) == "[object String]" && !/\d/.test(date)) {
                        mask = date;
                        date = undefined;
                    }

                    // Passing date through Date applies Date.parse, if necessary
                    date = date ? new Date(date) : new Date;
                    if (isNaN(date)) return '';

                    mask = String(dF.masks[mask] || mask || dF.masks["default"]);

                    // Allow setting the utc argument via the mask
                    if (mask.slice(0, 4) == "UTC:") {
                        mask = mask.slice(4);
                        utc = true;
                    }

                    var _ = utc ? "getUTC" : "get",
                        d = date[_ + "Date"](),
                        D = date[_ + "Day"](),
                        m = date[_ + "Month"](),
                        y = date[_ + "FullYear"](),
                        H = date[_ + "Hours"](),
                        M = date[_ + "Minutes"](),
                        s = date[_ + "Seconds"](),
                        L = date[_ + "Milliseconds"](),
                        o = utc ? 0 : date.getTimezoneOffset(),
                        flags = {
                            d:    d,
                            dd:   pad(d),
                            ddd:  dF.i18n.dayNames[D],
                            dddd: dF.i18n.dayNames[D + 7],
                            m:    m + 1,
                            mm:   pad(m + 1),
                            mmm:  dF.i18n.monthNames[m],
                            mmmm: dF.i18n.monthNames[m + 12],
                            yy:   String(y).slice(2),
                            yyyy: y,
                            h:    H % 12 || 12,
                            hh:   pad(H % 12 || 12),
                            H:    H,
                            HH:   pad(H),
                            M:    M,
                            MM:   pad(M),
                            s:    s,
                            ss:   pad(s),
                            l:    pad(L, 3),
                            L:    pad(L > 99 ? Math.round(L / 10) : L),
                            t:    H < 12 ? "a"  : "p",
                            tt:   H < 12 ? "am" : "pm",
                            T:    H < 12 ? "A"  : "P",
                            TT:   H < 12 ? "AM" : "PM",
                            Z:    utc ? "UTC" : (String(date).match(timezone) || [""]).pop().replace(timezoneClip, ""),
                            o:    (o > 0 ? "-" : "+") + pad(Math.floor(Math.abs(o) / 60) * 100 + Math.abs(o) % 60, 4),
                            S:    ["th", "st", "nd", "rd"][d % 10 > 3 ? 0 : (d % 100 - d % 10 != 10) * d % 10]
                        };

                    return mask.replace(token, function ($0) {
                        return $0 in flags ? flags[$0] : $0.slice(1, $0.length - 1);
                    });
                };
            }();

            // Some common format strings
            dateFormat.masks = {
                "default":      "ddd mmm dd yyyy HH:MM:ss",
                shortDate:      "m/d/yy",
                mediumDate:     "mmm d, yyyy",
                longDate:       "mmmm d, yyyy",
                fullDate:       "dddd, mmmm d, yyyy",
                shortTime:      "h:MM TT",
                mediumTime:     "h:MM:ss TT",
                longTime:       "h:MM:ss TT Z",
                isoDate:        "yyyy-mm-dd",
                isoTime:        "HH:MM:ss",
                isoDateTime:    "yyyy-mm-dd'T'HH:MM:ss",
                isoUtcDateTime: "UTC:yyyy-mm-dd'T'HH:MM:ss'Z'"
            };

            // Internationalization strings
            dateFormat.i18n = {
                dayNames: [
                    "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat",
                    "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
                ],
                monthNames: [
                    "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec",
                    "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"
                ]
            };

            // For convenience...
            Date.prototype.format = function (mask, utc) {
                return dateFormat(this, mask, utc);
            };


            var self = this,
                currentStudentId = API_CONFIG.currStudentId;
            e.user = e.user || {};
            //if (currentStudentId != null && currentStudentId !== 0) {

            self.states             = null;
           // self.selectedItem       = null;
            self.searchText         = '';
            self.querySearch = querySearch;
            self.selectedItemChange = selectedItemChange;
            self.simulateQuery      = false;
            self.isDisabled         = false;
            e.validateUniver = false;


            e.grantPeople = [];
            e.moderatorStat = [];


            e.getValidStateName = function (level) {

                switch(level){

                    case 0: return 'Не проверены';
                    case 1: return 'Данные верны';
                    case 2: return 'Есть замечания';
                    case 3: return 'Не заполнены';
                    default: return '';
                }

            }

            e.scrollToTop = function(){

               $('#admin-panel-content-view').parent().parent().animate({ scrollTop: 0 }, "slow");
            }

            e.loadIsParticipant = function (id) {
               // gProgress.mask(e);

                $http.get(API_CONFIG.serverPath +'api/grantstudent/isparticipant/' + id)
                .success(function (data, status) {
                    // gProgress.unmask(e);

                     e.IsParticipant = data;

                       if(e.IsParticipant){
                                      e.MyGrantStatus = 'Статус: Участвую в конкурсе';

                                             if(e.user.StudentBookState == 1){   //  && e.user.PassportState == 1
                                                 e.MyGrantStatus += " | Ваши данные верны, ожидайте результаты конкурса"
                                              }
                                              else if(e.user.StudentBookState == 2){   //  || e.user.PassportState == 2
                                                 e.MyGrantStatus += " | Есть замечания к данным зачетной книжки"
                                              } else if(e.user.StudentBookState == 0){   //  || e.user.PassportState == 0
                                                 e.MyGrantStatus += " | Данные зачетки не проверены, ожидайте проверки модератором";
                                              }

                                     } else {
                                       e.MyGrantStatus = 'Статус: Нет заявки на участие. Прием заявок окончен';
                                     }
                  }).error(function (error) {

                 /*   $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top')); */


                  });

            };




            e.loadCurrent = function() {
                studentResource.get({ id: 0 },

                    function(resp) {
                        angular.extend(e.user, resp);

                        if(Chatra && e.user ){

                           Chatra('setIntegrationData', {
                            'ID': e.user.UserId,
                            'Email': e.user.Email,
                            'LastName': e.user.LastName,
                            'Name' : e.user.Name
                           });
                        }

                        e.user.University = resp.University;

                        e.user.paspValidStateName = e.getValidStateName(e.user.PassportState);
                        e.user.bookValidStateName = e.getValidStateName(e.user.StudentBookState);
                        e.user.IncomeValidStateName = e.getValidStateName(e.user.IncomeState);

                        e.user.EditDateText =  dateFormat(e.user.ProfileEditDate, 'dd.mm.yyyy HH:MM:ss', true);
                        e.user.PersonalDataEditDate = dateFormat(e.user.PersonalDataEditDate, 'dd.mm.yyyy HH:MM:ss', true);
                        e.user.RecordBookEditDate = dateFormat(e.user.RecordBookEditDate, 'dd.mm.yyyy HH:MM:ss', true);
                        e.user.IncomeEditDate = dateFormat(e.user.IncomeEditDate, 'dd.mm.yyyy HH:MM:ss', true);

                        if(e.user.RecordBookEditDate == '01.01.1 00:00:00')
                        {
                            e.user.RecordBookEditDate = '';
                        }

                        if(e.user.IncomeEditDate == '01.01.1 00:00:00')
                        {
                            e.user.IncomeEditDate = '';
                        }

                        if(e.user.IncomeEditDate.indexOf('31.12.0')>=0)
                        {
                            e.user.IncomeEditDate = '';
                        }

                        if(e.user.PersonalDataEditDate == '01.01.1 00:00:00')
                        {
                            e.user.PersonalDataEditDate = '';
                        }



            
                        if(e.user.University) {
                                self.selectedItem = {
                                    value: resp.University.Id,
                                    display: resp.University.Name
                                };
                        }

                        e.loadRole();

                    });
                //}
            }



            e.loadCurrent();

            e.updateUserProfileClick = function() {
               // e.user.University = {
                 //   Id: e.ctrl.selectedItem.value
                //};
                var user = new studentResource(e.user);
                if(!user.Id && !currentStudentId)
                {
                    user.Id = currentStudentId;
                    if (user.University){
                        user.UniversityId = user.University.Id;
                    }
                    gp.mask(e);
                    user.$save(function(resp) {

                        API_CONFIG.currStudentId = resp.Id;
                        angular.extend(e.user, resp);
                        gp.unmask(e);

                       e.user.paspValidStateName = e.getValidStateName(e.user.PassportState);
                       e.user.bookValidStateName = e.getValidStateName(e.user.StudentBookState);
                       e.user.IncomeValidStateName = e.getValidStateName(e.user.IncomeState);

                    e.profileEdit = false;

                    $mdToast.show(
                        $mdToast.simple()
                        .content('Информация успешно сохранена')
                        .position('top'));

                    e.loadIsProfileFilled();

                    e.loadCurrent();

                    if(window.loadUserInfoBar){
                        loadUserInfoBar();
                    }

                    }, function (error) {
                    $mdToast.show(
                        $mdToast.simple()
                        .content('Ошибка сохранения профиля')
                        .theme("error-toast")
                        .position('top'));
                        gp.unmask(e);

                });
            }
                else
                {
                    gp.mask(e);
                     //user.UniversityId = user.University.Id;
                    user.$update(function(resp) {
                        API_CONFIG.currStudentId = resp.Id;
                        gp.unmask(e);
                        angular.extend(e.user, resp);

                      e.user.paspValidStateName = e.getValidStateName(e.user.PassportState);
                      e.user.bookValidStateName = e.getValidStateName(e.user.StudentBookState);
                      e.user.IncomeValidStateName = e.getValidStateName(e.user.IncomeState);
                      

                      e.loadCurrent();

                    if(window.loadUserInfoBar){
                        loadUserInfoBar();
                    }

                    e.profileEdit = false;

                          $mdToast.show(
                        $mdToast.simple()
                        .content('Информация успешно сохранена')
                        .position('top'));
                    e.loadIsProfileFilled();

                    });
                }
            };


            e.askUpdateUserProfileClick = function(){
              if(e.user.PassportState == 1)
                {
                    $mdDialog.show(
                        $mdDialog.confirm()
                        .title('Изменение персональных данных')
                        .content("Ваши данные проверены, при изменении ФИО потребуется новая проверка! Продолжить?")
                        .ok("Да")
                        .cancel("Отмена")).then(function(result){

                          if(result){
                             e.updateUserProfileClick();
                          }
                    });
                } else{

                    e.updateUserProfileClick();

                }
            }

            function querySearch(value) {

                e.validateUniver=true;

                if(value == "" || value == null){
                    value = 'null';
                }

                if (value.length > 0) {
                    var promise = $http.get(API_CONFIG.serverPath + 'api/Universities/'+ value ),
                        deferred = $q.defer();

                    promise.then(
                        function(resp) {
                            var result = new Array(),
                                data = resp.data;

                                for (var i = 0; i < data.length; i++) {
                                result.push({
                                    value: data[i].Id,
                                    display: data[i].Name
                                });

                                    if(result.length == 0){
                                         result.push({
                                            value: 1,
                                            display: "Неверно введено наименование. Вводите без кавычек и сокращений"
                                        });
                                }
                            }
                            deferred.resolve(result);
                        },
                        function(reason) {

                        });
                    return deferred.promise;
                };
            };

            function selectedItemChange(item) {

                if(item && item.value){
                    e.user.UniversityId = item.value;

                    if(e.user.University){
                        e.user.University.Name = item.display;
                        e.user.University.Id = item.value;
                    }
                    else{
                        e.user.University = {
                            Name:  item.display,
                            Id: item.value
                        };
                    }

                }
            };

            if($stateParams.tab){
                 e.profileEdit = true;
             }


    // uploaders
            e.avatarClick = function () {

                $("#avatarFileInput").click();

            };

            var uploader = e.uploader = new FileUploader({
                url: API_CONFIG.serverPath + 'api/file'
            });

    // FILTERS

            uploader.filters.push({
                name: 'imageFilter',
                fn: function (item /*{File|FileLikeObject}*/, options) {
                    var type = '|' + item.type.slice(item.type.lastIndexOf('/') + 1) + '|';
                    return '|jpg|png|jpeg|bmp|gif|'.indexOf(type) !== -1;
                }
            });

    // CALLBACKS

            uploader.onWhenAddingFileFailed = function (item /*{File|FileLikeObject}*/, filter, options) {
                console.info('onWhenAddingFileFailed', item, filter, options);
            };

            uploader.onAfterAddingFile = function (fileItem) {
                var t = this;

                if (t.queue.length > 1) {
                    t.removeFromQueue(t.queue[0]);
                }

                fileItem.upload();
            };

            uploader.onCompleteItem = function (fileItem, response, status, headers) {

                if (status == 200) {
                    e.user.ImageFile = response;
                }
                else {
                    $mdToast.show(
                    $mdToast.simple()
                    .content(response)
                    .theme("error-toast")
                    .position('top'));
                }
                console.info('onCompleteItem', fileItem, response, status, headers);
            };






            // страница студента (произвольного)



            e.loadPersonalInfoModerator = function(id)
            {

                 $http.get(API_CONFIG.serverPath +'api/students/getvalidatorname/'+ id + '/1')
                    .success(function (data, status) {
                         gp.unmask(e);

                        if(data && data.Data)
                        {
                            e.personalModeratorName = data.Data;
                        }


                  });

                    /*.error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  }); */


            }




           e.userClass = function(isValid) {
                return isValid ? 'user-left' : 'user-right';
            };

            e.loadValidationhistory = function(id)
            {

                 $http.get(API_CONFIG.serverPath +'api/students/getvalidationhistory/'+ id + '/2')
                    .success(function (data, status) {
                         gp.unmask(e);

                        if(data && data.Data)
                        {
                            e.validationHistory = data.Data;
                        }


                     });

            }


            e.go = function(id){

                  $state.go('admin-panel.default.g-user-page', {'param': id});
            }





            e.loadRecorBookInfoModerator = function(id)
            {

                 $http.get(API_CONFIG.serverPath +'api/students/getvalidatorname/'+ id + '/2')
                    .success(function (data, status) {
                         gp.unmask(e);

                        if(data && data.Data)
                        {
                            e.recordBookModeratorName = data.Data;
                        }


                  });

                    /*.error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  }); */


            }


            e.loadStudentInfo = function(id)
            {
                $http.get(API_CONFIG.serverPath +'api/students/getinfo/'+ id)
                    .success(function (data, status) {
                         gp.unmask(e);

                        e.userInfo = data;

                        e.userInfo.EditDateText =  dateFormat(e.userInfo.ProfileEditDate, 'dd.mm.yyyy HH:MM:ss', true);
                        e.userInfo.PersonalDataEditDate = dateFormat(e.userInfo.PersonalDataEditDate, 'dd.mm.yyyy HH:MM:ss', true);
                        e.userInfo.RecordBookEditDate = dateFormat(e.userInfo.RecordBookEditDate, 'dd.mm.yyyy HH:MM:ss', true);
                        e.userInfo.IncomeEditDate = dateFormat(e.user.IncomeEditDate, 'dd.mm.yyyy HH:MM:ss', true);

                        if(e.userInfo.IncomeEditDate == '01.01.1 00:00:00')
                        {
                            e.userInfo.IncomeEditDate = '';
                        }

                        e.userInfo.paspValidStateName = e.getValidStateName(e.userInfo.PassportState);
                        e.userInfo.bookValidStateName = e.getValidStateName(e.userInfo.StudentBookState);
                        e.userInfo.IncomeValidStateName = e.getValidStateName(e.userInfo.IncomeState);


                        e.loadRole();
                        e.loadPersonalInfoModerator(id);
                        e.loadRecorBookInfoModerator(id);
                        e.loadValidationhistory(id);

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
            }


            e.setpaspvalid = function() {

                gp.mask(e);

               $http.get(API_CONFIG.serverPath +'api/students/setpaspvalid/' + studentId)
                .success(function (data, status) {
                     gp.unmask(e);

                     e.userInfo.paspValidStateName = e.getValidStateName(1);
                     e.userInfo.PassValidationComment = null;

                       $mdToast.show(
                        $mdToast.simple()
                        .content('Корректность данных подтверждена')
                        .position('top'));

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
            }

            e.setbookvalid = function() {

                gp.mask(e);

               $http.get(API_CONFIG.serverPath +'api/students/setbookvalid/' + studentId)
                .success(function (data, status) {
                     gp.unmask(e);

                     e.userInfo.bookValidStateName = e.getValidStateName(1);
                     e.userInfo.BookValidationComment = null;

                       $mdToast.show(
                        $mdToast.simple()
                        .content('Корректность данных подтверждена')
                        .position('top'));

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
            }

            e.setIncomeValid = function() {

               gp.mask(e);

               $http.get(API_CONFIG.serverPath +'api/students/setincomevalid/' + studentId)
                  .success(function (data, status) {
                     gp.unmask(e);

                     e.userInfo.IncomeValidStateName = e.getValidStateName(1);
                     e.userInfo.IncomeValidationComment = null;

                       $mdToast.show(
                        $mdToast.simple()
                        .content('Корректность данных подтверждена')
                        .position('top'));

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
            }

            e.setpaspinvalid = function(force) {

                 if(force != true){
                    force = false;
                }

                gp.mask(e);

                var msg = {};
                msg.Message = e.userInfo.PassValidationComment;


               $http.put(API_CONFIG.serverPath +'api/students/setpaspinvalid/' + studentId + '/' + force , msg)
                .success(function (data, status) {
                     gp.unmask(e);

                     e.userInfo.paspValidStateName = e.getValidStateName(2);

                      $mdToast.show(
                        $mdToast.simple()
                        .content('Замечания к заполнению данных сохранены')
                        .position('top'));

                  }).error(function (error) {


                   if(error.Message == "Студент выбран победителем гранта. Замечание к его паспортным данным отменит этот выбор" ||
                     error.Message == "Студент выбран дополнительным победителем гранта. Замечание к его паспортным данным отменит этот выбор"){

                         $mdDialog.show(
                            $mdDialog.confirm()
                            .title('Отмена победителя')
                            .content(error.Message)
                            .ok("Да")
                            .cancel("Отмена")).then(function(result){

                              if(result){
                                 e.setpaspinvalid(true)
                              }
                        });



                    } else {
                           $mdToast.show(
                                $mdToast.simple()
                                .content(error.data.Message)
                                .theme("error-toast")
                                .position('top'));
                            }
                              });
            }

            e.setIncomeInvalid = function(force) {

              if(force != true){
                   force = false;
               }

               gp.mask(e);

               var msg = {};
               msg.Message = e.userInfo.IncomeValidationComment;


              $http.put(API_CONFIG.serverPath +'api/students/setincomeinvalid/' + studentId + '/' + force , msg)
               .success(function (data, status) {
                    gp.unmask(e);

                    e.userInfo.IncomeValidStateName = e.getValidStateName(2);

                     $mdToast.show(
                       $mdToast.simple()
                       .content('Замечания к заполнению данных сохранены')
                       .position('top'));

                 }).error(function (error) {


                  if(error.Message && error.Message.indexOf('Студент выбран победителем гранта')>0){

                        $mdDialog.show(
                           $mdDialog.confirm()
                           .title('Отмена победителя')
                           .content(error.Message)
                           .ok("Да")
                           .cancel("Отмена")).then(function(result){

                             if(result){
                                e.setIncomeInvalid(true)
                             }
                       });



                   } else {
                          $mdToast.show(
                               $mdToast.simple()
                               .content(error.data.Message)
                               .theme("error-toast")
                               .position('top'));
                           }
                             });
           }


            e.setbookinvalid = function(force) {


                if(force != true){
                    force = false;
                }

                gp.mask(e);

                var msg = {};
                msg.Message = e.userInfo.BookValidationComment;

               $http.put(API_CONFIG.serverPath +'api/students/setbookinvalid/' + studentId+ '/' + force, msg)
                .success(function (data, status) {
                     gp.unmask(e);

                     e.userInfo.bookValidStateName = e.getValidStateName(2);

                      $mdToast.show(
                        $mdToast.simple()
                        .content('Замечания к заполнению данных сохранены')
                        .position('top'));

                  }).error(function (error) {


                    if(error.Message == "Студент выбран победителем гранта. Замечание к его зачетной книжке отменит этот выбор" ||
                     error.Message == "Студент выбран дополнительным победителем гранта. Замечание к его зачетной книжке отменит этот выбор"){

                         $mdDialog.show(
                            $mdDialog.confirm()
                            .title('Отмена победителя')
                            .content(error.Message)
                            .ok("Да")
                            .cancel("Отмена")).then(function(result){

                              if(result){
                                 e.setbookinvalid(true)
                              }
                        });



                    } else {
                           $mdToast.show(
                                $mdToast.simple()
                                .content(error.data.Message)
                                .theme("error-toast")
                                .position('top'));
                            }
                              });
            }

            // достижения

            //todo переделать
            var getLevel = function (level) {

                switch(level){

                    case 0: return 'Международный';
                    case 1: return 'Россия';
                    case 2: return 'Республиканский';
                    case 3: return 'Городской';
                    case 4: return 'ВУЗовский';
                    default: return '';

                }

            }

            //todo переделать
            var getState = function (level) {

                switch(level){

                    case 0: return 'Победитель';
                    case 1: return 'Участник';
                    case 2: return 'Организатор';
                    default: return '';
                }

            }

            //todo переделать
            var getSubject = function (level) {

                switch(level){

                    case 0: return 'Наука';
                    case 1: return 'Спорт';
                    case 2: return 'Творчество';
                    case 3: return 'Общественная деятельность';
                    default: return '';

                }

            }

            e.editAchievementClick = function (item) {

                  window.setAchievement(item);
                  $("#achievementform").show();
                  $(".achievementtoolbar").hide();
                  $("#admin-panel-content-view").parent().parent().animate({ scrollTop: 400 }, "slow");
  
            }


           window.loadAchievements =  function(){

               e.loadAchievements(window.studentId);
           }



           e.loadAchievements = function (id) {

              $http.get(API_CONFIG.serverPath + 'api/achievement/getbystudent/' + id)
                .then(function(resp){

                    var year = new Date().getFullYear();

                      var achArr = [];
                      var oldArr = [];

                    resp.data.forEach(function(t){

                        var ach = {
                            CreateDate: t.CreateDate,
                            EditDate: t.EditDate,
                            Id: t.Id,
                            Level: t.Level,
                            LevelName : getLevel(t.Level),
                            Name: t.Name,
                            Files: t.Files,
                            Criterion : t.Criterion,
                            StateName: getState(t.State),
                            State: t.State,
                            ValidationState : t.ValidationState,
                            ValidStateName : getValidStateName(t.ValidationState),
                            Score : t.Score,
                            ValidationComment: t.ValidationComment,
                            StudentId: t.StudentId,
                            SubjectName: getSubject(t.Subject),
                            Subject: t.Subject,
                            Year: t.Year,
                            FilesList: t.FilesList,
                            ImageLink: t.ImageLink
                        };

                        if(ach.Year >= year - 1){
                            achArr.push(ach);
                        } else{
                            oldArr.push(ach);
                        }

                    });

                    e.otherUserAchievements = achArr;
                    e.otherUserOldAchievements = oldArr;

                  

                    setTimeout(e.showBadges, 1000);


                 });

            }

            e.goeditProfile = function() {

                 $state.go('admin-panel.default.g-user-profile');

            }

             e.goProfile = function() {

                 $state.go('admin-panel.default.g-user-profile');

            }

            e.showHiddenBadges = function(){

                var badges = $(".badge");
                badges.removeClass('hiddenbadge');
                badges.show();
            }


            e.showBadges = function() {

                var badges = $(".special");
                var divs = $(".crop-div");
                var imgs = $(".crop-div img");

                badges.each(function( index, value ) {

                    var badge = $(badges[index]);
                    var img = $(imgs[index]);
                    var div = $(divs[index]);

                    var t = img.position().top;
                    var l = img.position().left;
                    var w=  Math.min(img.width(), div.width());
                    var h = Math.min(img.height(), div.height());

                    badge.css({top: t + h-25, left: l + w- 60 - 375});
                });

                badges = $(".special2");

                 badges.each(function( index, value ) {

                    var badge = $(badges[index]);
                    var img = $(imgs[index]);
                    var div = $(divs[index]);

                    var t = img.position().top;
                    var l = img.position().left;
                    var w=  Math.min(img.width(), div.width());
                    var h = Math.min(img.height(), div.height());

                    badge.css({top: t + h-25 , left: l + w- 60 - 300});
                });

                badges = $(".special3");

                 badges.each(function( index, value ) {

                    var badge = $(badges[index]);
                    var img = $(imgs[index]);
                    var div = $(divs[index]);

                    var t = img.position().top;
                    var l = img.position().left;
                    var w=  Math.min(img.width(), div.width());
                    var h = Math.min(img.height(), div.height());

                    badge.css({top: t + h-25 , left: l + w- 60 - 225});
                });

                badges = $(".special4");

                 badges.each(function( index, value ) {

                    var badge = $(badges[index]);
                    var img = $(imgs[index]);
                    var div = $(divs[index]);

                    var t = img.position().top;
                    var l = img.position().left;
                    var w=  Math.min(img.width(), div.width());
                    var h = Math.min(img.height(), div.height());

                    badge.css({top: t + h-25 , left: l + w- 60 - 150});
                });

                badges = $(".special5");

                badges.each(function( index, value ) {

                   var badge = $(badges[index]);
                   var img = $(imgs[index]);
                   var div = $(divs[index]);

                   var t = img.position().top;
                   var l = img.position().left;
                   var w=  Math.min(img.width(), div.width());
                   var h = Math.min(img.height(), div.height());

                   badge.css({top: t + h-25 , left: l + w- 60 - 75});
               });

               badges = $(".special6");

               badges.each(function( index, value ) {

                  var badge = $(badges[index]);
                  var img = $(imgs[index]);
                  var div = $(divs[index]);

                  var t = img.position().top;
                  var l = img.position().left;
                  var w=  Math.min(img.width(), div.width());
                  var h = Math.min(img.height(), div.height());

                  badge.css({top: t + h-25 , left: l + w- 60});
              });


               setTimeout(e.showHiddenBadges, 500);
            }



            $( window ).resize(function() {
               e.showBadges();
            });


            e.$on('ngRepeatFinished', function(ngRepeatFinishedEvent) {
                 e.showBadges();
             });


            e.openImage = function (image, $event) {
                e.currentImage = image;
                $mdDialog.show({
                    controller: 'ScanDialogController',
                    templateUrl: 'app/grant/g-user-profile/g-user-scan-dialog.tmpl.html',
                    clickOutsideToClose: true,
                    focusOnOpen: false,
                    targetEvent: $event,
                    locals: {
                        image: image
                    }
                });
            };

            e.otherUserPinfo = e.otherUserPinfo || {};
            e.loadPersonalInfo = function (id, versionId) {

              $http.get(API_CONFIG.serverPath + 'api/personalInfo/getbystudent/' + id + '/' + versionId)
                .then(function(resp){

                    //
                    if(resp.data.Birthday) {
                        resp.data.BirthdayText = dateFormat(resp.data.Birthday, 'dd.mm.yyyy', true);
                        resp.data.Birthday = new Date(resp.data.Birthday);

                    }

                    if(resp.data.PassportIssueDate) {
                         resp.data.PassportIssueDateText = dateFormat(resp.data.PassportIssueDate, 'dd.mm.yyyy', true);
                         resp.data.PassportIssueDate = new Date(resp.data.PassportIssueDate);

                    }

                      //resp.data.EditDateText =  dateFormat(resp.data.EditDate, 'dd.mm.yyyy HH:MM:ss', true);

                    if(resp.data.Sex == 1 ){
                        resp.data.SexName = "Мужской"
                    } else {
                         resp.data.SexName = "Женский"
                    }



                      angular.extend(e.otherUserPinfo, resp.data);

                 });

            }

            e.loadinfoVersion = function(){

                if(e.selectedInfoversion)
                {
                    e.loadPersonalInfo(e.studentId , e.selectedInfoversion);
                }
            }



           e.loadStudentBook = function (id) {

              $http.get(API_CONFIG.serverPath + 'api/recordBook/getbystudent/' + id)
                .then(function(resp){

                    if(e.otherUserRecordbook){
                         e.otherUserRecordbook.Files = resp.data;
                    }
                    else{
                        e.otherUserRecordbook = {};
                        e.otherUserRecordbook.Files;
                    }
               });

            }

            e.loadIncomeFiles = function (id) {

                $http.get(API_CONFIG.serverPath + 'api/income/getbystudent/' + id)
                  .then(function(resp){
  
                      if(e.otherUserIncome)
                      {
                           e.otherUserIncome.Files = resp.data;
                      }
                      else{
                          e.otherUserIncome = {};
                          e.otherUserIncome.Files;
                      }
                 });
  
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


                     if(e.userInfo && e.userInfo.University && e.roleInfo.UniversCurator.indexOf(e.userInfo.University.Id) >= 0){
                        e.isCurator = true;
                     }


                     if( (e.isAdmin || e.isModerator) && e.defferredLoadPersonalInfo)
                     {
                        e.defferredLoadPersonalInfo();
                     }

                    e.checkCurPageAccess();

                    e.loadIsParticipant(28);


                    if(e.isAdmin){
                        e.myRoleStatus = "Администратор";
                    } else if (e.isModerator){
                        e.myRoleStatus = "Модератор";
                    } else if (e.isCurator){
                        e.myRoleStatus = "Куратор";
                    }

               });

            }

             e.checkCurPageAccess = function(){


                 var stateName = $state.current.name;

              if(e.isAdmin || e.isModerator || e.isCurator || window.studentId == 0)
              {

                if(stateName == 'admin-panel.default.g-user-moderators' && e.isModerator){

                } else{
                    return; // access
                }


              }

              if(stateName == 'admin-panel.default.g-user-profile'){

                return;

              }

               $mdToast.show(
                  $mdToast.simple()
                  .content('У Вас нет доступа к данному разделу')
                  .theme("error-toast")
                  .position('top'));

               $state.go('admin-panel.default.g-user-profile');
            }


            e.moderatorStatColumns = [
             {
                    title: "Фамилия Имя Отчетство",
                    field: "Fio",
                    sortable: true
                },{
                    title: "Проверено студентов",
                    field: "ValidationCount",
                    sortable: true
                }
            ];


             e.peopleColumns = [
                {
                    title: "Id",
                    field: "id",
                    sortable: true
                },
                {
                    title: "Фамилия Имя Отчетство",
                    field: "Fio",
                    sortable: true
                },{
                    title: "Балл",
                    field: "Score",
                    sortable: true
                },{
                    title: "Образовательное учреждение",
                    field: "UniversityName",
                    sortable: true
                },{
                    sortable: true,
                    filter: "tableImage",
                    title: "Перс. данные",
                    field: "PassportStateLink"
                },
                 {
                    title: "Зачетка",
                    field: "StudentBookStateLink",
                    sortable: true,
                    filter: "tableImage"

                },
                {
                    title: "Справка о доходах",
                    field: "IncomeStateLink",
                    sortable: true,
                    filter: "tableImage"

                },
                 {
                    title: "Телефон",
                    field: "Phone",
                    sortable: true
                }, {
                    title: "Email",
                    field: "Email",
                    sortable: true
                },
                {
                    title: "Дата изменения",
                    field: "EditDate",
                    sortable: true
                },
                {
                    title: "Гражданство",
                    field: "CitizenshipName",
                    sortable: false
                },
                 {
                    field: "link"
                }
            ];



            e.getCitizenshipName = function(val) {

                 if(val==1)
                    {
                       return 'Иное';
                    } else if(val == 0) {
                        return 'РФ';
                    } else {
                        return '';
                    }
            }

             e.loadUsers = function (text) {

                gp.mask(e);

                if(text){} else{

                    if(e.searchUser && e.searchUser.name){
                       var  text = e.searchUser.name;
                    }

                }

            /*    if(e.selectedGrant){
                      $window.sessionStorage.setItem('selectedGrant', e.selectedGrant);
                  } else {
                     $window.sessionStorage.setItem('selectedGrant', null);
                  } */

               var sortBy = 0;
               var asc = false;
               var cols = $(".elements-image-table-example thead tr th");

                    for(var i=0; i < cols.length; i++){

                      var col = $(cols[i]).find('md-icon');

                       if( $(col[0]).attr('aria-hidden') == 'false') {
                          sortBy = i;
                          asc = true;

                        }


                       if( $(col[1]).attr('aria-hidden') == 'false') {
                           sortBy = i;
                          asc = false;

                        }
                    }

                var searchStudent = {
                    Name : text,
                    GrantId: e.selectedGrant ? e.selectedGrant : null,
                    Citizenship : e.selectedCitizenship ? e.selectedCitizenship : null,
                    PersonalData : e.selectedPassport ? e.selectedPassport : null,
                    RecordBook : e.selectedRecordbook ? e.selectedRecordbook : null,
                    Income: e.selectedIncome,
                    UniversityId : e.selectedUniver ? e.selectedUniver : null,
                    IsWinner : e.selectedWinnerState ? e.selectedWinnerState : null,
                    IsPassportOutDate : e.selectedPassportOutOfDate ? e.selectedPassportOutOfDate : null,
                    lastId : 0,
                    sortBy : sortBy,
                    skip : 0,
                    Asc : asc
                };

                if(e.UsersQuery){
                    e.UsersQuery += 1;
                }else{
                    e.UsersQuery = 1;
                }

                var localQueryNum = e.UsersQuery ;

               $http.put(API_CONFIG.serverPath +'api/students/getbyname/', searchStudent)
                .success(function (data, status) {
                     gp.unmask(e);

                     if(localQueryNum < e.UsersQuery ){
                        return;
                     }

                     var result = [];
                     for (var i = 0; i < data.length; i++) {
                        result.push({
                            id: data[i].Id,
                            Fio: (data[i].Name ? data[i].Name : '') + ' ' + (data[i].LastName ? data[i].LastName : '')  +' '+ (data[i].Patronymic ? data[i].Patronymic : ''),
                            IsPassportValid : data[i].IsPassportValid ? '+' : '-' ,
                            IsStudentBookValid: data[i].IsStudentBookValid ? '+' : '-'  ,
                            UniversityName: data[i].UniversityName,
                            University: data[i].University,
                            link : '<a href="/my#/user/page/' + data[i].Id +'" target="_blank"> Просмотр  </a>',
                            PassportState: data[i].PassportState,
                            StudentBookState : data[i].StudentBookState,
                            IncomeState : data[i].IncomeState,
                            PassportStateLink :  '../ach/validstate' +  data[i].PassportState + '.png',
                            StudentBookStateLink : '../ach/validstate' +  data[i].StudentBookState + '.png',
                            IncomeStateLink : '../ach/validstate' +  data[i].IncomeState + '.png',
                            Score: data[i].Score,
                            Phone: data[i].Phone,
                            Email: data[i].Email,
                           EditDate : dateFormat(data[i].EditDate, 'dd.mm.yyyy HH:MM:ss', true),
                            Citizenship : data[i].Citizenship,
                            CitizenshipName : e.getCitizenshipName(data[i].Citizenship)
                        });
                    }

                    e.grantPeople = result;
                   // e.notFilteredPeopleList = result;

                    // if(e.selectedUniver || e.selectedRecordbook || e.selectedPassport){
                      //  e.applyUserFilter();
                    //}

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
            };


            e.loadMore = function() {

              if(e.isLoadingMoreEvents){
                return;
              }

              if(e.noMoreEvents){
                return;
              }

              e.isLoadingMoreEvents = true;


               gp.mask(e);


               var lastId = 0;
               var skip = 0;

               if(e.grantPeople && e.grantPeople.length > 0){

                 lastId = e.grantPeople[e.grantPeople.length-1].id;

                 skip = e.grantPeople.length;
               }

               var text = null;

                if(text){} else{

                    if(e.searchUser && e.searchUser.name){
                       var  text = e.searchUser.name;
                    }

                }

                var sortBy = 0;
                var asc = false;


                var cols = $(".elements-image-table-example thead tr th");

                    for(var i=0; i < cols.length; i++){

                      var col = $(cols[i]).find('md-icon');

                       if( $(col[0]).attr('aria-hidden') == 'false') {
                          sortBy = i;
                          asc = true;

                        }


                       if( $(col[1]).attr('aria-hidden') == 'false') {
                           sortBy = i;
                          asc = false;

                        }


                    }


                var searchStudent = {
                    Name : text,
                    GrantId: e.selectedGrant ? e.selectedGrant : null,
                    Citizenship : e.selectedCitizenship ? e.selectedCitizenship : null,
                    PersonalData : e.selectedPassport ? e.selectedPassport : null,
                    RecordBook : e.selectedRecordbook ? e.selectedRecordbook : null,
                    Income: e.selectedIncome ? e.selectedIncome : null,
                    UniversityId : e.selectedUniver ? e.selectedUniver : null,
                    IsWinner : e.selectedWinnerState ? e.selectedWinnerState : null,
                    IsPassportOutDate : e.selectedPassportOutOfDate ? e.selectedPassportOutOfDate : null,
                    lastId  : lastId,
                    sortBy : sortBy,
                    skip : skip,
                    Asc : asc
                };


                 $http.put(API_CONFIG.serverPath +'api/students/getbyname/', searchStudent)
                .success(function (data, status) {
                     gp.unmask(e);

                    var result = [];

                     for (var i = 0; i < data.length; i++) {

                        e.grantPeople.push({
                            id: data[i].Id,
                            Fio: (data[i].Name ? data[i].Name : '') + ' ' + (data[i].LastName ? data[i].LastName : '')  +' '+ (data[i].Patronymic ? data[i].Patronymic : ''),
                            IsPassportValid : data[i].IsPassportValid ? '+' : '-' ,
                            IsStudentBookValid: data[i].IsStudentBookValid ? '+' : '-'  ,
                            UniversityName: data[i].UniversityName,
                            University: data[i].University,
                            link : '<a href="/my#/user/page/' + data[i].Id +'" target="_blank"> Просмотр  </a>',
                            PassportState: data[i].PassportState,
                            StudentBookState : data[i].StudentBookState,
                            PassportStateLink :  '../ach/validstate' +  data[i].PassportState + '.png',
                            StudentBookStateLink : '../ach/validstate' +  data[i].StudentBookState + '.png',
                            IncomeStateLink : '../ach/validstate' +  data[i].IncomeState + '.png',
                            Score: data[i].Score,
                            Phone: data[i].Phone,
                            Email: data[i].Email,
                            EditDate : dateFormat(data[i].EditDate, 'dd.mm.yyyy HH:MM:ss', true),
                            Citizenship : data[i].Citizenship,
                            CitizenshipName : e.getCitizenshipName(data[i].Citizenship)
                        });
                    }

                    if(data.length == 0){
                        e.noMoreEvents = true;
                    }

                   gp.unmask(e);
                    e.isLoadingMoreEvents = false;

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));

                     e.isLoadingMoreEvents = false;
                  });




            };

              e.eventStartDateChanged = function(a,b,c) {


                if(e.eventStartDate) {


                    if(e.eventStartDate.length<4){

                         var index = e.eventStartDate.indexOf('.');


                        if(e.eventStartDate.length == 2 && index==1){

                            e.eventStartDate = e.eventStartDate.substr(0,1);

                        } else if(e.eventStartDate.length == 2 && index<0){

                            e.eventStartDate += '.';

                        } else if(e.eventStartDate.length > 2 && index<0){

                            e.eventStartDate = e.eventStartDate.substr(0,2) + '.';

                        } else if(e.eventStartDate.length > 2 && index!=2){

                            var b = e.eventStartDate.replace('.','');
                            if(b.length==2) {
                                e.eventStartDate = b + '.';
                            } else {
                                e.eventStartDate = b;
                            }
                        }

                          e.eventStartDateInvalid = true;
                          e.eventStartDateParsed = null;
                    }
                    else if(e.eventStartDate.length>4 && e.eventStartDate.length<7){

                        var len = e.eventStartDate.replace('.','').length;

                         var index = e.eventStartDate.replace('.','').indexOf('.');

                         if(e.eventStartDate.length == 5 && index==3){
                            e.eventStartDate = e.eventStartDate.substr(0,4);
                         }
                         else if(e.eventStartDate.length == 5 && index<0 && len == 4){
                            e.eventStartDate += '.';
                        }
                        else if(e.eventStartDate.length > 4 && index!= 4){

                            e.eventStartDate = '';
                        }

                          e.eventStartDateInvalid = true;
                          e.eventStartDateParsed = null;

                     }  else if (e.eventStartDate.length >= 7 && e.eventStartDate.length < 10){

                          e.eventStartDateInvalid = true;
                          e.eventStartDateParsed = null;


                     }  else if (e.eventStartDate.length == 10){

                        var from = e.eventStartDate.split(".");
                        var dt = new Date(from[2], from[1] - 1, from[0], 20);

                        if(dt.valueOf() > 0 || dt.valueOf() < 0) {


                            var year = dt.getFullYear();

                            if(year < 1900){
                                e.eventStartDateInvalid = true;
                                e.eventStartDateParsed = null;
                            }
                            else{
                                 e.eventStartDateInvalid = false;
                                  e.eventStartDateParsed = dt;
                                  e.applyEventFilter();
                            }

                        } else{
                          e.eventStartDateInvalid = true;
                          e.eventStartDateParsed = null;
                        }

                     } else if (e.eventStartDate.length > 10){

                        e.eventStartDate = e.eventStartDate.substr(0,10);

                     }
                }
            }

            e.eventEndDateChanged = function(a,b,c) {


                if(e.eventEndDate) {


                    if(e.eventEndDate.length<4){

                         var index = e.eventEndDate.indexOf('.');


                        if(e.eventEndDate.length == 2 && index==1){

                            e.eventEndDate = e.eventEndDate.substr(0,1);

                        } else if(e.eventEndDate.length == 2 && index<0){

                            e.eventEndDate += '.';

                        } else if(e.eventEndDate.length > 2 && index<0){

                            e.eventEndDate = e.eventEndDate.substr(0,2) + '.';

                        } else if(e.eventEndDate.length > 2 && index!=2){

                            var b = e.eventEndDate.replace('.','');
                            if(b.length==2) {
                                e.eventEndDate = b + '.';
                            } else {
                                e.eventEndDate = b;
                            }
                        }

                          e.eventEndDateInvalid = true;
                          e.eventEndDateParsed = null;
                    }
                    else if(e.eventEndDate.length>4 && e.eventEndDate.length<7){

                        var len = e.eventEndDate.replace('.','').length;

                         var index = e.eventEndDate.replace('.','').indexOf('.');

                         if(e.eventEndDate.length == 5 && index==3){
                            e.eventEndDate = e.eventEndDate.substr(0,4);
                         }
                         else if(e.eventEndDate.length == 5 && index<0 && len == 4){
                            e.eventEndDate += '.';
                        }
                        else if(e.eventEndDate.length > 4 && index!= 4){

                            e.eventEndDate = '';
                        }

                          e.eventEndDateInvalid = true;
                          e.eventEndDateParsed = null;

                     }  else if (e.eventEndDate.length >= 7 && e.eventEndDate.length < 10){

                          e.eventEndDateInvalid = true;
                          e.eventEndDateParsed = null;


                     }  else if (e.eventEndDate.length == 10){

                        var from = e.eventEndDate.split(".");
                        var dt = new Date(from[2], from[1] - 1, from[0], 20);

                        if(dt.valueOf() > 0 || dt.valueOf() < 0) {


                            var year = dt.getFullYear();

                            if(year < 1900){
                                e.eventEndDateInvalid = true;
                                e.eventEndDateParsed = null;
                            }
                            else{
                                 e.eventEndDateInvalid = false;
                                  e.eventEndDateParsed = dt;
                                  e.applyEventFilter();
                            }

                        } else{
                          e.eventEndDateInvalid = true;
                          e.eventEndDateParsed = null;
                        }

                     } else if (e.eventEndDate.length > 10){

                        e.eventEndDate = e.eventEndDate.substr(0,10);

                     }
                }
            }


            e.applyEventFilter = function(){

                e.loadModeratorStat();
            }


            e.loadModeratorStat = function(){


             var eventFilter = {
                                After : e.eventStartDateParsed,
                                Before : e.eventEndDateParsed,
                            };


             $http.put(API_CONFIG.serverPath +'api/students/getvalidationstat', eventFilter )
                .success(function (resp, status) {
                     gp.unmask(e);

                     if(resp && resp.Data){
                        e.moderatorStat = resp.Data;
                     }

                      for (var i = 0; i < e.moderatorStat.length; i++) {
                       e.moderatorStat[i].Fio =  (e.moderatorStat[i].Name ? e.moderatorStat[i].Name : '') + ' ' +
                                                 (e.moderatorStat[i].LastName ? e.moderatorStat[i].LastName : '')  +' '+
                                                 (e.moderatorStat[i].Patronymic ? e.moderatorStat[i].Patronymic : '');

                    }

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
            }

            e.loadGrantList = function(grantId)
            {
                gp.mask(e);

               $http.get(API_CONFIG.serverPath +'api/grant/list')
                .success(function (resp, status) {
                     gp.unmask(e);

                     e.grantList = resp.Data;

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });

            }

           e.loadGrantUnivers = function(grantId)
            {

               $http.get(API_CONFIG.serverPath +'api/allUniversity/')
                .success(function (data, status) {
                     gp.unmask(e);

                     e.univerList = data;

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });

            }


                if ($window.sessionStorage.getItem('selectedUniver')) {
                        e.selectedUniver = $window.sessionStorage.getItem('selectedUniver');
                }

                if ($window.sessionStorage.getItem('selectedPassport')) {
                        e.selectedPassport = $window.sessionStorage.getItem('selectedPassport');
                }

                if ($window.sessionStorage.getItem('selectedRecordbook')) {
                        e.selectedRecordbook = $window.sessionStorage.getItem('selectedRecordbook');
                }

                if ($window.sessionStorage.getItem('selectedIncome')) {
                    e.selectedIncome = $window.sessionStorage.getItem('selectedIncome');
                }

                 if ($window.sessionStorage.getItem('selectedGrant')) {
                        e.selectedGrant = $window.sessionStorage.getItem('selectedGrant');
                }



            e.applyUserFilter = function(){

                e.filteredPeopleList =  [];

                for(var i=0; i<e.notFilteredPeopleList.length; i++)
                {

                    var toAdd = true;

                    if(e.selectedUniver){

                          $window.sessionStorage.setItem('selectedUniver', e.selectedUniver);

                        if(e.notFilteredPeopleList[i].University && e.notFilteredPeopleList[i].University.Id == e.selectedUniver){

                        } else{
                            toAdd = false;
                        }
                    } else{
                        $window.sessionStorage.setItem('selectedUniver', '');
                    }

                    if(e.selectedPassport){

                        $window.sessionStorage.setItem('selectedPassport', e.selectedPassport);


                        if(e.notFilteredPeopleList[i].PassportState == e.selectedPassport){

                        }else{
                             toAdd = false;
                        }
                    } else {

                        $window.sessionStorage.setItem('selectedPassport', '');
                    }

                     if(e.selectedRecordbook){

                        $window.sessionStorage.setItem('selectedRecordbook', e.selectedRecordbook);

                        if(e.notFilteredPeopleList[i].StudentBookState == e.selectedRecordbook){

                        }else{
                             toAdd = false;
                        }
                    } else {

                        $window.sessionStorage.setItem('selectedRecordbook', '');
                    }


                    if(e.selectedIncome){

                        $window.sessionStorage.setItem('selectedIncome', e.selectedIncome);

                        if(e.notFilteredPeopleList[i].IncomeState == e.selectedIncome){

                        }else{
                             toAdd = false;
                        }
                    } else {

                        $window.sessionStorage.setItem('selectedIncome', '');
                    }


                     if(e.selectedCitizenship){

                        $window.sessionStorage.setItem('selectedCitizenship', e.selectedCitizenship);

                        if(e.notFilteredPeopleList[i].Citizenship == e.selectedCitizenship){

                        }else{
                             toAdd = false;
                        }
                    } else {

                        $window.sessionStorage.setItem('selectedCitizenship', '');
                    }


                    if(toAdd){
                         e.filteredPeopleList.push(e.notFilteredPeopleList[i]);
                    }

                }

                e.grantPeople = e.filteredPeopleList;


            }



            e.saveConfig = function () {
                gp.mask(e);

                $http.get(API_CONFIG.serverPath +'api/config/setbyname/' + e.usersCantEditOption)
                .success(function (data, status) {
                     gp.unmask(e);

                      $mdToast.show(
                        $mdToast.simple()
                        .content('Настройки успешно сохранены')
                        .position('top'));


                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));


                  });

            };


          e.getConfig = function () {
                gp.mask(e);

                $http.get(API_CONFIG.serverPath +'api/config/getbyname/')
                .success(function (resp, status) {
                     gp.unmask(e);

                     if(resp && resp.Data){
                         e.usersCantEditOption = resp.Data;
                     } else {
                         e.usersCantEditOption = false;
                     }




                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));


                  });

            };

              var getStatusName = function(status) {

                    switch (status) {
                      case 1:
                        return 'Черновик';
                      case 2:
                        return 'Регистрация участников';
                      case 4:
                        return 'Выбор победителей';
                       case 8:
                        return 'Выбор победителей завершен';
                      case 16:
                        return 'Выдача гранта победителям';
                      case 32:
                        return 'Завершение конкурса';
                      case 64:
                        return 'Удален';
                      case 128:
                        return 'Регистрация участников закрыта';
                      case 256:
                        return 'Выбор победителей завершен';
                       case 512:
                        return 'Выбор дополнительных победителей завершен';
                        case 1024:
                        return 'Выдача гранта победителям';
                        case 2048:
                        return 'Завершение конкурса';
                      default :
                         return '';
                    }
            }


             e.loadUserGrantData = function (studentId) {
                gp.mask(e);


                $http.get(API_CONFIG.serverPath +'api/grantstudent/getusergrants/' + studentId)
                .success(function (resp, status) {
                     gp.unmask(e);

                     var data = resp.Data;

                    var result = [];

                    for (var i = 0; i < data.length; i++) {
                        result.push({
                            Id: data[i].Id,
                            Name: data[i].Name,
                            thumb: data[i].ImageLink,
                            Status: data[i].IsWinner ? '<b>Победитель</b>' :
                                    (data[i].IsAdditionalWinner ? '<b>Дополнительный победитель</b>' : '<b>Участник</b>'),

                            // '<br>' + 'Статус: ' + '<b>' + getStatusName(data[i].Status) + '</b>' + '<br>' +
                                      //        'Окончание регистрации: ' + '<b>' + (new Date(data[i].ExpiresDate).toLocaleDateString()) + '</b>' ,
                            Description: data[i].Description
                        });
                    }

                    if (result.length > 0) {
                        e.contents = result;
                    }

                  }).error(function (error) {
                     gp.unmask(e);

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));


                  });
            };

             e.loadUserStat =  function()
            {
              gp.mask(e);

              $http.get(API_CONFIG.serverPath + 'api/students/getuserstat/0')
                .success(function (resp, status) {
                   gp.unmask(e);

                    e.userStat = resp.Data;

                  });
            }


            e.loadIsProfileFilled = function()
            {
               $http.get(API_CONFIG.serverPath +'api/students/isprofileFilled/'+ 0)
                    .success(function (data, status) {

                        e.isprofileFilled = data;

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
            }
            e.loadIsProfileFilled();


            e.loadSelectedModeratorUsers = function (grantId) {

                gp.mask(e);

               $http.get(API_CONFIG.serverPath +'api/students/getmoderators/')
                .success(function (data, status) {
                     gp.unmask(e);

                    var result = [];
                    for (var i = 0; i < data.length; i++) {
                        result.push({
                            id : data[i].Id,
                            name:  data[i].Name  + ' ' + data[i].LastName + ' ' + data[i].Patronymic,
                            thumb: data[i].ImageLink,
                            selected: true
                        });
                    }

                    if (result.length > 0) {
                        e.userSelectedModerators = result;
                        gp.unmask(e);
                    }
                    else
                    {
                      e.userSelectedModerators = [];
                    }

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
            };

            e.loadSelectedModeratorUsers();

            e.loadAdminUsers = function (text) {

                gp.mask(e);

                if(text){} else{
                    return;
                }

               $http.get(API_CONFIG.serverPath +'api/students/getbyname/' + text)
                .success(function (data, status) {
                     gp.unmask(e);

                    var result = [];
                    for (var i = 0; i < data.length; i++) {

                        var exist = false;

                         e.userSelectedModerators.forEach(function(adm) {
                           if(adm.id == data[i].Id) { exist = true;  }
                         });

                         if(!exist){

                         result.push({
                            id : data[i].Id,
                            name: data[i].Name + ' ' + data[i].LastName,
                            thumb: data[i].ImageLink,
                            selected: false
                        });

                         }
                    }

                    if (result.length > 0) {
                        e.userModerators = result;
                        gp.unmask(e);
                    }

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
            };

            e.searchModerator = function()
            {
              if(e.search && e.search.name && e.search.name.length > 2) {

                e.loadAdminUsers(e.search.name);

              } else {

                e.userModerators = [];

              }

            }


            e.searchOtherUser = function()
            {
              if(e.searchUser && e.searchUser.name && e.searchUser.name.length > 2) {


                $window.sessionStorage.setItem('searchUser', e.searchUser.name);
                e.loadUsers(e.searchUser.name);

              } else {

                 if(e.searchUser && e.searchUser.name.length == 0){
                     $window.sessionStorage.setItem('searchUser', '');
                    //e.loadPeople();
                    e.loadUsers(null);
                 }

              }

            }

           e.AddAdmin = function(item) {

              if(item && item.id){

                var exist = false;

                e.userSelectedModerators.forEach(function(adm) {
                    if(adm.id == item.id) { exist = true;  }
                });

                if(!exist){
                     e.userSelectedModerators.push(item);

                     item.selected = true;

                     for(var i = e.userModerators.length-1; i>=0;  i--){
                      if (e.userModerators[i].id == item.id) e.userModerators.splice(i, 1);
                    }
                }

                e.search.name = '';

              }

            }

            e.getselectedAdminIds = function (){
                var result = [];


                if(e.userSelectedModerators){

                   e.userSelectedModerators.forEach(function(item) {
                     if(item.selected) { result.push(parseInt(item.id)); }
                   });

                }

                return result;
            }


            e.updateModerators = function (){

              gp.mask(e);

              var adminArr = {
                ids: e.getselectedAdminIds()
               };

              $http.put(API_CONFIG.serverPath + "api/students/setmoderators",  adminArr).success(function (data, status) {
                     gp.unmask(e);

                     $mdToast.show(
                        $mdToast.simple()
                        .content('Модераторы успешно обновлены')
                        .position('top'));

                     e.loadSelectedModeratorUsers();

                  }).error(function (error) {

                   $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
            }

            e.loadIsAchievementFilled = function()
            {
               $http.get(API_CONFIG.serverPath +'api/achievement/isachievementFilled/'+ 0)
                    .success(function (data, status) {

                        e.isachievementFilled = data;

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
            }

                 e.timelineScroll =  function() {

                       
                          $("#admin-panel-content-view").parent().parent().bind('scroll', function() {

                           var raw = this;

                            if (raw.scrollTop + raw.offsetHeight +200 >= raw.scrollHeight) {
                                      e.loadMore();
                                }
                          });

                          $(".elements-image-table-example thead tr th").click(function() {

                                  e.grantPeople = [];
                                    setTimeout(function (){

                                      e.loadUsers();

                                    }, 500);

                            });

                    };



            e.loadIsAchievementFilled();



            e.loadInfoVersionList = function(studentId)
            {

               $http.get(API_CONFIG.serverPath +'api/personalInfo/getVersions/'+ studentId)
                .success(function (data, status) {
                     gp.unmask(e);

                     e.infoVersionList = data;

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });

            }


                 var stateName = $state.current.name;

            if($stateParams.param){
                window.studentId = $stateParams.param;
            }

             if($stateParams.tab){
                  userProfileService.goTab(1 + parseInt($stateParams.tab));
            }

            switch (stateName) {


                      case "admin-panel.default.g-user-users":

                          e.loadGrantUnivers();
                          e.loadGrantList();
                           e.loadUsers(null);
                           e.loadUserStat();

                             e.$on('ngRepeatFinished', function(ngRepeatFinishedEvent) {
                              e.timelineScroll();
                            })

                          break;

                      case "admin-panel.default.g-user-page":

                      if(studentId !=null){
                        e.studentId = studentId;
                        e.loadStudentInfo(studentId);
                        e.loadAchievements(studentId);
                        e.loadUserGrantData(studentId);
                        e.loadInfoVersionList(studentId);


                        if(e.roleInfo) {

                            if(e.isAdmin || e.isModerator)
                            {
                                e.loadPersonalInfo(studentId,0);
                            }

                         }
                         else
                         {
                            e.defferredLoadPersonalInfo =  function(){
                                e.loadPersonalInfo(studentId,0);
                                e.defferredLoadPersonalInfo = null;
                            }
                         }

                        e.loadStudentBook(studentId);
                        e.loadIncomeFiles(studentId);


                      } else {

                         $state.go('admin-panel.default.g-user-profile');
                      }

                      break;

                      case 'admin-panel.default.g-user-profile':

                        e.getConfig();

                     break;

                       case 'admin-panel.default.g-user-settings':

                        e.getConfig();

                     break;


                       case 'admin-panel.default.g-user-moderators':

                        e.loadModeratorStat();

                     break;
            }
        }
    ])
    .controller("AvatarUploadController", [
        "$scope", "studentResource", "universityResource", "API_CONFIG", "$mdDialog", "FileUploader",
        function (e, studentResource, universityResource, API_CONFIG, $mdDialog, FileUploader) {


        }
    ]);

