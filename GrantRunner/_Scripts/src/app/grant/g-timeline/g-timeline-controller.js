'use strict';

        angular.module("g-timeline")
        .controller("TimeLineController", 
            [ "$scope", "grantResource", "$mdToast", "$filter", "$state", "gProgress", 'FileUploader' , 'API_CONFIG', '$http' , '$stateParams', '$mdDialog', '$window', '$timeout', 
            function (e, grantResource, $mdToast, $filter, $state, gProgress, FileUploader, API_CONFIG, $http, $stateParams, $mdDialog, $window, $timeout) 
                {

                     e.eventTypeList = [];
                    e.eventTypeList.push({Id: 0, Name: 'Создание гранта'});
                    e.eventTypeList.push({Id: 1, Name: 'Удаление гранта'});
                    e.eventTypeList.push({Id: 2, Name: 'Открытие регистрации в конкурсе '});
                    e.eventTypeList.push({Id: 3, Name: 'Закрытие регистрация в конкурсе '});
                    e.eventTypeList.push({Id: 4, Name: 'Пользователь стал участником конкурса'});
                    e.eventTypeList.push({Id: 5, Name: 'Пользователь отказался от участия'});
                    e.eventTypeList.push({Id: 6, Name: 'Открыт Выбор победителей конкурса'});
                    e.eventTypeList.push({Id: 7, Name: 'Закрыт Выбор победителей конкурса'});
                    e.eventTypeList.push({Id: 8, Name: 'Открыт выбор дополнительных победителей конкурса'});
                    e.eventTypeList.push({Id: 9, Name: 'Закрыт выбор дополнительных победителей конкурса'});
                    e.eventTypeList.push({Id: 10, Name: 'Открыта выдача гранта победителям'});
                    e.eventTypeList.push({Id: 11, Name: 'Закрыта выдача гранта победителям'});
                    e.eventTypeList.push({Id: 12, Name: 'Выбран основной победитель конкурса'});
                    e.eventTypeList.push({Id: 13, Name: 'Отменен основной победитель конкурса'});
                    e.eventTypeList.push({Id: 14, Name: 'Выбран дополнительный победитель конкурса'});
                    e.eventTypeList.push({Id: 15, Name: 'Отменен дополнительный победитель конкурса'});
                    e.eventTypeList.push({Id: 16, Name: 'Изменен грант'});
                    e.eventTypeList.push({Id: 17, Name: 'Грант переведен в черновик'});
                    e.eventTypeList.push({Id: 18, Name: 'Запущен этап завершения конкурса'});
                    e.eventTypeList.push({Id: 19, Name: 'Этап завершения конкурса отменен'});
                    e.eventTypeList.push({Id: 20, Name: 'Изменение квот'});
                    e.eventTypeList.push({Id: 21, Name: 'Загружен отчет о победителях'});
                    e.eventTypeList.push({Id: 22, Name: 'Загружен отчет о дополнительных победителях'});
                    e.eventTypeList.push({Id: 23, Name: 'Удален отчет о победителях'});
                    e.eventTypeList.push({Id: 24, Name: 'Удален отчет о дополнительных победителях'});
                    e.eventTypeList.push({Id: 25, Name: 'Модератор валидировал данные'});
                    e.eventTypeList.push({Id: 26, Name: 'Полный доступ'});
                    e.eventTypeList.push({Id: 27, Name: 'Прикреплен отчет о победителях'});
                    e.eventTypeList.push({Id: 28, Name: 'Остановлено редактирование данных'});
                    e.eventTypeList.push({Id: 29 , Name: 'Возобновлено редактирование данных'});
                   

                    e.loadEvents = 
                            function() {
                        gProgress.mask(e);

                          var eventFilter = {
                                 Type : e.selectedEventType ? e.selectedEventType : null,
                                UserId : 0,
                                After : e.eventStartDateParsed,
                                Before : e.eventEndDateParsed,
                                LastId : 0,
                                Search : e.eventSearchText
                            };


                        $http.put(API_CONFIG.serverPath +'api/timeline/0', eventFilter )
                        .success(function (data, status) {
                             gProgress.unmask(e);

                            var result = [];
                             for (var i = 0; i < data.length; i++) {
                                result.push({
                                    id: data[i].Id,
                                    title: data[i].Title,
                                    subtitle: data[i].Subtitle,
                                    image: data[i].Image,
                                    content: data[i].Content,
                                    palette: data[i].Palette,
                                    eventType: data[i].EventType,
                                    date: new Date(data[i].EventDate).toLocaleDateString(),
                                    time: data[i].EventDate.replace('T',' ').substr(11,8).replace('.',''),
                                    description: data[i].Description,
                                    name: data[i].Name,
                                    conditions: data[i].Conditions,
                                    dateChange: data[i].DateChange,
                                    quotaChanged: data[i].QuotaChanged,
                                    administrators: data[i].ChangeAdmin,
                                    link: '/#/user/page/' + data[i].StudentId                       
                                });
                            }

                            if (result.length > 0) {
                                e.events = result;
                            } else {
                                 e.events = [];
                            }

                            gProgress.unmask(e);

                          }).error(function (error) {

                            $mdToast.show(
                                $mdToast.simple()
                                .content(error.data.Message)
                                .theme("error-toast")
                                .position('top'));
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

                    e.loadMore = function() {

                        if(e.noMoreEvents){
                            return;
                        }


                        if(e.isLoadingMoreEvents){
                            return;
                        }

                        e.isLoadingMoreEvents = true;


                        gProgress.mask(e);


                        var lastId = 0;

                        if(e.events && e.events.length > 0){

                             lastId = e.events[e.events.length-1].id;
                         }

                          var eventFilter = {
                                 Type : e.selectedEventType ? e.selectedEventType : null,
                                UserId : 0,
                                After : e.eventStartDateParsed,
                                Before : e.eventEndDateParsed,
                                LastId : lastId,
                                Search : e.eventSearchText
                            };

                        $http.put(API_CONFIG.serverPath +'api/timeline/0', eventFilter)
                        .success(function (data, status) {
                             gProgress.unmask(e);

                            var result = [];
                             for (var i = 0; i < data.length; i++) {
                                e.events.push({
                                    id: data[i].Id,
                                    title: data[i].Title,
                                    subtitle: data[i].Subtitle,
                                    image: data[i].Image,
                                    content: data[i].Content,
                                    palette: data[i].Palette,
                                    eventType: data[i].EventType,
                                    date: new Date(data[i].EventDate).toLocaleDateString(),
                                    time: data[i].EventDate.replace('T',' ').substr(11,8).replace('.',''),
                                    description: data[i].Description,
                                    name: data[i].Name,
                                    conditions: data[i].Conditions,
                                    dateChange: data[i].DateChange,
                                    quotaChanged: data[i].QuotaChanged,
                                    administrators: data[i].ChangeAdmin,
                                    link: '/#/user/page/' + data[i].StudentId                       
                                });
                            }

                            if(data.length==0){
                                e.noMoreEvents = true;
                            }

                           e.isLoadingMoreEvents = false;

                          }).error(function (error) {

                            $mdToast.show(
                                $mdToast.simple()
                                .content(error.data.Message)
                                .theme("error-toast")
                                .position('top'));

                             e.isLoadingMoreEvents = false;
                          });

                          
                    }

                     e.applyEventFilter = function(){
                        e.noMoreEvents = false;

                        e.loadEvents();
                    }


                    e.timelineScroll =  function() {

                     

                          $("#admin-panel-content-view").parent().parent().bind('scroll', function() {

                           var raw = this;

                            if (raw.scrollTop + raw.offsetHeight +200 >= raw.scrollHeight) {
                                      e.loadMore();
                                }      
                          });

                    };


                    e.$on('ngRepeatFinished', function(ngRepeatFinishedEvent) {
                              e.timelineScroll();
                            })


            e.loadEvents();
            
                } 
            ]);