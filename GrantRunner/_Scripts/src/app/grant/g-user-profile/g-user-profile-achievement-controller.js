angular.module("g-user-profile-achievement", [])
.config([
    "$translatePartialLoaderProvider", "$stateProvider", function(e, n) {
        n.state("admin-panel.default.g-user-achievement", {
            url: "/user/achievement/:id",
            templateUrl: 'app/grant/g-user-profile/g-user-achievement.tmpl.html',
            controller: "EditAchievementController"
        });
    }
])
.controller("StudentAchievementController", [
        "$scope", "$http", "$mdDialog", "API_CONFIG", "FileUploader", "gProgress", 'API_CONFIG', "$mdToast", '$timeout',
        function ($scope, $http, $mdDialog, apiConfig, FileUploader, gProgress, API_CONFIG, $mdToast, $timeout) {
            var listUrl = apiConfig.serverPath + "api/achievement/1",
                getUrl = apiConfig.serverPath + "api/achievement/";
            $scope.columns = [
                {
                    title: "",
                    field: "tb",
                    sortable: false
                }, {
                    title: "Название",
                    field: "Name",
                    sortable: true
                }, {
                    title: "Год",
                    field: "Year",
                    sortable: true
                }, {
                    title: "Тематика",
                    field: "Subject",
                    sortable: false
                }, {
                    title: "Статус",
                    field: "State",
                    sortable: false
                }, {
                    title: "Уровень",
                    field: "Level",
                    sortable: false
                }
            ];
            $scope.achievements = [];


            //todo переделать
            var getLevel = function (level) {

                switch(level){

                    case 0: return 'Международный уровень';
                    case 1: return 'Всероссийский уровень';
                    case 2: return 'Региональный уровень';
                    case 3: return 'Муниципальный (городской)';
                    case 4: return 'Образовательная организация';

                    case 5: return 'Руководитель комитета города (муниципального образования)';
                    case 6: return 'Мэр города (главы муниципального образования)';
                    case 7: return 'Глава ведомства региона';
                    case 8: return 'Глава правительства региона';
                    case 9: return 'Председатель законодательного собрания региона';
                    case 10: return 'Глава региона';
                    case 11: return 'Президент республики';
                    case 12: return 'Президент';
                    default: return '';

                }

            }

            //todo переделать
            var getState = function (level) {

                switch(level){

                    case 0: return 'Победитель';
                    case 1: return 'Участник';
                    case 2: return 'Организатор';
               
                    case 4: return 'Золото';
                    case 5: return 'Серебро';
                    case 6: return 'Бронза';
                    default: return '';
                }

            }

            //todo переделать
            var getSubject = function (level) {

                switch(level){

                    case 0: return 'Научно-исследовательская деятельность';
                    case 1: return 'Спортивная деятельность';
                    case 2: return 'Культурно творческая деятельность';
                    case 3: return 'Общественная деятельность';
                    case 4: return 'Государственные награды, знаки отличия и иные формы поощрения';
                    default: return '';

                }

            }

            $scope.delete =  function (item) {

                gProgress.mask($scope);

                $http.delete(API_CONFIG.serverPath +'api/achievement/' + item.Id)
                .success(function (data, status) {
                     gProgress.unmask($scope);

                     $mdToast.show(
                        $mdToast.simple()
                        .content('Достижение успешно удалено')
                        .position('top'));

                    $scope.loadAchievements();


                  }).error(function (error) {
                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
            }

             $scope.hideBadges = function(){

                var badges = $(".badge");
                badges.addClass('hiddenbadge');
            }

            $scope.loadIsAchievementFilled = function()
            {
               $http.get(API_CONFIG.serverPath +'api/achievement/isachievementFilled/'+ 0)
                    .success(function (data, status) {

                        $scope.isachievementFilled = data;

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
            }


           window.loadMyAchievements =  $scope.loadAchievements = function () {

                gProgress.mask($scope);

                $scope.hideBadges();

                var year = new Date().getFullYear();

                 $http.get(listUrl)
                .then(function(resp){
                    //$scope.achievements = [];

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
                            StateName: getState(t.State),
                            State: t.State,
                            ValidationState : t.ValidationState,
                            ValidStateName : getValidStateName(t.ValidationState),
                            Score: t.Score,
                            ValidationComment: t.ValidationComment,
                            Criterion: t.Criterion,
                            StudentId: t.StudentId,
                            SubjectName: getSubject(t.Subject),
                            Subject: t.Subject,
                            Year: t.Year,
                            FilesList: t.FilesList,
                            ImageLink: t.ImageLink,
                            ProofList: t.ProofList,
                            ProofFile: t.ProofFile
                        };

                        if(ach.Year >= year - 1){
                            achArr.push(ach);
                        } else{
                            oldArr.push(ach);
                        }
                    });

                    $scope.achievements = achArr;
                    $scope.oldAchievements = oldArr;

                    $scope.loadIsAchievementFilled();

                    setTimeout($scope.showBadges, 500 + 300 * achArr.length);

                    gProgress.unmask($scope);

                 });

            }

            if(!$scope.isModerating)
            {
                $scope.loadAchievements();
            }

            $scope.showHiddenBadges = function(){

                var badges = $(".badge");
                badges.removeClass('hiddenbadge');
                badges.show();
            }

            $scope.showBadges = function() {

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



                setTimeout($scope.showHiddenBadges, 500);

            }

            $( window ).resize(function() {
               $scope.showBadges();
            });


            $scope.$on('ngRepeatFinished', function(ngRepeatFinishedEvent) {
                 $scope.showBadges();
             });

            $scope.createAchievementClick = function () {

                var newItem = {};

                window.setAchievement(newItem);
                $("#achievementform").show();
                $(".achievementtoolbar").hide();
                //showEditAchivementDialog();
            }

            $scope.editAchievementClick = function (item) {

              //  if($scope.usersCantEditOption && $scope.user.Id != 6154){} else{
              //       showEditAchivementDialog(item);
             //   }

                window.setAchievement(item);
                $("#achievementform").show();
                $(".achievementtoolbar").hide();

                $("#admin-panel-content-view").parent().parent().animate({ scrollTop: 400 }, "slow");

            }

            var showEditAchivementDialog =  function (item) {
                $mdDialog.show({
                    templateUrl: 'app/grant/g-user-profile/g-user-achievement-edit.tmpl.html',
                    parent: angular.element(document.body),
                    controller: 'EditAchievementController',
                    bindToController: true,
                   // scope: $scope,
                    locals: {
                        achieve: item
                    }
                }).then(function () {
                        $scope.loadAchievements();
                    });
            };
            $scope.openFilesDialog = function (item, $event) {
                if (item.FilesList && item.FilesList.length > 0) {
                    $mdDialog.show({
                        templateUrl: 'app/grant/g-user-profile/user-profile-achievements-files.tmpl.html',
                        parent: angular.element(document.body),
                        controller: 'AchievementFilesController',
                        locals: {
                            achieve: item
                        },
                        bindToController: true,
                        clickOutsideToClose: true,
                        focusOnOpen: false
                    }).then(function () {
                        $scope.loadAchievements();
                    });
                }
                else {
                    $mdDialog.show(
                    $mdDialog.confirm()
                    .title("Отсутствие данных")
                    .content("У выбранного достижения нет прикрепленных документов")
                    .ok("Oк")
                    .cancel("Отмена")
                    //.targetEvent($event)
                );
                }

            }


        }
    ])
    .controller("EditAchievementController",
                ["$scope", "$mdDialog", "$http", "API_CONFIG", "FileUploader", '$mdToast', 'gProgress', '$stateParams',
        function ($scope, $mdDialog, $http, apiConfig, FileUploader, $mdToast, gProgress, $stateParams) {
            var postUrl = apiConfig.serverPath + "api/achievement/1",
                getUrl = apiConfig.serverPath + "api/achievement/1",
                putUrl = apiConfig.serverPath + "api/achievement/",
                i = 2018;
                //achieve = this.achieve;

            if (this.achieve) {
               // angular.extend($scope, achieve);
               $scope.achieve = this.achieve;
                $scope.years = [];

            }
            else {
                $scope.id = null;
                $scope.years = [];
                $scope.achieve = $scope.achieve || {};

                if ($scope.achieve.FilesArr) {}
                    else{
                        $scope.achieve.FilesArr = [];
                    }
            }

            $scope.isModerating = false;
  
            if(location.href && location.href.indexOf('/user/page/')>0){
                $scope.isModerating = true;
            }

              
            while(i <= 2019){
                $scope.years.push(i++);
            }

            window.loadAchievement = function(id){
              
                    gProgress.mask($scope);
    
                    $http.get(apiConfig.serverPath +'api/achievement/getbyid/' + id)
                    .success(function (data, status) {    
                        $scope.achieve = data;
                        gProgress.unmask($scope);
    
                      }).error(function (error) {
    
                        $mdToast.show(
                          $mdToast.simple()
                            .content(error.data.Message)
                            .theme("error-toast")
                            .position('top'));
    
    
                      });
            }

            window.setAchievement = function(item){
                $scope.achieve = item;

                if(item.Id){

                } else{

                }
                
                $scope.achieveUploader.clearQueue();
                $scope.attachUploader.clearQueue();

                item.ValidStateName = getValidStateName(item.ValidationState);
            }

            window.getValidStateName = function (level) {

                switch(level){

                    case 0: return 'Не проверены';
                    case 1: return 'Данные верны';
                    case 2: return 'Есть замечания';
                    case 3: return 'Не заполнены';
                    default: return '';
                }

            }

            if(window.achievementId){
                $scope.loadAchievement(window.achievementId);
            }

            $scope.setAchievementValid = function(id) {

                gProgress.mask($scope);
               
                $http.get(apiConfig.serverPath +'api/achievement/valid/' + id)
                  .success(function (data, status) {
                    gProgress.unmask($scope);

                    $scope.achieve.ValidStateName = getValidStateName(1);
                    $scope.achieve.ValidationComment = null;
                    $scope.achieve.ValidationState = 1;

                       $mdToast.show(
                        $mdToast.simple()
                        .content('Корректность данных подтверждена')
                        .position('top'));

                        if($scope.isModerating){
                            window.loadAchievements();
                        } else{
                            $scope.loadAchievements();
                        }
                       

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
            }

            $scope.setAchievementInvalid = function(id, force) {

                if(force != true){
                     force = false;
                 }
  
                 gProgress.mask($scope);
  
                 var msg = {};
                 msg.Message = $scope.achieve.ValidationComment;
  
                $http.put(apiConfig.serverPath +'api/students/setachinvalid/' + id + '/' + force , msg)
                 .success(function (data, status) {
                    gProgress.unmask($scope);
  
                    $scope.achieve.ValidStateName = getValidStateName(2);
                    $scope.achieve.ValidationState = 2;
                    
                    if($scope.isModerating){
                        window.loadAchievements();
                    } else{
                        $scope.loadAchievements();
                    }
  
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
                                  $scope.setAchievementInvalid(id,true)
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
                

            $scope.delete =  function (item) {


                $http.delete(apiConfig.serverPath +'api/achievement/' + item.Id)
                .success(function (data, status) {

                     $mdToast.show(
                        $mdToast.simple()
                        .content('Достижение успешно удалено')
                        .position('top'));

                        $scope.loadAchievements();

                     //  $mdDialog.hide();

                     $scope.close();


                  }).error(function (error) {
                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
            }

            $scope.close = function(){
               //$mdDialog.hide();
                $("#achievementform").hide();
                $(".achievementtoolbar").show();

            };

            $scope.save = function(){
                var data = $scope.achieve;
                console.log(data);

                if(data.FilesArr && data.FilesArr.length > 0) {
                    data.Files = '' + data.FilesArr;
                }


                if(data.Id)
                {

                     $http.put(putUrl + data.Id, data)
                        .then(function(resp){
                            //копируем значения из респонза в модель
                            angular.extend($scope.achieve, resp.data);

                            $mdToast.show(
                            $mdToast.simple()
                            .content('Достижение успешно отредактировано')
                            .position('top'));

                              $mdDialog.hide();

                              if($scope.isModerating){
                                window.loadAchievements();
                                } else{
                                $scope.loadAchievements();
                               }

                              $scope.achieve.ValidStateName = getValidStateName(0);
                              
                              

                           // $scope.loadAchievements()
                           // listScope.loadAchievements();
                        }, function(e){
                            console.log(e);
                        });


                } else {

                    $http.post(postUrl, data)
                        .then(function(resp){
                            //копируем значения из респонза в модель
                            angular.extend($scope.achieve, resp.data);

                            $mdToast.show(
                            $mdToast.simple()
                            .content('Достижение успешно добавлено')
                            .position('top'));

                             $mdDialog.hide();

                             $scope.loadAchievements();

                             $scope.achieve.ValidStateName = getValidStateName(0);

                           // $scope.loadAchievements()
                           // listScope.loadAchievements();
                        }, function(e){
                            console.log(e);
                        });
                }
            };

            $scope.filesUploadClick = function () {

               // if(this.mouseActive)
                //{
                   $('#achieveFileInput').click();
               // }
            }

            var keynum, lines = 1;

            $scope.limitLines =function(obj, e) {
                // IE
                if(window.event) {
                  keynum = window.event.keyCode;

                if(keynum == 13) {
                    if(lines == 3) {
                           window.event.preventDefault();
                           return false;
                    }else{
                           return true;
                    }
                }
              }
           }



           ///

            // uploader for attachments

                var attachUploader = $scope.attachUploader = new FileUploader({
                    url: apiConfig.serverPath +'api/file'
                });

                attachUploader.onAfterAddingFile = function(fileItem)  {
                    var t = this;

                     gProgress.mask($scope)

                    fileItem.upload();
                };


                attachUploader.onCompleteItem = function(fileItem, response, status, headers) {

                   gProgress.unmask($scope);


                    if(status == 200)
                    {
                        $scope.achieve.ProofFile = response;
                        $scope.achieve.ProofList = [];
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


            $scope.attachClick = function() {
                 if(this.mouseActive)
                {
                   $("#GrantAttachementsFileUploader").click();
                 }
              }



           ///



            //todo Вынести это дело в один общий модуль
            var uploader = $scope.achieveUploader = new FileUploader({
                url: apiConfig.serverPath + 'api/filehighres'
            });

            // CALLBACKS

            uploader.onWhenAddingFileFailed = function (item /*{File|FileLikeObject}*/, filter, options) {
                console.info('onWhenAddingFileFailed', item, filter, options);
            };

            uploader.onAfterAddingFile = function (fileItem) {
                var t = this;


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

                if(t.queue.length > 1)
                {
                    t.removeFromQueue(t.queue[0]);
                }

                gProgress.mask($scope);

                fileItem.upload();
            };

            uploader.onCompleteItem = function (fileItem, response, status, headers) {

                 gProgress.unmask($scope);

                if (status == 200) {
                    $scope.achieve.FilesArr = [];
                    $scope.achieve.FilesArr.push(response);
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
        }
    ])
    .controller("AchievementFilesController", ["$scope", "$mdDialog", "$http", "API_CONFIG", "FileUploader",
        function ($scope, $mdDialog, $http, apiConfig, FileUploader, id, listScope) {
            $scope.files = this.achieve.FilesList;
            $scope.deleteFile = function (item) {

            };

            $scope.delete = function () {

            };

            $scope.openImage = function (image, $event) {
                $scope.currentImage = image;
                $mdDialog.show({
                    controller: 'AchievementFilesDialogController',
                    templateUrl: 'app/grant/g-user-profile/g-user-scan-dialog.tmpl.html',
                    clickOutsideToClose: true,
                    focusOnOpen: true,
                    targetEvent: $event,
                    locals: {
                        image: image
                    }
                });
            };
            $scope.filesUploadClick = function () {
                $("#achieveFileInput").click();
            };
        }])
        .controller('AchievementFilesDialogController', function ($scope, $mdDialog, image) {
            $scope.currentImage = image;

            $scope.closeDialog = function () {
                $mdDialog.cancel();
            };
        });
