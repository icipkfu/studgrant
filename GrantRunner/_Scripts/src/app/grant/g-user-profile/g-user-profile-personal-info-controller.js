angular.module("g-user-profile-personal-info", [])
    .controller("StudentPersonalInfoController", [
        "$scope", "$http", "API_CONFIG", "$q", "gProgress", "$mdToast", "FileUploader", "$mdDialog", '$stateParams',
        function (e, $http, apiConfig, q, gp, $mdToast, FileUploader, $mdDialog, $stateParams) {


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


            var getUrl = apiConfig.serverPath + "api/personalInfo/1",
                postUrl = getUrl,
                cutDate = function(dstr){
                    if(!dstr) return null;
                    var s= dstr.substring(0, 10);

                   return new Date(s);
                };


            e.loadPinfoData = function () {
                 $http.get(getUrl)
                .then(function(resp){
                    // resp.data.Birthday = cutDate(resp.data.Birthday);

                    if(resp && resp.data){


                    if(resp.data.Sex==1)
                    {
                        e.pinfo.SexName = 'Мужской';
                    } else {
                        e.pinfo.SexName = 'Женский';
                    }

                    if(resp.data.Citizenship==1)
                    {
                        e.pinfo.CitizenshipText = 'Иное';
                    } else {
                        e.pinfo.CitizenshipText = 'РФ';
                    }

                    e.pinfo.EditDateText =  dateFormat(e.pinfo.EditDate, 'dd.mm.yyyy HH:MM:ss', true);

                    if(resp.data.Birthday){
                        resp.data.BirthdayText = cutDate(resp.data.Birthday).format('dd.mm.yyyy');
                    }

                    if(resp.data.Birthplace && resp.data.Birthplace.length > 32)
                    {
                       resp.data.Birthplace = resp.data.Birthplace.substr(0,32);
                    }

                    if(resp.data.PassportIssuedBy && resp.data.PassportIssuedBy.length > 32)
                    {
                       resp.data.PassportIssuedBy = resp.data.PassportIssuedBy.substr(0,50);
                    }


                     // resp.data.PassportIssueDate = cutDate(resp.data.PassportIssueDate);
                     if(resp.data.PassportIssueDate){
                         resp.data.PassportIssueDateText =cutDate(resp.data.PassportIssueDate).format('dd.mm.yyyy')
                     }
                 }


                    angular.extend(e.pinfo, resp.data);
                });
            }

            e.pinfo = e.pinfo || {};
            e.loadPinfoData();


            e.issuedByCodeChanged = function() {

                var index = e.pinfo.PassportIssuedByCode.indexOf('-');

                if(e.pinfo.PassportIssuedByCode && e.pinfo.PassportIssuedByCode.length == 3 && index<0){
                    e.pinfo.PassportIssuedByCode += '-';
                }

                if(e.pinfo.PassportIssuedByCode && e.pinfo.PassportIssuedByCode.length == 3 && index==2){
                    e.pinfo.PassportIssuedByCode =   e.pinfo.PassportIssuedByCode.substr(0,2);
                }

                if(e.pinfo.PassportIssuedByCode && e.pinfo.PassportIssuedByCode.length > 3 && index<0){
                    e.pinfo.PassportIssuedByCode =  e.pinfo.PassportIssuedByCode.substr(0,3) + '-';
                }

                if(e.pinfo.PassportIssuedByCode && e.pinfo.PassportIssuedByCode.length > 3 && index>=0 && index!=3){
                    e.pinfo.PassportIssuedByCode =  e.pinfo.PassportIssuedByCode.replace('-','').substr(0,3) + '-';
                }


                if(e.pinfo.PassportIssuedByCode && e.pinfo.PassportIssuedByCode.length > 7){
                    e.pinfo.PassportIssuedByCode = e.pinfo.PassportIssuedByCode.substr(0,7);
                }


                if(e.pinfo.PassportIssuedByCode.length == 7
                      && index == 3
                      && e.pinfo.PassportIssuedByCode.replace('-','').indexOf('-') < 0){

                    e.personal.passportIssuedByCode.$error = {};
                    e.pinfo.passportIssuedByCodeInvalid = false;
                }
            else
            {
               e.pinfo.passportIssuedByCodeInvalid = true;
                 e.personal.passportIssuedByCode.$error =
                {
                    pattern:true
                };
            }




            }

            e.birthdayChanged = function(a,b,c) {


                if(e.pinfo.BirthdayText) {


                    if(e.pinfo.BirthdayText.length<4){

                         var index = e.pinfo.BirthdayText.indexOf('.');


                        if(e.pinfo.BirthdayText.length == 2 && index==1){

                            e.pinfo.BirthdayText = e.pinfo.BirthdayText.substr(0,1);

                        } else if(e.pinfo.BirthdayText.length == 2 && index<0){

                            e.pinfo.BirthdayText += '.';

                        } else if(e.pinfo.BirthdayText.length > 2 && index<0){

                            e.pinfo.BirthdayText = e.pinfo.BirthdayText.substr(0,2) + '.';

                        } else if(e.pinfo.BirthdayText.length > 2 && index!=2){

                            var b = e.pinfo.BirthdayText.replace('.','');
                            if(b.length==2) {
                                e.pinfo.BirthdayText = b + '.';
                            } else {
                                e.pinfo.BirthdayText = b;
                            }
                        }

                          e.pinfo.BirthdayInvalid = true;
                    }
                    else if(e.pinfo.BirthdayText.length>4 && e.pinfo.BirthdayText.length<7){

                        var len = e.pinfo.BirthdayText.replace('.','').length;

                         var index = e.pinfo.BirthdayText.replace('.','').indexOf('.');

                         if(e.pinfo.BirthdayText.length == 5 && index==3){
                            e.pinfo.BirthdayText = e.pinfo.BirthdayText.substr(0,4);
                         }
                         else if(e.pinfo.BirthdayText.length == 5 && index<0 && len == 4){
                            e.pinfo.BirthdayText += '.';
                        }
                        else if(e.pinfo.BirthdayText.length > 4 && index!= 4){

                            e.pinfo.BirthdayText = '';
                        }

                          e.pinfo.BirthdayInvalid = true;

                     }  else if (e.pinfo.BirthdayText.length >= 7 && e.pinfo.BirthdayText.length < 10){

                        e.pinfo.BirthdayInvalid = true;


                     }  else if (e.pinfo.BirthdayText.length == 10){

                        var from = e.pinfo.BirthdayText.split(".");
                        var dt = new Date(from[2], from[1] - 1, from[0], 20);

                        if(dt.valueOf() > 0 || dt.valueOf() < 0) {


                            var year = dt.getFullYear();

                            if(year > 2002 || year < 1900){
                                 e.pinfo.BirthdayInvalid = true;
                            }
                            else{
                                 e.pinfo.BirthdayInvalid = false;
                                  e.pinfo.Birthday = dt;
                            }

                        } else{
                            e.pinfo.BirthdayInvalid = true;
                        }

                     } else if (e.pinfo.BirthdayText.length > 10){

                        e.pinfo.BirthdayText = e.pinfo.BirthdayText.substr(0,10);

                     }
                }
            }

             e.passportIssueDateChanged = function(a,b,c) {


                if(e.pinfo.PassportIssueDateText) {


                    if(e.pinfo.PassportIssueDateText.length<4){

                         var index = e.pinfo.PassportIssueDateText.indexOf('.');


                        if(e.pinfo.PassportIssueDateText.length == 2 && index==1){

                            e.pinfo.PassportIssueDateText = e.pinfo.PassportIssueDateText.substr(0,1);

                        } else if(e.pinfo.PassportIssueDateText.length == 2 && index<0){

                            e.pinfo.PassportIssueDateText += '.';

                        } else if(e.pinfo.PassportIssueDateText.length > 2 && index<0){

                            e.pinfo.PassportIssueDateText = e.pinfo.PassportIssueDateText.substr(0,2) + '.';

                        } else if(e.pinfo.PassportIssueDateText.length > 2 && index!=2){

                            var b = e.pinfo.PassportIssueDateText.replace('.','');
                            if(b.length==2) {
                                e.pinfo.PassportIssueDateText = b + '.';
                            } else {
                                e.pinfo.PassportIssueDateText = b;
                            }
                        }

                          e.pinfo.PassportIssueDateInvalid = true;
                    }
                    else if(e.pinfo.PassportIssueDateText.length>4 && e.pinfo.PassportIssueDateText.length<7){

                        var len = e.pinfo.PassportIssueDateText.replace('.','').length;

                         var index = e.pinfo.PassportIssueDateText.replace('.','').indexOf('.');

                         if(e.pinfo.PassportIssueDateText.length == 5 && index==3){
                            e.pinfo.PassportIssueDateText = e.pinfo.PassportIssueDateText.substr(0,4);
                         }
                         else if(e.pinfo.PassportIssueDateText.length == 5 && index<0 && len == 4){
                            e.pinfo.PassportIssueDateText += '.';
                        }
                        else if(e.pinfo.PassportIssueDateText.length > 4 && index!= 4){

                            e.pinfo.PassportIssueDateText = '';
                        }

                          e.pinfo.PassportIssueDateInvalid = true;

                     }  else if (e.pinfo.PassportIssueDateText.length >= 7 && e.pinfo.PassportIssueDateText.length < 10){

                        e.pinfo.PassportIssueDateInvalid = true;


                     }  else if (e.pinfo.PassportIssueDateText.length == 10){

                        var from = e.pinfo.PassportIssueDateText.split(".");
                        var dt = new Date(from[2], from[1] - 1, from[0], 20);

                        if(dt.valueOf() > 0 || dt.valueOf() < 0) {


                            var year = dt.getFullYear();

                            if(year > 2018 || year < 1900){
                                 e.pinfo.PassportIssueDateInvalid = true;
                            }
                            else{
                                 e.pinfo.PassportIssueDateInvalid = false;
                                  e.pinfo.PassportIssueDate = dt;
                            }

                        } else{
                            e.pinfo.PassportIssueDateInvalid = true;
                        }

                     } else if (e.pinfo.PassportIssueDateText.length > 10){

                        e.pinfo.PassportIssueDateText = e.pinfo.PassportIssueDateText.substr(0,10);

                     }
                }
            }

            e.updateUserPersonalInfoClick = function () {
                var promise = $http.post(postUrl, e.pinfo),
                         deferred = q.defer();
                gp.mask(e);
                promise.then(
                    function (resp) {

                    e.loadCurrent();

                    if(resp.data.Sex==1)
                    {
                        e.pinfo.SexName = 'Мужской';
                    } else {
                        e.pinfo.SexName = 'Женский';
                    }

                    if(resp.data.Citizenship==1)
                    {
                        e.pinfo.CitizenshipText = 'Иное';
                    } else {
                        e.pinfo.CitizenshipText = 'РФ';
                    }

                    if(resp.data.Birthday){
                        resp.data.BirthdayText = cutDate(resp.data.Birthday).format('dd.mm.yyyy');
                    }


                     // resp.data.PassportIssueDate = cutDate(resp.data.PassportIssueDate);
                     if(resp.data.PassportIssueDate){
                         resp.data.PassportIssueDateText =cutDate(resp.data.PassportIssueDate).format('dd.mm.yyyy')
                     }


                        var data = resp.data;

                        angular.extend(e.pinfo, data);

                        gp.unmask(e);

                        $mdToast.show(
                        $mdToast.simple()
                        .content('Информация сохранена')
                        .position('top'));

                        e.loadIsPassportFilled();

                        e.infoEdit = false;

                        e.pinfoUploader.clearQueue();


                        deferred.resolve(e.pinfo);
                    },
                    function (reason) {
                        gp.unmask(e);

                        $mdToast.show(
                            $mdToast.simple()
                            .content('Во время сохранения персональной информации произошли ошибки')
                            .position('top')
                            .theme("error-toast")
                            .hideDelay(5000)
                        );
                    });
                return deferred.promise;
            };

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
            e.delete = function (item) {
                $http.get(apiConfig.serverPath +"api/personalInfoData/" + item.Hash)
                .then(function (resp) {
                    angular.extend(e.pinfo, resp.data);
                });
            }

            e.askUpdateUserPersonalInfoClick = function(){


                if(e.user.PassportState == 1)
                {
                    $mdDialog.show(
                        $mdDialog.confirm()
                        .title('Изменение персональных данных')
                        .content("Ваши данные проверены, любое изменение потребует новой проверки! Продолжить?")
                        .ok("Да")
                        .cancel("Отмена")).then(function(result){

                          if(result){
                             e.updateUserPersonalInfoClick();
                          }
                    });
                } else{

                    e.updateUserPersonalInfoClick();

                }
            }



            e.showFiles = null;

            e.scanImageClick = function () {
               // if(this.mouseActive)
               // {
                      $("#passportScanFileInput").click();
              //  }

            };

             e.scanImageClick2 = function () {
                //if(this.mouseActive)
                //{
                      $("#passportScanFileInput2").click();
                //}

            };

             e.scanImageClick3 = function () {
               // if(this.mouseActive)
               // {
                      $("#passportScanFileInput3").click();
               // }

            };

             e.scanImageClick4 = function () {
               // if(this.mouseActive)
               // {
                      $("#passportScanFileInput4").click();
               // }

            };

            e.scanImageClick5 = function () {
               // if(this.mouseActive)
               // {
                      $("#passportScanFileInput5").click();
               // }

            };

             e.scanImageClick6 = function () {
              //  if(this.mouseActive)
               // {
                      $("#passportScanFileInput6").click();
               // }

            };

            e.scanImageClick7 = function () {
              //  if(this.mouseActive)
              //  {
                      $("#passportScanFileInput7").click();
              //  }

            };

            e.scanImageClick8 = function () {
              //  if(this.mouseActive)
              //  {
                      $("#passportScanFileInput8").click();
              //  }

            };

            e.scanImageClick9 = function () {
              //  if(this.mouseActive)
              //  {
                      $("#passportScanFileInput9").click();
              //  }

            };

            e.scanImageClick10 = function () {
              //  if(this.mouseActive)
              //  {
                      $("#passportScanFileInput10").click();
              //  }

            };







            var pinfoUploader = e.pinfoUploader = new FileUploader({
                url: apiConfig.serverPath + 'api/filehighresData'
            });


            // CALLBACKS

            e.pinfoUploader.onWhenAddingFileFailed = function (item /*{File|FileLikeObject}*/, filter, options) {
                //console.info('onWhenAddingFileFailed', item, filter, options);
            };

            e.pinfoUploader.onAfterAddingFile = function (fileItem) {

                var ext = '';
                var validExts = ["jpg", "png"];
                var isvalid = false;

                if(fileItem && fileItem.file && fileItem.file.name && fileItem.file.name.length>4){

                   var filename = fileItem.file.name;
                   ext = filename.substr(filename.length - 3, 3).toLowerCase();
                   isvalid = validExts.indexOf(ext) > -1;
                }

                if(!isvalid){


                     $mdToast.show(
                                    $mdToast.simple()
                                    .content("Требуется загрузить изображение в формате JPG либо PNG. Формат Вашего файла - " + ext +". Откройте картинку в Paint и сохраните в PNG")
                                    //.theme("error-toast")
                                    .hideDelay(10000)
                                    .position('top')
                                );


                    return;
                }

                gp.mask(e);
                fileItem.upload();
            };

            e.pinfoUploader.onCompleteItem = function (fileItem, response, status, headers) {

                gp.unmask(e);


                if(status == 200) {

                    e.pinfo.PassportPage1 = response;
                    e.pinfo.PassportPage1Id = response.Id;

                } else {
                    $mdToast.show(
                    $mdToast.simple()
                    .content(response)
                    .theme("error-toast")
                    .position('top'));
                }
            };


            var pinfoUploader2 = e.pinfoUploader2 = new FileUploader({
                url: apiConfig.serverPath + 'api/filehighresData'
            });


            // CALLBACKS

            e.pinfoUploader2.onWhenAddingFileFailed = function (item /*{File|FileLikeObject}*/, filter, options) {
                //console.info('onWhenAddingFileFailed', item, filter, options);
            };

            e.pinfoUploader2.onAfterAddingFile = function (fileItem) {

                var ext = '';
                var validExts = ["jpg", "png"];
                var isvalid = false;

                if(fileItem && fileItem.file && fileItem.file.name && fileItem.file.name.length>4){

                   var filename = fileItem.file.name;
                   ext = filename.substr(filename.length - 3, 3).toLowerCase();
                   isvalid = validExts.indexOf(ext) > -1;
                }

                if(!isvalid){


                     $mdToast.show(
                                    $mdToast.simple()
                                    .content("Требуется загрузить изображение в формате JPG либо PNG. Формат Вашего файла - " + ext +". Откройте картинку в Paint и сохраните в PNG")
                                    //.theme("error-toast")
                                    .hideDelay(10000)
                                    .position('top')
                                );


                    return;
                }

                gp.mask(e);
                fileItem.upload();
            };

            e.pinfoUploader2.onCompleteItem = function (fileItem, response, status, headers) {

                gp.unmask(e);


                if(status == 200) {

                    e.pinfo.PassportPage2 = response;
                    e.pinfo.PassportPage2Id = response.Id;

                } else {
                    $mdToast.show(
                    $mdToast.simple()
                    .content(response)
                    .theme("error-toast")
                    .position('top'));
                }
            };


            var pinfoUploader3 = e.pinfoUploader3 = new FileUploader({
                url: apiConfig.serverPath + 'api/filehighresData'
            });

            // CALLBACKS

            e.pinfoUploader3.onWhenAddingFileFailed = function (item /*{File|FileLikeObject}*/, filter, options) {
                //console.info('onWhenAddingFileFailed', item, filter, options);
            };

            e.pinfoUploader3.onAfterAddingFile = function (fileItem) {

                var ext = '';
                var validExts = ["jpg", "png"];
                var isvalid = false;

                if(fileItem && fileItem.file && fileItem.file.name && fileItem.file.name.length>4){

                   var filename = fileItem.file.name;
                   ext = filename.substr(filename.length - 3, 3).toLowerCase();
                   isvalid = validExts.indexOf(ext) > -1;
                }

                if(!isvalid){


                     $mdToast.show(
                                    $mdToast.simple()
                                    .content("Требуется загрузить изображение в формате JPG либо PNG. Формат Вашего файла - " + ext +". Откройте картинку в Paint и сохраните в PNG")
                                    //.theme("error-toast")
                                    .hideDelay(10000)
                                    .position('top')
                                );


                    return;
                }

                gp.mask(e);
                fileItem.upload();
            };

            e.pinfoUploader3.onCompleteItem = function (fileItem, response, status, headers) {
                gp.unmask(e);


                if(status == 200) {

                    e.pinfo.PassportPage3 = response;
                    e.pinfo.PassportPage3Id = response.Id;

                } else {
                    $mdToast.show(
                    $mdToast.simple()
                    .content(response)
                    .theme("error-toast")
                    .position('top'));
                }
            };


            var pinfoUploader4 = e.pinfoUploader4 = new FileUploader({
                url: apiConfig.serverPath + 'api/filehighresData'
            });

            // CALLBACKS

            e.pinfoUploader4.onWhenAddingFileFailed = function (item /*{File|FileLikeObject}*/, filter, options) {
                //console.info('onWhenAddingFileFailed', item, filter, options);
            };

            e.pinfoUploader4.onAfterAddingFile = function (fileItem) {
                var ext = '';
                var validExts = ["jpg", "png"];
                var isvalid = false;

                if(fileItem && fileItem.file && fileItem.file.name && fileItem.file.name.length>4){

                   var filename = fileItem.file.name;
                   ext = filename.substr(filename.length - 3, 3).toLowerCase();
                   isvalid = validExts.indexOf(ext) > -1;
                }

                if(!isvalid){


                     $mdToast.show(
                                    $mdToast.simple()
                                    .content("Требуется загрузить изображение в формате JPG либо PNG. Формат Вашего файла - " + ext +". Откройте картинку в Paint и сохраните в PNG")
                                    //.theme("error-toast")
                                    .hideDelay(10000)
                                    .position('top')
                                );


                    return;
                }

                gp.mask(e);
                fileItem.upload();
            };

            e.pinfoUploader4.onCompleteItem = function (fileItem, response, status, headers) {

                gp.unmask(e);

                if(status == 200) {

                    e.pinfo.PassportPage4 = response;
                    e.pinfo.PassportPage4Id = response.Id;

                } else {
                    $mdToast.show(
                    $mdToast.simple()
                    .content(response)
                    .theme("error-toast")
                    .position('top'));
                }
            };

            var pinfoUploader5 = e.pinfoUploader5 = new FileUploader({
                url: apiConfig.serverPath + 'api/filehighresData'
            });

            // CALLBACKS

            e.pinfoUploader5.onWhenAddingFileFailed = function (item /*{File|FileLikeObject}*/, filter, options) {
                //console.info('onWhenAddingFileFailed', item, filter, options);
            };

            e.pinfoUploader5.onAfterAddingFile = function (fileItem) {

                var ext = '';
                var validExts = ["jpg", "png"];
                var isvalid = false;

                if(fileItem && fileItem.file && fileItem.file.name && fileItem.file.name.length>4){

                   var filename = fileItem.file.name;
                   ext = filename.substr(filename.length - 3, 3).toLowerCase();
                   isvalid = validExts.indexOf(ext) > -1;
                }

                if(!isvalid){


                     $mdToast.show(
                                    $mdToast.simple()
                                    .content("Требуется загрузить изображение в формате JPG либо PNG. Формат Вашего файла - " + ext +". Откройте картинку в Paint и сохраните в PNG")
                                    //.theme("error-toast")
                                    .hideDelay(10000)
                                    .position('top')
                                );


                    return;
                }

                gp.mask(e);
                fileItem.upload();
            };

            e.pinfoUploader5.onCompleteItem = function (fileItem, response, status, headers) {
                gp.unmask(e);


                if(status == 200) {

                    e.pinfo.PassportPage5 = response;
                    e.pinfo.PassportPage5Id = response.Id;

                } else {
                    $mdToast.show(
                    $mdToast.simple()
                    .content(response)
                    .theme("error-toast")
                    .position('top'));
                }
            };

             var pinfoUploader6 = e.pinfoUploader6 = new FileUploader({
                url: apiConfig.serverPath + 'api/filehighresData'
            });


            // CALLBACKS

            e.pinfoUploader6.onWhenAddingFileFailed = function (item /*{File|FileLikeObject}*/, filter, options) {
                //console.info('onWhenAddingFileFailed', item, filter, options);
            };

            e.pinfoUploader6.onAfterAddingFile = function (fileItem) {

                var ext = '';
                var validExts = ["jpg", "png"];
                var isvalid = false;

                if(fileItem && fileItem.file && fileItem.file.name && fileItem.file.name.length>4){

                   var filename = fileItem.file.name;
                   ext = filename.substr(filename.length - 3, 3).toLowerCase();
                   isvalid = validExts.indexOf(ext) > -1;
                }

                if(!isvalid){


                     $mdToast.show(
                                    $mdToast.simple()
                                    .content("Требуется загрузить изображение в формате JPG либо PNG. Формат Вашего файла - " + ext +". Откройте картинку в Paint и сохраните в PNG")
                                    //.theme("error-toast")
                                    .hideDelay(10000)
                                    .position('top')
                                );


                    return;
                }

                gp.mask(e);
                fileItem.upload();
            };

            e.pinfoUploader6.onCompleteItem = function (fileItem, response, status, headers) {
                gp.unmask(e);

                if(status == 200) {

                    e.pinfo.PassportPage6 = response;
                    e.pinfo.PassportPage6Id = response.Id;

                } else {
                    $mdToast.show(
                    $mdToast.simple()
                    .content(response)
                    .theme("error-toast")
                    .position('top'));
                }
            };


             var pinfoUploader7 = e.pinfoUploader7 = new FileUploader({
                url: apiConfig.serverPath + 'api/filehighresData'
            });


            // CALLBACKS

            e.pinfoUploader7.onWhenAddingFileFailed = function (item /*{File|FileLikeObject}*/, filter, options) {
                //console.info('onWhenAddingFileFailed', item, filter, options);
            };

            e.pinfoUploader7.onAfterAddingFile = function (fileItem) {

                var ext = '';
                var validExts = ["jpg", "png"];
                var isvalid = false;

                if(fileItem && fileItem.file && fileItem.file.name && fileItem.file.name.length>4){

                   var filename = fileItem.file.name;
                   ext = filename.substr(filename.length - 3, 3).toLowerCase();
                   isvalid = validExts.indexOf(ext) > -1;
                }

                if(!isvalid){


                     $mdToast.show(
                                    $mdToast.simple()
                                    .content("Требуется загрузить изображение в формате JPG либо PNG. Формат Вашего файла - " + ext +". Откройте картинку в Paint и сохраните в PNG")
                                    //.theme("error-toast")
                                    .hideDelay(10000)
                                    .position('top')
                                );


                    return;
                }

                gp.mask(e);
                fileItem.upload();
            };

            e.pinfoUploader7.onCompleteItem = function (fileItem, response, status, headers) {
                gp.unmask(e);

                if(status == 200) {

                    e.pinfo.PassportPage7 = response;
                    e.pinfo.PassportPage7Id = response.Id;

                } else {
                    $mdToast.show(
                    $mdToast.simple()
                    .content(response)
                    .theme("error-toast")
                    .position('top'));
                }
            };

            var pinfoUploader8 = e.pinfoUploader8 = new FileUploader({
                url: apiConfig.serverPath + 'api/filehighresData'
            });

            // CALLBACKS

            e.pinfoUploader8.onWhenAddingFileFailed = function (item /*{File|FileLikeObject}*/, filter, options) {
                //console.info('onWhenAddingFileFailed', item, filter, options);
            };

            e.pinfoUploader8.onAfterAddingFile = function (fileItem) {
                var ext = '';
                var validExts = ["jpg", "png"];
                var isvalid = false;

                if(fileItem && fileItem.file && fileItem.file.name && fileItem.file.name.length>4){

                   var filename = fileItem.file.name;
                   ext = filename.substr(filename.length - 3, 3).toLowerCase();
                   isvalid = validExts.indexOf(ext) > -1;
                }

                if(!isvalid){


                     $mdToast.show(
                                    $mdToast.simple()
                                    .content("Требуется загрузить изображение в формате JPG либо PNG. Формат Вашего файла - " + ext +". Откройте картинку в Paint и сохраните в PNG")
                                    //.theme("error-toast")
                                    .hideDelay(10000)
                                    .position('top')
                                );


                    return;
                }

                gp.mask(e);
                fileItem.upload();
            };

            e.pinfoUploader8.onCompleteItem = function (fileItem, response, status, headers) {
                gp.unmask(e);

                if(status == 200) {

                    e.pinfo.PassportPage8 = response;
                    e.pinfo.PassportPage8Id = response.Id;

                } else {
                    $mdToast.show(
                    $mdToast.simple()
                    .content(response)
                    .theme("error-toast")
                    .position('top'));
                }
            };

            var pinfoUploader9 = e.pinfoUploader9 = new FileUploader({
                url: apiConfig.serverPath + 'api/filehighresData'
            });


            // CALLBACKS

            e.pinfoUploader9.onWhenAddingFileFailed = function (item /*{File|FileLikeObject}*/, filter, options) {
                //console.info('onWhenAddingFileFailed', item, filter, options);
            };

            e.pinfoUploader9.onAfterAddingFile = function (fileItem) {
                var ext = '';
                var validExts = ["jpg", "png"];
                var isvalid = false;

                if(fileItem && fileItem.file && fileItem.file.name && fileItem.file.name.length>4){

                   var filename = fileItem.file.name;
                   ext = filename.substr(filename.length - 3, 3).toLowerCase();
                   isvalid = validExts.indexOf(ext) > -1;
                }

                if(!isvalid){


                     $mdToast.show(
                                    $mdToast.simple()
                                    .content("Требуется загрузить изображение в формате JPG либо PNG. Формат Вашего файла - " + ext +". Откройте картинку в Paint и сохраните в PNG")
                                    //.theme("error-toast")
                                    .hideDelay(10000)
                                    .position('top')
                                );


                    return;
                }

                gp.mask(e);
                fileItem.upload();
            };

            e.pinfoUploader9.onCompleteItem = function (fileItem, response, status, headers) {
                gp.unmask(e);

                if(status == 200) {

                    e.pinfo.PassportPage9 = response;
                    e.pinfo.PassportPage9Id = response.Id;

                } else {
                    $mdToast.show(
                    $mdToast.simple()
                    .content(response)
                    .theme("error-toast")
                    .position('top'));
                }
            };

            var pinfoUploader10 = e.pinfoUploader10 = new FileUploader({
                url: apiConfig.serverPath + 'api/filehighresData'
            });

            // CALLBACKS

            e.pinfoUploader10.onWhenAddingFileFailed = function (item /*{File|FileLikeObject}*/, filter, options) {
                //console.info('onWhenAddingFileFailed', item, filter, options);
            };

            e.pinfoUploader10.onAfterAddingFile = function (fileItem) {
                var ext = '';
                var validExts = ["jpg", "png"];
                var isvalid = false;

                if(fileItem && fileItem.file && fileItem.file.name && fileItem.file.name.length>4){

                   var filename = fileItem.file.name;
                   ext = filename.substr(filename.length - 3, 3).toLowerCase();
                   isvalid = validExts.indexOf(ext) > -1;
                }

                if(!isvalid){


                     $mdToast.show(
                                    $mdToast.simple()
                                    .content("Требуется загрузить изображение в формате JPG либо PNG. Формат Вашего файла - " + ext +". Откройте картинку в Paint и сохраните в PNG")
                                    //.theme("error-toast")
                                    .hideDelay(10000)
                                    .position('top')
                                );


                    return;
                }

                gp.mask(e);

                fileItem.upload();
            };

            e.pinfoUploader10.onCompleteItem = function (fileItem, response, status, headers) {
                gp.unmask(e);

                if(status == 200) {

                    e.pinfo.PassportPage10 = response;
                    e.pinfo.PassportPage10Id = response.Id;

                } else {
                    $mdToast.show(
                    $mdToast.simple()
                    .content(response)
                    .theme("error-toast")
                    .position('top'));
                }
            };



             if($stateParams.tab){
                 e.infoEdit = true;
             }

             e.CopyAddress = function(){

                if(e.pinfo.RegistrationRepublic)
                {
                    e.pinfo.LiveRepublic = e.pinfo.RegistrationRepublic;

                } else {

                  $mdToast.show(
                    $mdToast.simple()
                    .content('Не заполнено поле Адрес прописки: Субъект РФ')
                    .theme("error-toast")
                    .position('top'));

                  e.pinfo.IsLiveAddressSame = false;

                  return;

                }


                if(e.pinfo.RegistrationzZone)
                {
                    e.pinfo.LiveZone = e.pinfo.RegistrationzZone;
                }

                if(e.pinfo.RegistrationIndex)
                {
                    e.pinfo.LiveIndex = e.pinfo.RegistrationIndex;

                } else {

                  $mdToast.show(
                    $mdToast.simple()
                    .content('Не заполнено поле Адрес прописки: Почтовый индекс')
                    .theme("error-toast")
                    .position('top'));

                  e.pinfo.IsLiveAddressSame = false;

                   return;

                }

                if(e.pinfo.RegistrationCity)
                {
                    e.pinfo.LiveCity = e.pinfo.RegistrationCity;
                }

                if(e.pinfo.RegistrationPlace)
                {
                    e.pinfo.LivePlace = e.pinfo.RegistrationPlace;

                }  else {

                    if(e.pinfo.RegistrationCity) {} else{

                         $mdToast.show(
                            $mdToast.simple()
                            .content('Не заполнено одно из двух полей -  Адрес прописки: Город / Населенный пункт')
                            .theme("error-toast")
                            .position('top'));

                          e.pinfo.IsLiveAddressSame = false;

                           return;

                    }
                }

                if(e.pinfo.RegistrationStreet)
                {
                    e.pinfo.LiveStreet = e.pinfo.RegistrationStreet

                } else {

                  $mdToast.show(
                    $mdToast.simple()
                    .content('Не заполнено поле Адрес прописки: Улица')
                    .theme("error-toast")
                    .position('top'));

                  e.pinfo.IsLiveAddressSame = false;

                   return;

                }

                if(e.pinfo.RegistrationHouse)
                {
                    e.pinfo.LiveHouse = e.pinfo.RegistrationHouse

                } else {

                  $mdToast.show(
                    $mdToast.simple()
                    .content('Не заполнено поле Адрес прописки: Дом')
                    .theme("error-toast")
                    .position('top'));

                  e.pinfo.IsLiveAddressSame = false;

                   return;

                }

                if(e.pinfo.RegistrationHousing)
                {
                    e.pinfo.LiveHousing = e.pinfo.RegistrationHousing
                }

               if(e.pinfo.RegistrationFlat)
                {
                    e.pinfo.LiveFlat = e.pinfo.RegistrationFlat

                }

                /* else {

                  $mdToast.show(
                    $mdToast.simple()
                    .content('Не заполнено поле Адрес прописки: Квартира')
                    .theme("error-toast")
                    .position('top'));

                  e.pinfo.IsLiveAddressSame = false;

                   return;

                } */

             }

            e.loadIsPassportFilled = function()
            {
               $http.get(apiConfig.serverPath +'api/personalInfo/ispassportFilled/'+ 0)
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
            e.loadIsPassportFilled();

        }
    ])
    .controller('ScanDialogController', function ($scope, $mdDialog, image) {
    $scope.currentImage = image;

    $scope.closeDialog = function () {
        $mdDialog.cancel();
    };
});
