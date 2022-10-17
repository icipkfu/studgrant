'use strict';

angular.module("g-grants")
    .controller("GrantsController", [
        "$scope", "grantResource", "$mdToast", "$filter", "$state", "gProgress", 'FileUploader' , 'API_CONFIG', '$http' , '$stateParams', '$mdDialog', '$window', '$timeout',
        function (e, grantResource, $mdToast, $filter, $state, gProgress, FileUploader, API_CONFIG, $http, $stateParams, $mdDialog, $window, $timeout) {
            e.selected = null;
            ///  для дат 

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
                    if (isNaN(date)) throw SyntaxError("invalid date");

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

            ////  для дат

            var homeMenuItem =           
                            {
                                name: "К списку конкурсов",
                                icon: "icon-person",
                                type: 'link',
                                state: "admin-panel.default.g-grants-list",
                                priority: 1
                            };


            e.universityQuotas = [];
            e.grantPeople = [];
            e.grantAdditionalPeople = [];

            e.selectedRecordbook = null,
            e.selectedAchievements = null,
            e.selectedScore  = null,
            e.selectedIncome = null;


            e.columns = [
                {
                    title: "",
                    field: "thumb",
                    sortable: false,
                    filter: "tableImage"
                }, {
                    title: "Наименование",
                    field: "Name",
                    sortable: true
                }
            ];


            e.quotaColumns = [
                {
                    title: "",
                    field: "thumb",
                    sortable: false,
                    filter: "tableImage"
                },
                {
                    title: "Тип",
                    field: "univerType",
                    sortable: true
                },
                {
                    title: "Зона",
                    field: "city",
                    sortable: true
                },
                {
                    title: "Город",
                    field: "town",
                    sortable: true
                },
                {
                    title: "Образовательная организация",
                    field: "univerlink",
                    sortable: true
                }, {
                    title: "Квота",
                    field: "quota",
                    sortable: true
                }, {
                    title: "Участников",
                    field: "studentCount",
                    sortable: true
                }, {
                    title: "Победителей",
                    field: "winnerCount",
                    sortable: true
                },
                {
                    title: "Зарегистрированных",
                    field: "RegisteredCount",
                    sortable: true
                },
                {
                    title: "Отчет",
                    field: "ReportLink",
                    sortable: true
                },
                {
                    title: "",
                    field: "link",
                    sortable: false
                }


            ];

            e.peopleSocialColumns = [
              {
                  title: "",
                  field: "thumb",
                  sortable: false,
                  filter: "tableImage"
              },  {
                  title: "Фамилия И. О.",
                  field: "Fio",
                  sortable: true
              },
              { 
                  title: "Зачетка",
                  field: "StudentBookStateName",
                  sortable: true
              },
              { 
                title: "Справка о доходах",
                field: "IncomeStateName",
                sortable: true
              },
              { 
                title: "Среднедушевой доход",
                field: "Income",  
                sortable: true
              },
               {
                title: "",
                field:"Link",
                sortable: false
              }
          ];


            e.peopleColumns = [
                {
                    title: "",
                    field: "thumb",
                    sortable: false,
                    filter: "tableImage"
                }, {
                    title: "Выбрать победителем",
                    field: "IsWinner",
                    sortable: false
                }, {
                    title: "Фамилия И. О.",
                    field: "Fio",
                    sortable: true
                }, {
                    title: "Баллы",
                    field: "Score",
                    sortable: true
                },
                /* {
                    title: "Проверка перс. данных",
                    field: "IsPassportValid",
                    sortable: true
                },*/
                { 
                    title: "Зачетка",
                    field: "IsStudentBookValid",
                    sortable: true
                },
                { 
                  title: "Корректные достижения",
                  field: "HasValidAchievements",
                  sortable: true
                },
                { 
                  title: "Некорректные достижения",
                  field: "HasInvalidAchievements",  
                  sortable: true
                },
                { 
                  title: "Непроверенные достижения",
                  field: "HasNotCheckedAchievements",  
                  sortable: true
                },
                
                
                {
                    title: "",
                    field: "StudentId",
                    sortable: false
                },
                 {
                  title: "",
                  field:"Link",
                  sortable: false
                }
            ];

            e.additionalColumns = [
                {
                    title: "",
                    field: "thumb",
                    sortable: false,
                    filter: "tableImage"
                }, {
                    title: "Выбрать победителем",
                    field: "IsAdditionalWinner",
                    sortable: false
                }, {
                    title: "Фамилия И. О.",
                    field: "Fio",
                    sortable: true
                }, {
                    title: "Баллы",
                    field: "Score",
                    sortable: true
                }, 
                /*{
                    title: "Валидация данных паспорта",
                    field: "IsPassportValid",
                    sortable: true
                }, */
                 {
                    title: "Валидация зачетной книжки",
                    field: "IsStudentBookValid",
                    sortable: true
                }, {
                    title: "",
                    field: "StudentId",
                    sortable: false
                }
            ];


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
         


            e.curDate = new Date();
            e.maxDate = new Date();
            e.maxDate.setYear(e.curDate.getFullYear() + 1);

            e.checkAdmistrators = function (adminsArr) 
            { 
              if(e.userAdmins)
                  { e.userAdmins.forEach(function(item) 
                    { if(adminsArr.indexOf(item.Id) >= 0) { item.selected = true; } 
                    }); 
                  } 
            }

            

            e.setCurStage = function (status)
            {
               if(e.grant.stages) { } else 
               {
                    e.grant.stages = {};        
               }

                     //todo переделать
                     switch(status) {
                          case 1:
                            e.grant.stages.Draft = true;
                            e.grant.stages.Registration = false;

                             e.grant.stages.WinnersSelection = null;
                             e.grant.stages.AddtitionalSelection = null;
                             e.grant.stages.Delivery = null;
                             e.grant.stages.Final = null;

                            break
                          case 2:
                              e.grant.stages.Registration = true;
                              e.grant.stages.Draft = false;
                            break
                          case 4:

                             e.grant.stages.WinnersSelection = true;
                             e.grant.stages.Registration = null;
                             e.grant.stages.Draft = false;
                              e.grant.stages.AddtitionalSelection = null;
                           break

                          case 8:
                             e.grant.stages.AddtitionalSelection = true;
                             e.grant.stages.Draft = false;

                            var additionalMenuItem =           
                            {
                                name: "Невыбранные участники",
                                icon: "icon-person",
                                type: 'link',
                                state: "admin-panel.default.g-grants-additional",
                                 params: e.grant.id,
                                priority: 9
                            };


                            var deferredAdditionalMenuFunc = function()

                            {
                              if(menu.length < 10){
                                 menu.push(additionalMenuItem);
                              }
                              
                                e.deferredAdditionalMenuFunc = null;
                            }


                            if(e.roleInfo){
                              if(e.isAdmin || e.isGrantAdmin){

                                     deferredAdditionalMenuFunc();
                              }
                            } else
                            {
                                  e.deferredAdditionalMenuFunc = deferredAdditionalMenuFunc;
                            }

                        
                           break
                          case 16:
                              e.grant.stages.Delivery = true;
                              e.grant.stages.Draft = null;
                              e.grant.stages.Registration = null;
                              e.grant.stages.WinnersSelection = null;
                              e.grant.stages.AddtitionalSelection = null;
                              e.grant.stages.Final = false;
                           break
                          case 32:
                              e.grant.stages.Final = true;
                              e.grant.stages.Delivery = null;
                              e.grant.stages.Draft = null;
                              e.grant.stages.Registration = null;
                              e.grant.stages.WinnersSelection = null;
                              e.grant.stages.AddtitionalSelection = null;
                           break
                           case 64:
                             e.grant.stages.Deleted = true;
                              e.grant.stages.Draft = false;
                           case 128:
                              e.grant.stages.Registration = false;
                              e.grant.stages.WinnersSelection = false;
                              e.grant.stages.Draft = false;
                           break;
                           case 256:
                              e.grant.stages.Registration = null;
                              e.grant.stages.WinnersSelection = false;
                              e.grant.stages.Draft = false;
                              e.grant.stages.AddtitionalSelection = false;
                           break;
                           case 512:
                              e.grant.stages.Registration = null;
                              e.grant.stages.WinnersSelection = null;
                              e.grant.stages.Draft = false;
                              e.grant.stages.AddtitionalSelection = false;
                              e.grant.stages.Delivery = false;

                            if(menu.length==10){
                               menu.pop();
                             }
                           break;
                            case 1024:
                              e.grant.stages.Final = null;
                              e.grant.stages.Delivery = false;
                              e.grant.stages.Draft = false;
                              e.grant.stages.Registration = false;
                              e.grant.stages.WinnersSelection = false;
                              e.grant.stages.AddtitionalSelection = false;
                           break;
                           case 2048:
                              e.grant.stages.Final = false;
                              e.grant.stages.Delivery = false;
                              e.grant.stages.Draft = null;
                              e.grant.stages.Registration = null;
                              e.grant.stages.WinnersSelection = false;
                              e.grant.stages.AddtitionalSelection = false;
                           break;

                          default :
                             break
                     } 

                      e.grant.statusName = getStatusName(status)
            }



            e.loadIsParticipant = function (id) {
                gProgress.mask(e);

                $http.get(API_CONFIG.serverPath +'api/grantstudent/isparticipant/' + id)
                .success(function (data, status) {
                     gProgress.unmask(e);

                     e.IsParticipant = data;

                       
                     if(e.roleInfo) {
                      
                           if(e.isAdmin){
                               e.MyGrantStatus = 'Администратор';
                           }
                           else {
                                if(e.isGrantAdmin){
                                     e.MyGrantStatus = 'Администратор гранта';
                                 } else {

                                     if(e.IsParticipant){
                                      e.MyGrantStatus = 'Уже участвую';
                                     } else {
                                       e.MyGrantStatus = 'Еще не участвую';
                                     }
                                 } 
                           } 
                    }
                    else
                    {
                       e.deferredIsParticipant = function(){
                          if(e.isAdmin){
                               e.MyGrantStatus = 'Администратор';
                           }
                           else {
                                if(e.isGrantAdmin){
                                     e.MyGrantStatus = 'Администратор гранта';
                                 } else {

                                     if(e.IsParticipant){
                                      e.MyGrantStatus = 'Уже участвую';
                                     } else {
                                       e.MyGrantStatus = 'Еще не участвую';
                                     }
                                 } 
                           } 

                            e.deferredIsParticipant = null;


                       }
                    }

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));


                  });
              
            };
             
             e.loadGrant = function (id) {
                gProgress.mask(e);

                $http.get(API_CONFIG.serverPath +'api/grant/' + id)
                .success(function (data, status) {
                    // gProgress.unmask(e);

                     if(e.grant){

                     } else{
                        e.grant = {};
                     }

                    e.grant.id = data.Id;
                    e.grant.name = data.Name;
                    e.grant.description = data.Description;
                    e.grant.imagefile = data.ImageFile;
                    e.grant.attachmentFiles = data.AttachmentFiles;
                    e.grant.conditions = data.Conditions;
                    e.grant.administrators = data.Administrators;
                    e.grant.status = data.Status;
                    e.grant.expiresDate = new Date(data.ExpiresDate);
                    e.grant.expiresDateText = e.grant.expiresDate.format('dd.mm.yyyy');
                    e.grant.fullQuota = data.FullQuota;
                    e.grant.expiresDateString = new Date(data.ExpiresDate).toLocaleDateString();
                    e.grant.thumb = data.ImageLink;
                    e.grant.AttachmentsLinks = data.AttachmentsLinks;
                    e.grant.statusAndDate = '<br>' + 'Этап: ' + '<b>' + getStatusName(data.Status) + '</b>' + '<br>' + 
                                              'Окончание регистрации: ' + '<b>' + (new Date(data.ExpiresDate).toLocaleDateString()) + '</b>' ;

                    e.grant.nowDate = (new Date()).format('dd.mm.yyyy');
                    e.grant.CanAddReport = data.CanAddReport;

                    e.setCurStage(data.Status);
                    e.setPageTitle(e.grant.name);


                   // loadAdminUsers();

                    e.loadIsParticipant(e.grant.id);
                   // e.loadRole();
                    gProgress.unmask(e);

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));


                  });
              
            };


            e.loadEvents = function() {

              var id = grantId;

                gProgress.mask(e);

                var eventFilter = {
                    Type : e.selectedEventType ? e.selectedEventType : null,
                    UserId : 0,
                    After : e.eventStartDateParsed,
                    Before : e.eventEndDateParsed,
                    LastId : 0,
                    Search : e.eventSearchText
                };

                $http.put(API_CONFIG.serverPath +'api/grantevent/' + id, eventFilter)
                .success(function (data, status) {
                     gProgress.unmask(e);


                    var result = [];
                     for (var i = 0; i < data.length; i++) {
                        result.push({
                            id: data[i].Id,
                            title: data[i].Title,
                            subtitle: data[i].Subtitle,
                            image: data[i].Image,
                            name: data[i].Name,
                            content: data[i].Content,
                            palette: data[i].Palette,
                            date: new Date(data[i].EventDate).toLocaleDateString(),
                            time: data[i].EventDate.replace('T',' ').substr(11,8).replace('.',''),
                            description: data[i].Description,
                            conditions: data[i].Conditions,
                            dateChange: data[i].DateChange,
                            quotaChanged: data[i].QuotaChanged,
                            administrators: data[i].ChangeAdmin,
                            link: '/#/user/page/' + data[i].StudentId  
                        });
                    }

                    if (result.length > 0) {
                        e.events = result;
                        gProgress.unmask(e);
                    } else{
                       e.events = [];
                    }

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
              
            };




           // todo прокидывать имена состояний enum с бекенда
           //temporary
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

            var loadData = function () {
                gProgress.mask(e);

                grantResource.query(function (data) {
                    var result = [];

                    for (var i = 0; i < data.length; i++) {
                        result.push({
                            Id: data[i].Id,
                            Name: data[i].Name,
                            Curator: data[i].Curator,
                            thumb: data[i].ImageLink,
                            Status:  '<br>' + 'Этап: ' + '<b>' + getStatusName(data[i].Status) + '</b>' + '<br>' + 
                                              'Окончание регистрации: ' + '<b>' + (new Date(data[i].ExpiresDate).toLocaleDateString()) + '</b>' ,
                            Description: data[i].Description,
                            Conditions: data[i].Conditions,
                            Attachments: data[i].AttachmentFiles
                        });
                    }

                    if (result.length > 0) {
                        e.contents = result;
                        gProgress.unmask(e);
                    }

                });
            };

            e.searchAdmin = function()
            {
              if(e.search && e.search.name && e.search.name.length > 2) {

                e.loadAdminUsers(e.search.name);

              } else {

                e.userAdmins = [];

              }

            }

            e.AddAdmin = function(item) {

              if(item && item.id){

                var exist = false;

                e.userSelectedAdmins.forEach(function(adm) {
                    if(adm.id == item.id) { exist = true;  }
                });

                if(!exist){
                     e.userSelectedAdmins.push(item);

                     item.selected = true;


                     for(var i = e.userAdmins.length-1; i>=0;  i--){
                      if (e.userAdmins[i].id == item.id) e.userAdmins.splice(i, 1);
                    }
                }

                if(e.search) { e.search.name = ''; }
              }

            
            }

             e.loadAdminUsers = function (text) {

                gProgress.mask(e);

               $http.get(API_CONFIG.serverPath +'api/students/getbyname/' + text)
                .success(function (data, status) {
                     gProgress.unmask(e);

                    var result = [];
                    for (var i = 0; i < data.length; i++) {

                      var exist = false;

                      e.userSelectedAdmins.forEach(function(adm) {
                          if(adm.id == data[i].Id) { exist = true;  }
                      });

                     if(!exist) {

                        result.push({
                            id : data[i].Id,
                            name: data[i].Name + ' ' + data[i].LastName,
                            thumb: data[i].ImageLink,
                            selected: false
                        });
                      }
                    }

                    if (result.length > 0) {
                        e.userAdmins = result;
                        gProgress.unmask(e);
                    }

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
            };

              e.loadSelectedAdminUsers = function (grantId) {

                gProgress.mask(e);

               $http.get(API_CONFIG.serverPath +'api/grant/getadmins/' + grantId)
                .success(function (data, status) {
                     gProgress.unmask(e);                     

                    var result = [];
                    for (var i = 0; i < data.length; i++) {
                        result.push({
                            id : data[i].Id,
                            name: data[i].Name + ' ' + data[i].LastName,
                            thumb: data[i].ImageLink,
                            selected: true
                        });
                    }

                    if (result.length > 0) {
                        e.userSelectedAdmins = result;
                        gProgress.unmask(e);
                    }
                    else
                    {
                      e.userSelectedAdmins = [];
                    }

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
            };


            var loadGrantUsers = function() {

                var grantUsers = [];
                grantUsers.push({
                            id : 1,
                            username : 'Студент1',
                            university: 'Вуз1',
                            thumb: "../assets/images/avatars/avatar-1.png"
                        });
                grantUsers.push({
                            id : 2,
                            username : 'Студент2',
                            university: 'Вуз2',
                            thumb: "../assets/images/avatars/avatar-2.png"
                        });
                grantUsers.push({
                            id : 1,
                            username : 'Студент3',
                            university: 'Вуз3',
                            thumb: "../assets/images/avatars/avatar-3.png"
                        });
                grantUsers.push({
                            id : 1,
                            username : 'Студент4',
                            university: 'Вуз1',
                            thumb: "../assets/images/avatars/avatar-4.png"
                        });
                grantUsers.push({
                            id : 1,
                            username : 'Студент5',
                            university: 'Вуз2',
                            thumb: "../assets/images/avatars/avatar-5.png"
                        });
                grantUsers.push({
                            id : 1,
                            username : 'Студент6',
                            university: 'Вуз3',
                            thumb: "../assets/images/avatars/avatar-6.png"
                        });


                e.grantUsers = grantUsers;

            }


            e.loadGrantStages = function (id){

                var test = '';

            }

            e.returnMenu = function(){

                    while(menu.length>0){
                        menu.pop();
                    }

                    oldMenu.forEach(function(item) {
                       menu.push(item);
                    });

                     window.oldMenu = null;
            }
            

            loadData();

            loadGrantUsers();

            e.clearGrant = function (){

                     e.grant.id = null;
                     e.grant.name = '';
                     e.grant.description = '';
                     e.grant.imagefile = '';
                     e.grant.attachmentFiles = '';
                     e.grant.conditions = '';

                     e.uploader.clearQueue();
                     e.attachUploader.clearQueue();
            }

            e.startGrant = function (){
                $http.get(API_CONFIG.serverPath +'api/grant/start/' + e.grant.id)
                .success(function (data, status) {
                     gProgress.unmask(e);

                     $mdToast.show(
                        $mdToast.simple()
                        .content('Грант успешно запущен')
                        .position('top'));

                     e.grant.status = 2;
                     e.setCurStage(2);

                  }).error(function (error) {
                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });


            }

            e.closeRegistrationGrant = function (){

              $http.get(API_CONFIG.serverPath +'api/grant/closeregistration/' + e.grant.id)
                .success(function (data, status) {
                     gProgress.unmask(e);

                     $mdToast.show(
                        $mdToast.simple()
                        .content('Регистрация участников закрыта')
                        .position('top'));

                     e.grant.status = 128;
                     e.setCurStage(128);

                  }).error(function (error) {
                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });


            }  


            e.makeDraftGrant = function (){

              $http.get(API_CONFIG.serverPath +'api/grant/returndraft/' + e.grant.id)
                .success(function (data, status) {
                     gProgress.unmask(e);

                     $mdToast.show(
                        $mdToast.simple()
                        .content('Грант переведен в статус черновика')
                        .position('top'));

                     e.grant.status = 1;
                     e.setCurStage(1);

                  }).error(function (error) {
                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });


            } 


            e.makeProgressTabs = function() {

              var tab1 = $(".md-tab")[0];
              $(tab1).html('<span class="grant-step-label ng-scope">1</span> <span ng-scope">Описание</span> ');

              var tab2 = $(".md-tab")[1];
              $(tab2).html('<span style="color: rgb(0,188,212);" class="grant-step-label ng-scope">2</span> <span style="color: rgb(0,188,212);" ng-scope">Условия проведения</span> ');
            }


            e.agreeConditions = function(){
                 $http.get(API_CONFIG.serverPath +'api/grantstudent/agreegrant/' + e.grant.id + '/' + e.participantCode)
                .success(function (data, status) {
                     gProgress.unmask(e);

                     $mdToast.show(
                        $mdToast.simple()
                        .content('Вы успешно зарегистрировались в гранте')
                        .position('top'));

                      e.grantParticipateProgress = 100;

                      e.infoGrant(e.grant.id);

                  }).error(function (error) {
                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
            } 


           e.agreeGrant2 =  function(){

           $timeout(function(){
                e.grantParticipateProgress = 50;
             })
           }

           e.backToFirstTab2  =  function(){

                 $timeout(function(){
                e.grantParticipateProgress = 0;
             })
           }

           e.agreeGrant = function(){

            e.grantParticipateProgress = 50;

             var tabConditions = $(".md-tab")[1];

             $timeout(function(){
               $(tabConditions).trigger('click')
             })
           }

           e.controlProgress = function(){

           }

           e.backToFirstTab = function(){

             e.grantParticipateProgress = 0;

             var tabConditions = $(".md-tab")[0];

             $timeout(function(){
               $(tabConditions).trigger('click')
             })

           }

           e.declineGrant = function(){

              $http.get(API_CONFIG.serverPath +'api/grantstudent/declinegrant/' + e.grant.id)
                .success(function (data, status) {
                     gProgress.unmask(e);

                     $mdToast.show(
                        $mdToast.simple()
                        .content('Вы успешно отказались от участия в гранте')
                        .position('top'));

                     e.IsParticipant = false;

                      e.infoGrant(e.grant.id);

                  }).error(function (error) {
                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.Message)
                        .theme("error-toast")
                        .position('top'));

                     e.infoGrant(e.grant.id);
                  });


           }

           e.openWinnerSelectionGrant = function (){

              $http.get(API_CONFIG.serverPath +'api/grant/openwinnersselection/' + e.grant.id)
                .success(function (data, status) {
                     gProgress.unmask(e);

                     $mdToast.show(
                        $mdToast.simple()
                        .content('Запущен этап выбора основных победителей')
                        .position('top'));

                     e.grant.status = 4;
                     e.setCurStage(4);

                  }).error(function (error) {
                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });

            }  


             e.changeCanAddReport = function(){

                 $http.get(API_CONFIG.serverPath +'api/grant/changeCanAddReport/' + e.grant.id + '/' + e.grant.CanAddReport)
                .success(function (data, status) {
                     gProgress.unmask(e);


                    if(e.grant.CanAddReport){

                         $mdToast.show(
                            $mdToast.simple()
                            .content('Открыта возможность добавления/удаления отчета о победителях')
                            .position('top'));

                     } else {

                        $mdToast.show(
                            $mdToast.simple()
                            .content('Закрыта возможность добавления/удаления отчета о победителях')
                            .position('top'));
                     }


                  }).error(function (error) {
                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });


             }

           e.openAdditionalSelectionGrant = function (){

              $http.get(API_CONFIG.serverPath +'api/grant/openadditionalselection/' + e.grant.id)
                .success(function (data, status) {
                     gProgress.unmask(e);

                     $mdToast.show(
                        $mdToast.simple()
                        .content('Запущен этап выбора дополнительных победителей')
                        .position('top'));

                     e.grant.status = 8;
                     e.setCurStage(8);

                  }).error(function (error) {
                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });

            } 

            e.closeAdditionalSelectionGrant = function (){

              $http.get(API_CONFIG.serverPath +'api/grant/closeadditionalselection/' + e.grant.id)
                .success(function (data, status) {
                     gProgress.unmask(e);

                     $mdToast.show(
                        $mdToast.simple()
                        .content('Этап выбора дополнительных победителей завершен')
                        .position('top'));

                     e.grant.status = 512;
                     e.setCurStage(512);

                  }).error(function (error) {
                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });

            } 

            e.closeWinnerSelectionGrant = function (){

              $http.get(API_CONFIG.serverPath +'api/grant/closewinnersselection/' + e.grant.id)
                .success(function (data, status) {
                     gProgress.unmask(e);

                     $mdToast.show(
                        $mdToast.simple()
                        .content('Этап выбора основных победителей завершен')
                        .position('top'));

                     e.grant.status = 256;
                     e.setCurStage(256);

                  }).error(function (error) {
                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });

            }  

            e.openDeliveryGrant = function (){

              $http.get(API_CONFIG.serverPath +'api/grant/opendelivery/' + e.grant.id)
                .success(function (data, status) {
                     gProgress.unmask(e);

                     $mdToast.show(
                        $mdToast.simple()
                        .content('Запущен этап выдачи гранта победителям')
                        .position('top'));

                     e.grant.status = 16;
                     e.setCurStage(16);

                  }).error(function (error) {
                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });

            } 

            e.cancelDeliveryGrant = function (){

              $http.get(API_CONFIG.serverPath +'api/grant/canceldelivery/' + e.grant.id)
                .success(function (data, status) {
                     gProgress.unmask(e);

                     $mdToast.show(
                        $mdToast.simple()
                        .content('Этап выдачи гранта победителям отменен')
                        .position('top'));

                     e.grant.status = 1024;
                     e.setCurStage(1024);

                  }).error(function (error) {
                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });

            } 

            e.openFinalGrant = function (){

              $http.get(API_CONFIG.serverPath +'api/grant/openfinal/' + e.grant.id)
                .success(function (data, status) {
                     gProgress.unmask(e);

                     $mdToast.show(
                        $mdToast.simple()
                        .content('Запущен этап завершения конкурса')
                        .position('top'));

                     e.grant.status = 32;
                     e.setCurStage(32);

                  }).error(function (error) {
                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });

            }

            e.cancelFinalGrant = function (){

              $http.get(API_CONFIG.serverPath +'api/grant/cancelfinal/' + e.grant.id)
                .success(function (data, status) {
                     gProgress.unmask(e);

                     $mdToast.show(
                        $mdToast.simple()
                        .content('Этап завершения конкурса отменен')
                        .position('top'));

                     e.grant.status = 2048;
                     e.setCurStage(2048);

                  }).error(function (error) {
                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });

            }


            e.updateGrant = function (){

              gProgress.mask(e);

              if( e.grant.attachmentFiles){ }
                else{
                  e.grant.attachmentFiles = [];
                }

              e.grant.administrators = e.getselectedAdminIds();
              e.grant.attachmentFiles = '' + e.grant.attachmentFiles;

              $http.put(API_CONFIG.serverPath + "api/grant/"+ e.grant.id,  e.grant).success(function (data, status) {
                     gProgress.unmask(e);

                     $mdToast.show(
                        $mdToast.simple()
                        .content('Грант успешно обновлен')
                        .position('top'));

                     e.infoGrant(e.grant.id);

                  }).error(function (error) {
                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
            }

            e.confirmStart = function () {

                   $mdDialog.show(
                    $mdDialog.confirm()
                    .title('Запустить грант')
                    .content("Вы уверены, что хотите запустить грант?")
                    .ok("Да")
                    .cancel("Отмена")).then(function(result){

                      if(result){
                        e.startGrant();
                      }
              });
            }

            e.askDeclineGrant = function (){

                $mdDialog.show(
                    $mdDialog.confirm()
                    .title('Отказаться от участия')
                    .content("Вы уверены, что хотите отказаться от участия?")
                    .ok("Да")
                    .cancel("Отмена")).then(function(result){

                      if(result){
                         e.declineGrant(e.grant.id)
                      }  
                });   
            }

            e.confirmDelete = function () {

                   $mdDialog.show(
                    $mdDialog.confirm()
                    .title('Удалить грант')
                    .content("Вы уверены, что хотите удалить грант?")
                    .ok("Да")
                    .cancel("Отмена")).then(function(result){

                      if(result){
                        e.deleteGrant();
                      }     
              });
            }


            e.deleteGrant =  function () {
                gProgress.mask(e);

                $http.delete(API_CONFIG.serverPath +'api/grant/' + e.grant.id)
                .success(function (data, status) {
                     gProgress.unmask(e);

                     $mdToast.show(
                        $mdToast.simple()
                        .content('Грант успешно удален')
                        .position('top'));
                   e.clearGrant();

                  }).error(function (error) {
                    var msg = "Произошла ошибка";

                    if(error.data && error.data.Message) { msg = error.error.data.Message }
                      if(error.ExceptionMessage) { msg = error.ExceptionMessage }
                       $mdToast.show(
                        $mdToast.simple().content(msg).theme("error-toast").position('top'));
                   });
            }

            e.getselectedAdminIds = function (){
                var result = [];


                if(e.userSelectedAdmins){

                   e.userSelectedAdmins.forEach(function(item) {
                     if(item.selected) { result.push(item.id); }
                   });

                }

                if(result.length > 0)
                {
                   return '' + result;
                }

                return '';
            }

            e.createGrant = function () {

               if( e.grant.attachmentFiles){ }
                else{
                  e.grant.attachmentFiles = [];
                }

                var newGrant = new grantResource({
                    Name: e.grant.name,
                    Description: e.grant.description,
                    ExpiresDate: new Date(e.grant.expiresDate),
                    FullQuota: e.grant.fullQuota,
                    ImageFile:  e.grant.imagefile,
                    AttachmentFiles: '' + e.grant.attachmentFiles,
                    Conditions: e.grant.conditions,
                    Administrators: ""
                    //Administrators : e.getselectedAdminIds()
                });

                gProgress.mask(e);
                newGrant.$save(function (resp) {
                    gProgress.unmask(e);
                    $mdToast.show(
                        $mdToast.simple()
                        .content('Грант успешно создан')
                        .position('top'));

                    e.grant.id = resp.Id;
                    e.grant.status = resp.Status;

                   // e.goListGrant();

                     e.infoGrant(e.grant.id);
                    
                }, function (error) {

					$mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
						.theme("error-toast")
                        .position('top'));
						
                });
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
                e.grant.Name = "";
            };

             // uploaders
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
                        e.grant.imagefile =  response;
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


                e.deleteAttachments = function(){

                  e.grant.attachmentFiles = null;
                  e.grant.AttachmentsLinks = null;


                }


                // uploader for attachments

                var attachUploader = e.attachUploader = new FileUploader({
                    url: API_CONFIG.serverPath +'api/file'
                });

                attachUploader.onAfterAddingFile = function(fileItem)  {
                    var t = this;

                     gProgress.mask(e);

                    fileItem.upload();
                };

                attachUploader.filters.push({
                    name: 'pdfFilter',
                    fn: function(item /*{File|FileLikeObject}*/, options) {
                        var type = '|' + item.type.slice(item.type.lastIndexOf('/') + 1) + '|';
                        return '|pdf|'.indexOf(type) !== -1;
                    }
                });

                attachUploader.onCompleteItem = function(fileItem, response, status, headers) {

                   gProgress.unmask(e);

                    if(status == 200)
                    {

                       if(e.grant.attachmentFiles){
                            e.grant.attachmentFiles += '|' + response;
                       }
                       else{
                          e.grant.attachmentFiles = response;
                       }
                    }
                    else
                    {
                        $mdToast.show(
                        $mdToast.simple()
                        .content(response)
                        .theme("error-toast")
                        .position('top'));
                    }
                    
                };

                //uploader for winReport

                 var winReportUploader = e.winReportUploader = new FileUploader({
                    url: API_CONFIG.serverPath +'api/fileoriginal'
                });

                winReportUploader.onAfterAddingFile = function(fileItem)  {
                    var t = this;

                    fileItem.upload();
                };

                winReportUploader.onCompleteItem = function(fileItem, response, status, headers) {

                    if(status == 200)
                    {
                      e.addWinnerReport(grantId, response);
                      e.winReportUploader.clearQueue();
                    }
                    else
                    {
                        $mdToast.show(
                        $mdToast.simple()
                        .content(response)
                        .theme("error-toast")
                        .position('top'));
                    }
                    
                };

                var winAdditionalReportUploader = e.winAdditionalReportUploader = new FileUploader({
                    url: API_CONFIG.serverPath +'api/fileoriginal'
                });

                winAdditionalReportUploader.onAfterAddingFile = function(fileItem)  {
                    var t = this;

                    fileItem.upload();
                };

                winAdditionalReportUploader.onCompleteItem = function(fileItem, response, status, headers) {

                    if(status == 200)
                    {
                      e.addAdditionalWinnerReport(grantId, response);
                    }
                    else
                    {
                        $mdToast.show(
                        $mdToast.simple()
                        .content(response)
                        .theme("error-toast")
                        .position('top'));
                    }
                    
                };

                e.editGrant = function(id){

                    $state.go('admin-panel.default.g-grants-create', {'param': id});
                  
                }

                // e.infoGrant = function(id){

                  //  $state.go('admin-panel.default.g-grants-timeline', id);

                //}

                 e.stageGrant = function(id){

                    $state.go('admin-panel.default.g-grants-stage', {'param': id});

                }

                e.goeditGrant = function(){
                    $state.go('admin-panel.default.g-grants-create', {'param': grantId});
                }


                e.grantMenu = function(id){

                 var descriptionMenuItem =  {
                     name: "Конкурс",
                     icon: "icon-description",
                     type: 'link',
                     state: "admin-panel.default.g-grants-description",
                      params: id,
                     priority: 2
                 };

                 var timeLineMenuItem =  {
                     name: "Лента конкурса",
                     icon: "icon-access-time",
                     type: 'link',
                     state: 'admin-panel.default.g-grants-timeline',
                     params: id,
                     priority: 3
                 };

                var stageMenuItem =            
                            {
                                name: "Этапы гранта",
                                icon: "icon-expand-more",
                                type: 'link',
                                state: "admin-panel.default.g-grants-stage",
                                 params: id,
                                priority: 4
                            };

                var adminsMenuItem = 
                            {
                                name: "Администраторы гранта",
                                icon: "icon-people",
                                type: 'link',
                                state: "admin-panel.default.g-grants-admins",
                                 params: id,
                                priority: 5
                            };

                var univMenuItem =         
                            {
                                name: "Образовательные организации",
                                icon: "icon-account-balance",
                                type: 'link',
                                state: "admin-panel.default.g-grants-unv",
                                 params: id,
                                priority: 6
                            };

                var peopleMenuItem =           
                            {
                                name: "Участники",
                                icon: "icon-person",
                                type: 'link',
                                state: "admin-panel.default.g-grants-people",
                                 params: id,
                                priority: 7
                            };

                 var peopleActiveMenuItem =           
                 {
                    name: "Социально активные",
                    icon: "icon-person",
                    type: 'link',
                    state: "admin-panel.default.g-grants-people-active",
                    params: id,
                    priority: 8
                  };

                  var peopleSocialMenuItem =           
                  {
                    name: "Социально-нуждающиеся",
                    icon: "icon-person",
                    type: 'link',
                    state: "admin-panel.default.g-grants-people-social",
                    params: id,
                    priority: 9
                  };

                var statMenuItem = 
                {
                  name: "Статистика",
                  icon: "icon-info",
                  type: 'link',
                  state: "admin-panel.default.g-grants-stat",
                  params: id,
                  priority: 10

                };

                var reportMenu = 
                {
                  name: "Выгрузка",
                  icon: "icon-cloud-download",
                  type: 'link',
                  state: "admin-panel.default.g-grants-reports",
                  params: id,
                  priority: 11

                };
                    
                  if(!window.menu || window.menu.length == 0 || window.menu[0].name == "Профиль" || window.menu[menu.length-1].name == "Профиль"){

                        window.oldMenu = [];
                        while(menu.length>0){
                            oldMenu.push(menu.pop());
                        }

                        window.grantId = id;

                        menu.push(homeMenuItem);
                        menu.push(descriptionMenuItem);
                        menu.push(timeLineMenuItem);

                        var deferredGrantMenuFunc = function() {

                          if(menu.length <7)
                          {
                              
                              menu.push(stageMenuItem);
                              menu.push(adminsMenuItem);
                              menu.push(univMenuItem);

                              if(window.grantId < 28)
                              {
                                menu.push(peopleMenuItem);

                              } else{
                                menu.push(peopleActiveMenuItem);
                                menu.push(peopleSocialMenuItem);
                              }
                             

                              

                              if(e.isAdmin){
                                menu.push(statMenuItem);
                              }
                              
                              menu.push(reportMenu);
                          }

                          e.roleDeferredMenu = null;
                        }

                        var deferredGrantMenuFuncCurator = function(){

                          if(menu.length < 5)
                          {
                             // menu.push(univMenuItem);

                             if(window.grantId < 28)
                             {
                               menu.push(peopleMenuItem);
                             } else{
                               menu.push(peopleActiveMenuItem);
                               menu.push(peopleSocialMenuItem);
                             }
                            
                          }

                          e.deferredGrantMenuFuncCurator = null;

                        }

                        if(e.roleInfo){
                          if(e.isAdmin || e.isGrantAdmin){
                             deferredGrantMenuFunc();
                          } else {
                            if(e.isAnyCurator && e.deferredGrantMenuFuncCurator){
                              e.deferredGrantMenuFuncCurator();
                            }
                          }
                        } else
                        {
                           e.roleDeferredMenu = deferredGrantMenuFunc;
                           e.deferredGrantMenuFuncCurator = deferredGrantMenuFuncCurator;
                        }
                        
                  }

                }


                e.infoGrant = function(id){

                  $state.go('admin-panel.default.g-grants-description', {'param': id});

                }

                e.participateGrant = function(code){

                  $state.go('admin-panel.default.g-grants-participate', {'param': grantId, 'code': code});

                }

                e.showMoreGrant = function(id) {
                 
                }

                e.findColNumberPerRow = function() {
                  var nCols;
                  var width = $window.innerWidth;

                  if (width < 600) {
                      // xs
                      nCols = 1;
                  }
                  else if(width >= 768 && width < 1500) {
                       // sm and md
                       nCols = 2
                  } else if (width >= 1500) {
                      // lg
                      nCols = 3;
                  }
                  return nCols;
              }

              e.getCardWidth = function() {
                 
                 var cols = e.findColNumberPerRow();

                 if(cols==3){

                   return '32%';
                 }

                 return  '' + parseInt(100/cols) + '%';
              }

              e.CardSwitcher = function (index) {

                 var cols = e.findColNumberPerRow();

                 if(cols > 1)
                 {
                     return index % cols;
                 }

                 return 0;

              }

              e.grantImageClick = function() {
                 if(this.mouseActive)
                {
                       $("#grantImageFileInput").click();
                }

              }


              e.grantAttachClick = function() {
                 if(this.mouseActive)
                {
                   $("#GrantAttachementsFileUploader").click();
                 }
              }

               e.grantWinReportAttachClick = function() {
        
                   $("#winReportUploader").click();
                 
              }

               e.grantWinAdditionalReportAttachClick = function() {
                
                   $("#winAdditionalReportUploader").click();
                 
              }

              e.saveGrantAdmins = function() {

                e.updateGrant();
              }

              var getQuotaId   = function(universityId) {

                var result = 0;

                if(e.grantQuotas){
                   e.grantQuotas.forEach(function(item) {
                         if(item.universityId == universityId) {
                           result = item.id;
                         } 
                    });                 

                }



                return result;
              }


            var getQuota = function(universityId) {

                var result = 0;

                if(e.grantQuotas){

                  e.grantQuotas.forEach(function(item) {
                         if(item.universityId == universityId) {
                            result = item.quota;
                         } 
                    });

                }
                else
                {
                  result = '';
                }

                return result;
            }

            var getStudentCount = function(universityId) {

                var result = 0;

                 if(e.grantQuotas){

                  e.grantQuotas.forEach(function(item) {
                         if(item.universityId == universityId) {
                            result = item.StudentCount;
                         } 
                    });

                 }
                 else
                {
                  result = '';
                }

                return result;
            }

            var getWinnersCount = function(universityId) {

                var result = 0;

                if(e.grantQuotas){

                  e.grantQuotas.forEach(function(item) {
                         if(item.universityId == universityId) {
                            result = item.WinnerCount;
                         } 
                    });

                }
                else
                {
                  result = '';
                }

                return result;
            }

            var getReportLink = function(universityId) {

                var result = 0;

                if(e.grantQuotas){

                  e.grantQuotas.forEach(function(item) {
                         if(item.universityId == universityId) {
                    
                            result = item.Link ? '<a href="'+ item.Link +'" target="_blank" >Отчет</a>'  : '';
                         } 
                    });

                }
                else
                {
                  result = '';
                }

                return result;
            }


            e.changeAdditionalSelectedUniver = function(){

               e.loadAdditionalPeople(grantId, e.selectedUniver);
               e.loadUniversityQuotas(grantId, e.selectedUniver);
               e.loadUniversityFullQuotas(grantId, e.selectedUniver);
            }

            e.changeSelectedUniver = function(){

                e.grantPeople = [];

                e.showOnlyValid = false;

                 e.loadGrantPeople(grantId, e.selectedUniver);
                 e.loadUniversityQuotas(grantId, e.selectedUniver);
                 e.loadUniversityFullQuotas(grantId, e.selectedUniver);
            }


            e.loadGrantUnivers = function(grantId)
            {

               $http.get(API_CONFIG.serverPath +'api/grantstudent/getunivers/' + grantId )
                .success(function (data, status) {
                    // gProgress.unmask(e);
                     
                     e.univerList = data; 

                     if(e.univerList.length > 0){                         
                          if (window.univerId) {
                            e.setComboUniverId(parseInt(univerId));
                          } else {
                            e.selectedUniver = e.univerList[0].Id;
                          }
                     }                

                   // gProgress.unmask(e);

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });  

            }


            e.loadGrantAdditionalUnivers = function(grantId)
            {

               $http.get(API_CONFIG.serverPath +'api/grantstudent/getunivers/' + grantId + '?additional=true')
                .success(function (data, status) {
                     gProgress.unmask(e);

                     e.univerList = data; 

                     if(e.univerList.length > 0){
                      // e.selectedUniver = e.univerList[0].Id;
                     } 

                      e.selectedUniver = 0;
                      e.changeAdditionalSelectedUniver();             

                    gProgress.unmask(e);

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });  

            }

            e.loadGrantPeopleFilter =  function(){
              e.loadGrantPeople( e.peopleGrantId , e.peopleUniverId)
            }

            e.loadGrantPeople = function(grantId, univerId){

                //gProgress.mask(e);


                e.peopleGrantId = grantId;
                e.peopleUniverId = univerId;

                var filter = {
                  selectedRecordbook: e.selectedRecordbook,
                  selectedAchievements: e.selectedAchievements,
                  selectedScore : e.selectedScore,
                  selectedIncome : e.selectedIncome,
                  social:false,
                  active:false
              };

                var stateName = $state.current.name;

                if(stateName && stateName.indexOf('people-active')>0){
                  filter.active = true;
                }

                if(stateName && stateName.indexOf('people-social')>0){
                  filter.social = true;
                }

                $http.put(API_CONFIG.serverPath +'api/grantstudent/' + grantId + '/' + univerId, filter)
                .success(function (data, status) {
                     gProgress.unmask(e);

                     e.grantPeople = data.Data;

               /*     e.grantPeople.forEach(function(item) {
                          item.Link =  '<a href="/my#/user/page/' + item.StudentId +'">Просмотр</a>' ;
                          //item.Fio = '<a href="/my#/user/page/' + item.StudentId +'">' + item.Fio + '</a>' ;
                    }); */

                   
                   // gProgress.unmask(e);

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });  
            }

            e.getGrantPeopleReport = function()
            {
              gProgress.mask(e);

              $http.get(API_CONFIG.serverPath + 'api/grantstudent/getstudentsreport/' + grantId + '/' + e.selectedUniver)
                .success(function (data, status) {
                   gProgress.unmask(e);

                   var hiddenElement = document.createElement('a');

                    hiddenElement.href = data.path;
                    hiddenElement.download = data.fileName;
                    hiddenElement.click();

                  });
            }

            e.getGrantWinnersReport = function()
            {
              gProgress.mask(e);

              var onlyNewWinners = null;
              if(e.reportFilterOnlyNewWinners){
                 onlyNewWinners = true;
              }
              if(e.reportFilterOnlyOldWinners){
                onlyNewWinners = false;
              }

              $http.get(API_CONFIG.serverPath + 'api/grantstudent/getmainwinnersreport/' + grantId + '?onlyNewWinners=' + onlyNewWinners )
                .success(function (data, status) {
                   gProgress.unmask(e);

                   var hiddenElement = document.createElement('a');

                    hiddenElement.href = data.path;
                    hiddenElement.download = data.fileName;
                    hiddenElement.click();

                  });
            }

           e.getGrantWinnersOtherReport = function(activ)
            {
             
              gProgress.mask(e);

                 var onlyNewWinners = null;
              if(e.reportFilterOnlyNewWinners){
                 onlyNewWinners = true;
              }
              if(e.reportFilterOnlyOldWinners){
                onlyNewWinners = false;
              }
            

              $http.get(API_CONFIG.serverPath + 'api/grantstudent/getwinnersotherreport/' + grantId +'/' +activ  + '?onlyNewWinners=' + onlyNewWinners  )
                .success(function (data, status) {
                   gProgress.unmask(e);

                   var hiddenElement = document.createElement('a');

                    hiddenElement.href = data.path;
                    hiddenElement.download = data.fileName;
                    hiddenElement.click();

                  });
            }



             e.getAllAdditionalReport = function()
            {
              gProgress.mask(e);

                  var onlyNewWinners = null;
              if(e.reportFilterOnlyNewWinners){
                 onlyNewWinners = true;
              }
              if(e.reportFilterOnlyOldWinners){
                onlyNewWinners = false;
              }
             

              $http.get(API_CONFIG.serverPath + 'api/grantstudent/getalladditionalstudentsreport/' + grantId  + '?onlyNewWinners=' + onlyNewWinners   )
                .success(function (data, status) {
                   gProgress.unmask(e);

                   var hiddenElement = document.createElement('a');

                    hiddenElement.href = data.path;
                    hiddenElement.download = data.fileName;
                    hiddenElement.click();

                  });
            }


            e.getGrantWinnerList = function()
            {
              gProgress.mask(e);

                var onlyNewWinners = null;
              if(e.reportFilterOnlyNewWinners){
                 onlyNewWinners = true;
              }
              if(e.reportFilterOnlyOldWinners){
                onlyNewWinners = false;
              }

              $http.get(API_CONFIG.serverPath + 'api/grantstudent/getwinnerslist/' + grantId  + '?onlyNewWinners=' + onlyNewWinners )
                .success(function (data, status) {
                   gProgress.unmask(e);

                   var hiddenElement = document.createElement('a');

                    hiddenElement.href = data.path;
                    hiddenElement.download = data.fileName;
                    hiddenElement.click();

                  });
            }


            e.getGrantAddWinnersReport = function()
            {
              gProgress.mask(e);

               var onlyNewWinners = null;
              if(e.reportFilterOnlyNewWinners){
                 onlyNewWinners = true;
              }
              if(e.reportFilterOnlyOldWinners){
                onlyNewWinners = false;
              }

             

              $http.get(API_CONFIG.serverPath + 'api/grantstudent/getwinnersreport/' + grantId  + '?additional=true'  + '&onlyNewWinners=' + onlyNewWinners)
                .success(function (data, status) {
                   gProgress.unmask(e);

                   var hiddenElement = document.createElement('a');

                    hiddenElement.href = data.path;
                    hiddenElement.download = data.fileName;
                    hiddenElement.click();

                  }).error(function (error) {

                     gProgress.unmask(e);

                    $mdToast.show(
                      

                        $mdToast.simple()
                        .content("Ошибка при получении выгрузки")
                        .theme("error-toast")
                        .position('top'));
                  });  
            }

            e.getGrantPeopleDbfReport = function()
            {
              gProgress.mask(e);

              $http.get(API_CONFIG.serverPath + 'api/grantstudent/getstudentsdbfreport/' + grantId + '/' + e.selectedUniver)
                .success(function (data, status) {
                   gProgress.unmask(e);

                   var hiddenElement = document.createElement('a');

                    hiddenElement.href = data.path;
                    hiddenElement.download = data.fileName;
                    hiddenElement.click();

                  });
            }

            e.getGrantAdditionalPeopleReport = function()
            {
              gProgress.mask(e);

              $http.get(API_CONFIG.serverPath + 'api/grantstudent/getstudentsreport/' + grantId + '/' + e.selectedUniver + '?additional=true')
                .success(function (data, status) {
                   gProgress.unmask(e);

                   var hiddenElement = document.createElement('a');

                    hiddenElement.href = data.path;
                    hiddenElement.download = data.fileName;
                    hiddenElement.click();

                  });
            }

            e.loadUserStat =  function()
            {
              gProgress.mask(e);

              $http.get(API_CONFIG.serverPath + 'api/students/getuserstat/'+ grantId)
                .success(function (resp, status) {
                   gProgress.unmask(e);

                    e.userStat = resp.Data;

                  });
            }


            e.grantStat = {};

            e.loadGrantStat =  function()
            {
              gProgress.mask(e);

              $http.get(API_CONFIG.serverPath + 'api/grantstudent/getstat/' + grantId)
                .success(function (data, status) {
                   gProgress.unmask(e);

                    e.grantStat = data;

                  });
            }




           e.grantChart = {
                            labels: [],
                            series: ['Регистрация в конкурсе', 'Участвующие повторно', 'Победители прошлых конкурсов' ],
                            options: {
                                datasetFill: false,
                                responsive: true
                            },
                            data: []
                        };

            e.loadGrantChart = function(){


                     $http.get(API_CONFIG.serverPath + 'api/grant/getregchart/' + grantId)
                        .success(function (data, status) {
                           gProgress.unmask(e);

                              e.grantChart.labels = data.Labels;
                              e.grantChart.data.push(data.RowReg);
                              e.grantChart.data.push(data.RowRepeatReg);
                              e.grantChart.data.push(data.RowRepeatWin);
                          });        
            }

            e.addWinnerReport = function(grantId, report)
            {
                gProgress.mask(e);

                 $http.get(API_CONFIG.serverPath +'api/addWinnerReport/' + grantId + '/' + e.selectedUniver  + '/' + report)
                .success(function (data, status) {
                     gProgress.unmask(e);

                   // e.grantPeople = data;
                   e.WinnerReport = report;
                   e.WinnerReportFile = data.WinnerReportFile;

                   $mdToast.show(
                          $mdToast.simple()
                          .content('Отчет о победителях успешно загружен')
                          .position('top'));

                  }).error(function (error) {

                     gProgress.unmask(e);

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.Message)
                        .theme("error-toast")
                        .position('top'));
                  });  


            }            

            e.addAdditionalWinnerReport = function(grantId, report)
            {
                gProgress.mask(e);

                 $http.get(API_CONFIG.serverPath +'api/addAdditionalWinnerReport/' + grantId + '/' + e.selectedUniver  + '/' + report)
                .success(function (data, status) {
                     gProgress.unmask(e);

                   // e.grantPeople = data;
                   e.AdditionalWinnerReport = report;
                   e.AdditionalWinnerReportFile = data.AdditionalWinnerReportFile;

                   $mdToast.show(
                          $mdToast.simple()
                          .content('Отчет о дополнительных победителях успешно загружен')
                          .position('top'));

                    gProgress.unmask(e);

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });  


            }

            e.deleteWinnerReport = function()
            {
                gProgress.mask(e);

                 $http.get(API_CONFIG.serverPath +'api/grantquota/deletewinnerreport/' + grantId + '/' + e.selectedUniver)
                .success(function (data, status) {
                     gProgress.unmask(e);

                   // e.grantPeople = data;
                   e.WinnerReport = null;
                   e.WinnerReportFile = null;
                    e.winReportUploader.clearQueue();

                   $mdToast.show(
                          $mdToast.simple()
                          .content('Отчет о победителях успешно удален')
                          .position('top'));


                  }).error(function (error) {

                      gProgress.unmask(e);

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.Message)
                        .theme("error-toast")
                        .position('top'));
                  });  

            }

            e.deleteAdditionalWinnerReport = function()
            {
                gProgress.mask(e);

                 $http.get(API_CONFIG.serverPath +'api/grantquota/deleteadditionalwinnerreport/' + grantId + '/' + e.selectedUniver)
                .success(function (data, status) {
                     gProgress.unmask(e);

                   e.AdditionalWinnerReport = null;
                   e.AdditionalWinnerReportFile = null;
                    e.winAdditionalReportUploader.clearQueue();

                   $mdToast.show(
                          $mdToast.simple()
                          .content('Отчет о дополнительных победителях успешно удален')
                          .position('top'));

                    gProgress.unmask(e);

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });  


            }

            e.downloadWinnerReportFile = function() {
              if (!!e.WinnerReportFile) {
                var hiddenElement = document.createElement('a');

                    hiddenElement.href = e.WinnerReportFile.VirtualPath
                    hiddenElement.download = e.WinnerReportFile.FullName;
                    hiddenElement.click();
              }
            }

            e.downloadAdditionalWinnerReportFile = function() {
              if (!!e.AdditionalWinnerReportFile) {
                var hiddenElement = document.createElement('a');

                    hiddenElement.href = e.AdditionalWinnerReportFile.VirtualPath
                    hiddenElement.download = e.AdditionalWinnerReportFile.FullName;
                    hiddenElement.click();
              }
            }

           e.loadAdditionalPeople = function(grantId, univerId){

                gProgress.mask(e);

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
                    
                    GrantId: grantId,
                    UniversityId : univerId,
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


                $http.put(API_CONFIG.serverPath +'api/grantstudent/additional/', searchStudent)
                .success(function (data, status) {
                     gProgress.unmask(e);

                     if(localQueryNum < e.UsersQuery ){
                        return;
                     }

                     e.grantAdditionalPeople = data.Data;

                    e.grantAdditionalPeople.forEach(function(item) {
                          item.Fio = '<a href="/my#/user/page/' + item.StudentId +'">' + item.Fio + '</a>' ;
                    });

                    gProgress.unmask(e);

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });

            }


             e.loadMoreAdditionalPeople = function() {

              if(e.isLoadingMoreEvents){
                return;
              }

              if(e.noMoreEvents){
                return;
              }

              e.isLoadingMoreEvents = true;


               gProgress.mask(e);


               var lastId = 0;
               var skip = 0;

               if(e.grantAdditionalPeople && e.grantAdditionalPeople.length > 0){

                 lastId = e.grantAdditionalPeople[e.grantAdditionalPeople.length-1].id;

                 skip = e.grantAdditionalPeople.length;
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
                    
                    GrantId: grantId,
                    UniversityId :  e.selectedUniver,
                    lastId : lastId,
                    sortBy : sortBy,
                    skip : skip,
                    Asc : asc
                };
               

             $http.put(API_CONFIG.serverPath +'api/grantstudent/additional/', searchStudent)
                .success(function (data, status) {
                     gProgress.unmask(e);

                    var result = [];

                    var arr = data.Data;

                    arr.forEach(function(item) {
                          item.Fio = '<a href="/my#/user/page/' + item.StudentId +'">' + item.Fio + '</a>' ;
                           e.grantAdditionalPeople.push(item);
                    });

                    if(arr.length == 0){
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

               

               
            };


             e.AdditionalWinnersScroll =  function() {

                        

                          $("#admin-panel-content-view").parent().parent().bind('scroll', function() {

                           var raw = this;

                            if (raw.scrollTop + raw.offsetHeight +200 >= raw.scrollHeight) {

                                      e.loadMoreAdditionalPeople();
                                }      
                          });

                         $(".elements-image-table-example thead tr th").click(function() {

                                  e.grantAdditionalPeople = [];
                                    setTimeout(function (){

                                         e.loadAdditionalPeople(grantId, e.selectedUniver);
                                    

                                    }, 500);

                            }); 

                    };


            e.reloadUniversityQuotas = function(){

              if(e.grantQuotas &&  e.UniversityList) {

                    var data =  e.UniversityList;
                    var result =  [];
                    for (var i = 0; i < data.length; i++) {

                        result.push({
                            id: getQuotaId(data[i].Id),
                            universityId : data[i].Id,
                            grantId: grantId,
                            univerlink : data[i].Name,
                            name: data[i].Name,
                            quota : getQuota(data[i].Id),
                            thumb: data[i].thumb,
                            city: data[i].City,
                            town: data[i].Town,
                            univerType: data[i].UniverType == 1 ? 'ВО' : 'СПО',
                            studentCount: getStudentCount(data[i].Id),
                            winnerCount: getWinnersCount(data[i].Id),
                            RegisteredCount : data[i].RegisteredCount,
                            ReportLink : getReportLink(data[i].Id),
                            link: '<a href="/my#/grant/people/' + grantId + '/' + data[i].Id +'">Просмотр</a>'
                        });
                    }


                    if (result.length > 0) {
                        window.universityQuotasCash = result;
                        e.universityQuotas = result;
                       // gProgress.unmask(e);
                    }

                  }

              }


              e.filterGrantPeople = function(){


                if(e.showOnlyValid){

                     e.oldGrantPeople = e.grantPeople;
                    var filtered = [];

                    for (var i = 0; i < e.grantPeople.length; i++) {

                        var a = e.grantPeople[i];

                        if( a.IsStudentBookValid) //a.IsPassportValid &&
                        {
                            filtered.push(a);
                        }

                    }

                    e.grantPeople = filtered;
              } else {
                   if(e.oldGrantPeople){
                      e.loadGrantPeople(grantId, e.selectedUniver);
                   }
              }


          }

 
              e.loadUniversities = function(){

                if( e.UniversityList ){
                  return;
                }


                  $http.get(API_CONFIG.serverPath +'api/university')
                .success(function (data, status) {
                     gProgress.unmask(e);

                     e.UniversityList = data;
                     e.reloadUniversityQuotas();

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
              }

              e.saveGrantQuotas = function(){

                var updQuotas = [];

                 e.universityQuotas.forEach(function(item) {
                       if(item.dirty) {
                           updQuotas.push(item);
                       } 
                  });

                 $http.put(API_CONFIG.serverPath + "api/grantquota/"+ grantId,  updQuotas).success(function (data, status) {
                       gProgress.unmask(e);

                       $mdToast.show(
                          $mdToast.simple()
                          .content('Квоты успешно обновлены')
                          .position('top'));

                      // e.loadGrantQuotas(grantId);

                    }).error(function (error) {
                      $mdToast.show(
                          $mdToast.simple()
                          .content(error.data.Message)
                          .theme("error-toast")
                          .position('top'));
                    });
             }             


             e.editQuota = function(univer, quota)
             {
                var validatedQuota = quota;

                e.universityQuotas.forEach(function(item, index) {
                     if(item.name == univer) {

                      var tQuotaSum = e.quotaSum - e.universityQuotas[index].quota + quota;

                      if(e.grant.fullQuota) {} else{

                         $mdToast.show(
                          $mdToast.simple()
                          .content('Квота гранта не задана')
                          .theme("error-toast")
                          .position('top'));

                        validatedQuota = e.universityQuotas[index].quota;

                      }


                      if (!!e.grant.fullQuota && e.grant.fullQuota - tQuotaSum < 0) {
                        $mdToast.show(
                          $mdToast.simple()
                          .content('Квота гранта исчерпана')
                          .theme("error-toast")
                          .position('top'));

                        validatedQuota = e.universityQuotas[index].quota;
                      } else {
                          e.quotaSum = tQuotaSum;
                         e.universityQuotas[index].quota = quota;
                         e.universityQuotas[index].dirty = true;
                       } 
                     }
                });

                return validatedQuota;
             }

             e.makeEditableGrid = function (){

              var t = e;

                var quotacolumns = [];

                $('tfoot').hide();

                $("md-table tr").each(function(index, item) {

                  if(index > 0){

                    var cols = $(item).find('td');
                    var quotaCol = cols[5];

                    if(quotaCol){

                      var univer = $(cols[4]).text(); 

                      if(e.isAdmin || e.isGrantAdmin){

                      quotaCol.contentEditable = true;

                            $(quotaCol).on('blur',
                               function () {

                                var quota = parseInt($(this).text());

                                if(quota>0) {} else {
                                  quota = 0;
                                }

                                $(this).text(t.editQuota(univer, quota));
                           });


                           $(quotaCol).keypress(function(e) {
                                if(e.which == 13) {
                                    $(this).blur();
                                }
                            });

                       }

                    }
                    
                  }

                });

              }

          e.setGrantWinner = function(grantId, studentId) {

              $http.get(API_CONFIG.serverPath +'api/grantstudent/winner/' + grantId + '/' + studentId)
                .success(function (data, status) {
                     gProgress.unmask(e);

                    $mdToast.show(
                        $mdToast.simple()
                        .content('Победитель успешно выбран')
                        .position('top'));


                  }).error(function (error) {


                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.Message)
                        .theme("error-toast")
                        .position('top')).then(function(){
                            window.location.reload();
                        });



                  });
             }

           e.setAdditionalWinner = function(grantId, studentId) {

              $http.get(API_CONFIG.serverPath +'api/grantstudent/additionalwinner/' + grantId + '/' + studentId)
                .success(function (data, status) {
                     gProgress.unmask(e);

                    $mdToast.show(
                        $mdToast.simple()
                        .content(data)
                        .position('top'));

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.Message)
                        .theme("error-toast")
                        .position('top')).then(function(){
                            window.location.reload();
                        });;
                  });
             }

            e.cancelWinner = function(grantId, studentId) {



              $http.get(API_CONFIG.serverPath +'api/grantstudent/cancelwinner/' + grantId + '/' + studentId)
                .success(function (data, status) {
                     gProgress.unmask(e);

                    $mdToast.show(
                        $mdToast.simple()
                        .content('Победитель успешно отменен')
                        .position('top'));

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
             }

              e.cancelAdditionalWinner = function(grantId, studentId) {

              $http.get(API_CONFIG.serverPath +'api/grantstudent/canceladditionalwinner/' + grantId + '/' + studentId)
                .success(function (data, status) {
                     gProgress.unmask(e);

                    $mdToast.show(
                        $mdToast.simple()
                        .content('Дополнительный победитель успешно отменен')
                        .position('top'));

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
             }

             e.chooseWinner = function(val, id){



             if(e.isAdmin && !val) {} else
             {
               if(e.WinnerReport) {

                          $mdToast.show(
                          $mdToast.simple()
                          .content('Нельзя выбрать победителя так как прикреплен отчет о победителях')
                          .theme("error-toast")
                          .position('top'));

                       return false;

                    }
                }



             if(e.isAdmin && !val) {} else
             {

                 if(e.grant && e.grant.stages && (e.grant.stages.WinnersSelection == false || e.grant.stages.WinnersSelection == null)  && e.isAdmin != true){

                            $mdToast.show(
                            $mdToast.simple()
                            .content('Нельзя менять победителей, так как этап выбора победителей неактивен')
                            .theme("error-toast")
                            .position('top'));

                         return false;

                      } 
                 }     


                 if(val) {
                  
                  if(e.univerQuotaSum <= 0){

                     $mdToast.show(
                        $mdToast.simple()
                        .content('Квота Вашего ВУЗа исчерпана')
                        .theme("error-toast")
                        .position('top'));

                     return false;

                  }

                    e.setGrantWinner(grantId, id);
                    e.univerQuotaSum -= 1;
                   
                 } else {
                    e.univerQuotaSum += 1;
                    e.cancelWinner(grantId, id);
                 }

                 return true;

              }

              e.chooseAdditionalWinner = function(val, id){

                if(e.AdditionalWinnerReport) {

                        $mdToast.show(
                        $mdToast.simple()
                        .content('Нельзя выбрать победителя так как прикреплен отчет о дополнительных победителях')
                        .theme("error-toast")
                        .position('top'));

                     return false;

                  }

                 if(val) {

                  //if(e.univerQuotaSum == 0){

                    // $mdToast.show(
                      //  $mdToast.simple()
                        //.content('Квота Вашего ВУЗа исчерпана')
                        //.theme("error-toast")
                        //.position('top'));

                     //return false;

                  //}

                    e.univerQuotaSum -= 1;
                    e.setAdditionalWinner(grantId, id);
                 } else {
                    e.univerQuotaSum += 1;
                    e.cancelAdditionalWinner(grantId, id);
                 }

                 return true;

              }


              e.makeCheckboxGrid = function(){

                var stateName = $state.current.name;

                if(stateName && stateName.indexOf('people-social')>0){
                  $("md-table").show();
                  return;
                }

                var t = e;

                var quotacolumns = [];



                  $("md-table tr").each(function(index, item) {

                    if(index > 0){

                      var cols = $(item).find('td');

                      var winnerVal = $(cols[1]).text();
                      var bookValid = $(cols[4]).text(); //paspValid
                      var hasValidAch = $(cols[5]).text(); 
                      var hasInvalidAch = $(cols[6]).text(); 
                      var hasNotCheckedAch = $(cols[7]).text(); 
                     // var bookValid = $(cols[5]).text();

                      if(winnerVal.length > 0) 
                      {

                      $(cols[1]).html('<input type="checkbox"/>');
                      $(cols[4]).html('<input type="checkbox"/>');
                      $(cols[5]).html('<input type="checkbox"/>');
                      $(cols[6]).html('<input type="checkbox"/>');
                      $(cols[7]).html('<input type="checkbox"/>');
                      //$(cols[5]).html('<input type="checkbox"/>');

                      $(cols[1]).find('input').prop('checked', (winnerVal == 'true'));
                      $(cols[4]).find('input').prop('checked', (bookValid == 'true'));  //paspValid
                      $(cols[5]).find('input').prop('checked', (hasValidAch == 'true'));
                      $(cols[6]).find('input').prop('checked', (hasInvalidAch == 'true')); 
                      $(cols[7]).find('input').prop('checked', (hasNotCheckedAch == 'true')); 
                      //$(cols[5]).find('input').prop('checked', (bookValid == 'true'));


                    
                      //if(e.grant && e.grant.stages && e.grant.stages.WinnersSelection == false)
                     // {
                          $(cols[1]).find('input').attr('readonly',true);
                   //   }
                     

                      $(cols[4]).find('input').attr('readonly',true);
                      $(cols[5]).find('input').attr('readonly',true);
                      $(cols[6]).find('input').attr('readonly',true);
                      $(cols[7]).find('input').attr('readonly',true);

                     // $(cols[5]).find('input').attr('readonly',true);

                      $(cols[4]).find('input').attr('disabled','disabled');
                      $(cols[5]).find('input').attr('disabled','disabled');
                      $(cols[6]).find('input').attr('disabled','disabled');
                      $(cols[7]).find('input').attr('disabled','disabled');
                     // $(cols[5]).find('input').attr('disabled','disabled');

                      var curId = $(cols[8]).text();
                      $(cols[8]).hide();


                       if(bookValid == 'true'){ //  && paspValid == 'true'  #todopasp

                                $($(cols[1]).find('input')).click(function() {

                                    var result =  t.chooseWinner($(this).is(":checked"), curId);

                                    if(!result)
                                    {
                                      $(this).prop('checked', ! $(this).is(":checked"));
                                    }     
                                });

                            } else {

                              $($(cols[1]).find('input')).click(function() {

                                 $mdToast.show(
                                  $mdToast.simple()
                                  .content("Нельзя выбрать данного участника победителем до валидации его данных")
                                  .theme("error-toast")
                                  .position('top'));

                                  $(this).prop('checked', ! $(this).is(":checked"));


                                });
                            }

                      }
                    }
                  });

                 gProgress.unmask(e);

                if(stateName && stateName.indexOf('people-active')>0){
                  $(".IsWinner-cell").hide();
                  $($("th")[1]).hide();
                }
                 $("md-table").show();

              }

               e.makeAdditionalCheckboxGrid = function(){


                var t = e;

                var quotacolumns = [];

                  $("md-table tr").each(function(index, item) {

                    if(index > 0){

                      var cols = $(item).find('td');

                      var winnerVal = $(cols[1]).text();
                      var bookValid = $(cols[4]).text(); // paspValid
                     // var bookValid = $(cols[5]).text();

                      if(winnerVal.length > 0) {

                      $(cols[1]).html('<input type="checkbox"/>');
                      $(cols[4]).html('<input type="checkbox"/>');
                     // $(cols[5]).html('<input type="checkbox"/>');

                      $(cols[1]).find('input').prop('checked', (winnerVal == 'true'));
                      $(cols[4]).find('input').prop('checked', (bookValid == 'true')); //paspValid
                    //  $(cols[5]).find('input').prop('checked', (bookValid == 'true'));


                      $(cols[4]).find('input').attr('readonly',true);
                   //   $(cols[5]).find('input').attr('readonly',true);

                      $(cols[4]).find('input').attr('disabled','disabled');
                    //  $(cols[5]).find('input').attr('disabled','disabled');

                      var curId = $(cols[5]).text();
                      $(cols[5]).hide();


                      if(bookValid == 'true'){ //  && paspValid == 'true'

                            $($(cols[1]).find('input')).click(function() {

                                var result =  t.chooseAdditionalWinner($(this).is(":checked"), curId);

                                if(!result)
                                {
                                  $(this).prop('checked', false);
                                }     
                            });

                      } else {

                              $($(cols[1]).find('input')).click(function() {

                                 $mdToast.show(
                                  $mdToast.simple()
                                  .content("Нельзя выбрать данного участника дополнительным победителем до валидации его данных")
                                  .theme("error-toast")
                                  .position('top'));

                                  $(this).prop('checked', ! $(this).is(":checked"));


                                });
                            }

                      }
                    }
                  });
              }

           e.loadUniversityQuotas = function(grantId, univerId) {

                gProgress.mask(e);

               $http.get(API_CONFIG.serverPath +'api/grantquota/getuniversityquota/' + grantId + '/' + univerId)
                .success(function (data, status) {
                    // gProgress.unmask(e);

                    e.univerQuotaSum = data.Quota;

                    e.WinnerReport = data.WinnerReport;
                    e.WinnerReportFile = data.WinnerReportFile;
                    e.AdditionalWinnerReport = data.AdditionalWinnerReport;
                    e.AdditionalWinnerReportFile = data.AdditionalWinnerReportFile;

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });

                  
            }

             e.loadUniversityFullQuotas = function(grantId, univerId) {

                gProgress.mask(e);

               $http.get(API_CONFIG.serverPath +'api/grantquota/getuniversityfullquota/' + grantId + '/' + univerId)
                .success(function (data, status) {
                    // gProgress.unmask(e);

                    e.univerFullQuotaSum = data.Quota;

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });

                  
            }

              e.loadGrantQuotas = function(grantId, universityId) {

                if(  e.grantQuotas){
                  return;
                }

              gProgress.mask(e);

               $http.get(API_CONFIG.serverPath +'api/grantquota/' + grantId)
                .success(function (data, status) {
                     gProgress.unmask(e);

                    e.quotaSum = 0;

                    var result = [];
                    for (var i = 0; i < data.length; i++) {


                    e.quotaSum += data[i].Quota;

                        result.push({
                            id : data[i].Id,
                            universityId : data[i].UniversityId,
                            quota : data[i].Quota,
                            grantId : data[i].GrantId,
                            StudentCount : data[i].StudentCount,
                            WinnerCount : data[i].WinnerCount,
                            Link : data[i].Link
                        });
                    }


                    e.grantQuotas = result;
                    gProgress.unmask(e);

                   e.reloadUniversityQuotas();

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });

                  
            }

            e.goCreateGrant  = function(){
                    $state.go('admin-panel.default.g-grants-create');
            }


            e.goProfile = function(){
                    e.returnMenu();

                    if(e.isInfoFilled == false){
                      $state.go('admin-panel.default.g-user-profile', {'tab': 0});
                      return;
                    }

                    if(e.ispassportFilled == false){
                      $state.go('admin-panel.default.g-user-profile', {'tab': 1});
                      return;
                    }

                    if(e.isrecordbookFilled == false)
                    {
                      $state.go('admin-panel.default.g-user-profile', {'tab': 2});
                      return;
                    }

                    if(e.isIncomeFilled == false && e.participantCode == "h"){
                      $state.go('admin-panel.default.g-user-profile', {'tab': 3});
                      return;
                    }

                    if(e.isachievementFilled == false && e.participantCode == "a"){
                      $state.go('admin-panel.default.g-user-profile', {'tab': 4});
                      return;
                    }

                   


                    $state.go('admin-panel.default.g-user-profile');
                    return;

            }

            e.applyTimelineUserFilter = function() {






            }

            e.goListGrant = function() {
               $state.go('admin-panel.default.g-grants-list');
            }

            e.changeStage = function(stage, $event)
            {
                var stageName, stageCode, isStageOn, stageFunc;
                
                switch (stage) {

                    case "registration":
                    
                    stageCode = "Registration";
                    stageName = "регистрации участников";
                    if(e.grant.stages.Registration)
                    {
                        isStageOn = false;
                        stageFunc = e.closeRegistrationGrant; 

                    } else { 
                        isStageOn = true;
                        stageFunc = e.startGrant;
                    }

                    break

                    case "winnerselection" :
                    
                    stageCode = "WinnersSelection";
                    stageName = "выбора основных победителей";
                    if(e.grant.stages.WinnersSelection)
                    {
                        isStageOn = false;
                        stageFunc = e.closeWinnerSelectionGrant;

                    } else { 
                        isStageOn = true;
                        stageFunc = e.openWinnerSelectionGrant;
                    }

                    break;

                    case "additionalselection":
                    
                    stageCode = "AddtitionalSelection";
                    stageName = "выбора дополнительных победителей";
                    if(e.grant.stages.AddtitionalSelection)
                    {
                        isStageOn = false;
                        stageFunc = e.closeAdditionalSelectionGrant;

                    } else { 
                        isStageOn = true;
                        stageFunc = e.openAdditionalSelectionGrant;
                    }

                    break;

                    case "draft" :
                    
                    stageCode = "Draft";
                    stageName = "черновика";
                    if(e.grant.stages.Draft)
                    {
                        isStageOn = false;
                        
                    } else { 
                        isStageOn = true;
                        stageFunc = e.makeDraftGrant;

                    }

                    break;

                    case "delivery":
                    
                    stageCode = "Delivery";
                    stageName = "выдачи гранта победителям";
                    if(e.grant.stages.Delivery)
                    {
                        isStageOn = false;
                        stageFunc = e.cancelDeliveryGrant;

                    } else { 
                        isStageOn = true;
                        stageFunc = e.openDeliveryGrant;
                    }

                    break;

                    case "finalgrant":
                    
                    stageCode = "Final";
                    stageName = "завершения конкурса";
                    if(e.grant.stages.Final)
                    {
                        isStageOn = false;
                        stageFunc = e.cancelFinalGrant;

                    } else { 
                        isStageOn = true;
                        stageFunc = e.openFinalGrant;
                    }

                    break;
                }
                
                var message = 'Вы действительно хотите ' + 
                    (isStageOn ? 'запустить ' : 'завершить ') +
                    'этап ' + stageName;
                
                $mdDialog.show(
                    $mdDialog.confirm()
                    .title('Измение этапа')
                    .content(message)
                    .ok("Да")
                    .cancel("Отмена"))
                    .then(function() {
                        if(stageFunc){
                            stageFunc();
                        } 
                    }, function() {
                        e.grant.stages[stageCode] = !isStageOn;
                    });
            }

              e.ExpiresDataChanged = function(a,b,c) {



                
                if(e.grant.expiresDateText) {

                   
                    if(e.grant.expiresDateText.length<4){

                         var index = e.grant.expiresDateText.indexOf('.');


                        if(e.grant.expiresDateText.length == 2 && index==1){

                            e.grant.expiresDateText = e.grant.expiresDateText.substr(0,1);

                        } else if(e.grant.expiresDateText.length == 2 && index<0){

                            e.grant.expiresDateText += '.';

                        } else if(e.grant.expiresDateText.length > 2 && index<0){

                            e.grant.expiresDateText = e.grant.expiresDateText.substr(0,2) + '.';

                        } else if(e.grant.expiresDateText.length > 2 && index!=2){

                            var b = e.grant.expiresDateText.replace('.','');
                            if(b.length==2) {
                                e.grant.expiresDateText = b + '.';
                            } else {
                                e.grant.expiresDateText = b;
                            }
                        }

                          e.expiresDateInvalid = true;
                    } 
                    else if(e.grant.expiresDateText.length>4 && e.grant.expiresDateText.length<7){
                       
                        var len = e.grant.expiresDateText.replace('.','').length;

                         var index = e.grant.expiresDateText.replace('.','').indexOf('.');

                         if(e.grant.expiresDateText.length == 5 && index==3){
                            e.grant.expiresDateText = e.grant.expiresDateText.substr(0,4);
                         }
                         else if(e.grant.expiresDateText.length == 5 && index<0 && len == 4){
                            e.grant.expiresDateText += '.';
                        } 
                        else if(e.grant.expiresDateText.length > 4 && index!= 4){

                            e.grant.expiresDateText = '';
                        }

                          e.expiresDateInvalid = true;

                     }  else if (e.grant.expiresDateText.length >= 7 && e.grant.expiresDateText.length < 10){

                        e.expiresDateInvalid = true;


                     }  else if (e.grant.expiresDateText.length == 10){

                        var from = e.grant.expiresDateText.split(".");
                        var dt = new Date(from[2], from[1] - 1, from[0], 20);

                        if(dt.valueOf() > 0 || dt.valueOf() < 0) {

                            var now = new Date();

                            if(dt < now){
                               e.expiresDateInvalid = true;
                            }
                            else{
                                 e.expiresDateInvalid = false;
                                  e.grant.expiresDate = dt;
                            }

                        } else{
                            e.expiresDateInvalid = true;
                        }

                     } else if (e.grant.expiresDateText.length > 10){

                        e.grant.expiresDateText = e.grant.expiresDateText.substr(0,10);

                     }
                }
            }

            

            e.loadRole = function () {


              $http.get(API_CONFIG.serverPath + "api/students/getroleinfo")
                .then(function(resp){
                    e.roleInfo = resp.data;


                    if(e.roleInfo) {
                       e.isAdmin = (e.roleInfo.Role == 5);
                    } else {
                      return
                    }

                    if(e.roleInfo.UniversCurator.length >0 ) {
                       e.isAnyCurator = true;
                    }

                    if(e.grant && e.roleInfo.GrantsAdmin.indexOf(e.grant.id) >= 0){
                        e.isGrantAdmin = true;
                    }

                     if(e.roleInfo && e.deferredIsParticipant) {

                      e.deferredIsParticipant();
                          
                    }
                     

                    if(e.roleDeferredMenu){
                      if(e.roleInfo){
                          if(e.isAdmin || e.isGrantAdmin){

                              e.roleDeferredMenu();

                              if(e.deferredAdditionalMenuFunc)
                              {
                                 e.deferredAdditionalMenuFunc();
                              }

                          } else {

                            if(e.isAnyCurator &&  e.deferredGrantMenuFuncCurator){
                               e.deferredGrantMenuFuncCurator();

                            }

                          }
                        }
                    }

                    e.checkCurPageAccess();


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


            e.loadIsPassportFilled = function()
            {
               $http.get(API_CONFIG.serverPath +'api/personalInfo/ispassportFilled/'+ 0)
                    .success(function (data, status) {

                        e.ispassportFilled = data;

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
            }

            e.loadIsRecordBookFilled = function()
            {
               $http.get(API_CONFIG.serverPath +'api/students/isrecordbookFilled/'+ 0)
                    .success(function (data, status) {

                        e.isrecordbookFilled = data;

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
            }

            e.loadIsIncomeFilled = function()
            {
               $http.get(API_CONFIG.serverPath +'api/students/isincomeFilled/'+ 0)
                    .success(function (data, status) {

                        e.isIncomeFilled = data;

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
            }

            e.loadStudentInfo = function()
            {
                $http.get(API_CONFIG.serverPath +'api/Students/'+ 1)
                    .success(function (data, status) {


                        e.userInfo = data;
                        e.loadIsPassportFilled();
                        e.loadIsAchievementFilled();
                        e.loadIsRecordBookFilled();
                        e.loadIsIncomeFilled();

                        if(e.userInfo.Name && e.userInfo.LastName && e.userInfo.Patronymic && e.userInfo.UniversityId) {
                           e.isInfoFilled = true;
                        }
                        else
                        {
                           e.isInfoFilled = false;
                        }
                        

                  
                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
            }

            e.setPageTitle = function(title) {

                var pagetitle = $('.admin-toolbar .breadcrumb');
                $(pagetitle).each(function(index, item) { $(item).text(title); }  );
            }
            
            e.setComboUniverId = function(univerId) {
                e.selectedUniver = univerId;
            }

            e.checkCurPageAccess = function(){

              var stateName = $state.current.name;

              if(e.isAdmin || e.isGrantAdmin)
              {
                return;
              }

              if(e.isAnyCurator)
              {

                if(stateName == 'admin-panel.default.g-grants-unv' || 
                   stateName == 'admin-panel.default.g-grants-people' || 
                   stateName == 'admin-panel.default.g-grants-people-active' || 
                   stateName == 'admin-panel.default.g-grants-people-social' || 
                   stateName == 'admin-panel.default.g-grants-additional'){

                  return;

              }


              }

              if(stateName == 'admin-panel.default.g-grants-list' || 
                 stateName == 'admin-panel.default.g-grants-participate' || 
                 stateName == 'admin-panel.default.g-grants-timeline' || 
                 stateName == 'admin-panel.default.g-grants-description'){

                return;
              }

               $mdToast.show(
                  $mdToast.simple()
                  .content('У Вас нет доступа к данному разделу')
                  .theme("error-toast")
                  .position('top'));

               $state.go('admin-panel.default.g-grants-list');
            }

            e.loadMore = function() {

              if(e.isLoadingMoreEvents){
                return;
              }

              if(e.noMoreEvents){
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
               

                $http.put(API_CONFIG.serverPath +'api/grantevent/' + grantId, eventFilter)
                .success(function (data, status) {
                     gProgress.unmask(e);

                    var result = [];

                     for (var i = 0; i < data.length; i++) {

                        e.events.push({
                            id: data[i].Id,
                            title: data[i].Title,
                            subtitle: data[i].Subtitle,
                            image: data[i].Image,
                            name: data[i].Name,
                            content: data[i].Content,
                            palette: data[i].Palette,
                            date: new Date(data[i].EventDate).toLocaleDateString(),
                            time: data[i].EventDate.replace('T',' ').substr(11,8).replace('.',''),
                            description: data[i].Description,
                            conditions: data[i].Conditions,
                            dateChange: data[i].DateChange,
                            quotaChanged: data[i].QuotaChanged,
                            administrators: data[i].ChangeAdmin,
                            link: '/#/user/page/' + data[i].StudentId  
                        });
                    }

                    if(data.length == 0){
                        e.noMoreEvents = true;
                    }

                   gProgress.unmask(e);
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


            e.grantTimelineScroll =  function() {

           

              $("#admin-panel-content-view").parent().parent().bind('scroll', function() {

               var raw = this;

                if (raw.scrollTop + raw.offsetHeight + 200 >= raw.scrollHeight) {
                          e.loadMore();
                    }      
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
                e.noMoreEvents = false;

                e.loadEvents();
            }

            e.scrollToTop = function(){
               
               $('#admin-panel-content-view').parent().parent().animate({ scrollTop: 0 }, "slow");
            }



            e.reportFilterOnlyOldWinnersChanged = function(){

                if(e.reportFilterOnlyOldWinners){
                    e.reportFilterOnlyNewWinners = false;
                }
            }

            e.reportFilterOnlyNewWinnersChanged = function(){

                 if(e.reportFilterOnlyNewWinners){
                    e.reportFilterOnlyOldWinners = false;
                }
            }


            var stateName = $state.current.name;

            if($stateParams.param){
                window.grantId = $stateParams.param;
            }
            
            if($stateParams.univerId){
                window.univerId = $stateParams.univerId;
            } else {
                delete window.univerId;
            }

            switch (stateName) {

                      case "admin-panel.default.g-grants-create":

                        if($stateParams.param)
                        {
                          e.loadGrant($stateParams.param);
                          e.loadRole();
                        }
                        else
                        {
                          e.loadRole();
                        }

                        break

                      case "admin-panel.default.g-grants-stage":

                        if(window.grantId)
                        {
                          e.loadRole();
                          e.loadGrant(grantId);
                          e.grantMenu(grantId);
                        }
                        else
                        {
                           $state.go('admin-panel.default.g-grants-list');
                        }
                        break

                      case "admin-panel.default.g-grants-admins":

                        if(window.grantId)
                        {
                          e.loadRole();
                          e.loadGrant(grantId);
                          e.grantMenu(grantId);
                          e.loadSelectedAdminUsers(grantId);
                        }
                        else
                        {
                           $state.go('admin-panel.default.g-grants-list');
                        }
                        break

                        case "admin-panel.default.g-grants-description":

                          if(window.grantId)
                          {
                            e.loadRole();
                            e.loadIsParticipant(window.grantId);
                            e.loadGrant(grantId);
                            e.loadEvents();
                            e.grantMenu(grantId);
                          }
                          else
                          {
                             $state.go('admin-panel.default.g-grants-list');
                          }
                          break

                       case "admin-panel.default.g-grants-timeline":

                        if(window.grantId)
                        {
                          e.loadRole();
                          e.loadGrant(grantId);
                          e.loadEvents();
                          e.grantMenu(grantId);

                          e.$on('ngRepeatFinished', function(ngRepeatFinishedEvent) {
                              e.grantTimelineScroll();
                            });

                        }
                        else
                        {
                           $state.go('admin-panel.default.g-grants-list');
                        }
                        break

                      case "admin-panel.default.g-grants-people":

                        if(window.grantId)
                        {
                          e.loadRole();
                          e.loadGrant(grantId);
                          e.loadGrantUnivers(grantId);
                          //e.loadGrantPeople(grantId);
                          //e.loadUniversityQuotas(grantId);
                          //e.loadUniversityFullQuotas(grantId);
                          e.grantMenu(grantId);

                          e.$on('ngRepeatFinished', function(ngRepeatFinishedEvent) {
                              e.makeCheckboxGrid();
                            });
                        }
                        else
                        {
                           $state.go('admin-panel.default.g-grants-list');
                        }
                        break

                        case "admin-panel.default.g-grants-people-active":

                        if(window.grantId)
                        {
                          e.loadRole();
                          e.loadGrant(grantId);
                          e.loadGrantUnivers(grantId);
                          //e.loadGrantPeople(grantId);
                          //e.loadUniversityQuotas(grantId);
                          //e.loadUniversityFullQuotas(grantId);
                          e.grantMenu(grantId);

                          e.$on('ngRepeatFinished', function(ngRepeatFinishedEvent) {
                              e.makeCheckboxGrid();
                            });
                        }
                        else
                        {
                           $state.go('admin-panel.default.g-grants-list');
                        }
                        break

                        case "admin-panel.default.g-grants-people-social":

                        if(window.grantId)
                        {
                          e.loadRole();
                          e.loadGrant(grantId);
                          e.loadGrantUnivers(grantId);
                          //e.loadGrantPeople(grantId);
                          //e.loadUniversityQuotas(grantId);
                          //e.loadUniversityFullQuotas(grantId);
                          e.grantMenu(grantId);

                          e.$on('ngRepeatFinished', function(ngRepeatFinishedEvent) {
                              e.makeCheckboxGrid();
                            });
                        }
                        else
                        {
                           $state.go('admin-panel.default.g-grants-list');
                        }
                        break

                      case "admin-panel.default.g-grants-additional":

                        if(window.grantId)
                        {
                          e.loadRole();
                          e.loadGrant(grantId);
                          e.loadGrantAdditionalUnivers(grantId);
                          e.grantMenu(grantId);

                          e.$on('ngRepeatFinished', function(ngRepeatFinishedEvent) {
                              e.makeAdditionalCheckboxGrid();
                               e.AdditionalWinnersScroll();
                            });

                        }
                        else
                        {
                           $state.go('admin-panel.default.g-grants-list');
                        }
                        break

                      case "admin-panel.default.g-grants-unv":

                        if(window.grantId)
                        {

                          if(window.universityQuotasCash){
                            e.universityQuota = window.universityQuotasCash;
                          }

                          if(e.unvLoaded) {}
                            else{

                               e.loadRole();
                               e.loadGrant(grantId);
                               e.loadGrantQuotas(grantId);
                               e.loadUniversities();
                               e.grantMenu(grantId);
                               e.unvLoaded = true;
                            }
                          
                          e.$on('ngRepeatFinished', function(ngRepeatFinishedEvent) {
                              e.makeEditableGrid(); 
                            });
                        }
                        else
                        {
                           $state.go('admin-panel.default.g-grants-list');
                        }
                        break

                      case "admin-panel.default.g-grants-participate":

                        e.grantParticipateProgress = 0;
                        e.controlProgress();

                        if($stateParams.code)
                        { 
                          e.participantCode = $stateParams.code;
                        }
                        else{
                          e.participantCode = 'a';
                        }

                        if(window.grantId)
                        {
                          e.loadRole();
                          e.loadIsParticipant(window.grantId);
                          e.loadGrant(grantId);
                          e.grantMenu(grantId);
                          e.loadStudentInfo();

                           e.$on('ngRepeatFinished', function(ngRepeatFinishedEvent) {
                              e.makeProgressTabs(); 
                            });
                        }
                        else
                        {
                           $state.go('admin-panel.default.g-grants-list');
                        }

                      break;

                      case "admin-panel.default.g-grants-list":

                      e.loadRole();

                       if(window.oldMenu){
                               e.returnMenu();
                       }
                       break

                      case "admin-panel.default.g-grants-stat":

                        e.loadRole();
                        e.loadGrantStat();
                        e.loadUserStat();
                        e.grantMenu(grantId);
                        e.loadGrantChart(grantId);  
                        
             

                      break;

                       case "admin-panel.default.g-grants-reports":

                        e.loadRole();
                        e.grantMenu(grantId);

                      break;

                      default :
                         var test = '';  
             }


            
                   // e.loadRole();


        }
    ]);