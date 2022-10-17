angular.module("g-user-profile")
    .controller("IncomeController", [
        "$scope", "$http", "API_CONFIG", "$q", "gProgress", "$mdToast", "FileUploader", "$mdDialog",
        function (e, $http, apiConfig, q, gp, $mdToast, FileUploader, $mdDialog) {
            var getUrl = apiConfig.serverPath + "api/income/1",
                postUrl = getUrl;

            $http.get(getUrl).then(function (resp) {
               e.income.Files = resp.data;
            });

            e.openImage = function (image, $event) {
                e.currentImage = image;
                $mdDialog.show({
                    controller: 'IncomeDialogController',
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
                $http.get(apiConfig.serverPath + "api/income/delete/" + item.Hash)
                .then(function (resp) {
                    e.income.Files = resp.data;

                    e.loadIsIncomeFilled();

                    e.loadCurrent();

                     if(e.user.StudentBookState == 1){
                        e.loadCurrent();
                     }

                });
            }



            e.askDelete = function(item){


              if(e.income.Files.length < 1){

                 $mdDialog.show(
                                $mdDialog.confirm()
                                .title('Удаление скана справки о доходах')
                                .content("Нельзя удалить последний скан. Если требуется его удалить - загрузите сначала новый")
                                .ok("Ок")).then(function(result){


                            });

                    return;

              }


                 if(e.user.IncomeState == 1)
                        {
                            $mdDialog.show(
                                $mdDialog.confirm()
                                .title('Изменение данных о доходе')
                                .content("Ваши данные проверены, любое изменение потребует новой проверки! Продолжить?")
                                .ok("Да")
                                .cancel("Отмена")).then(function(result){

                                  if(result){
                                      e.delete(item);
                                  }
                            });
                        } else{
                             e.delete(item);
                        }
            }

            e.showFiles = null;

            var incomeUploader = e.incomeUploader = new FileUploader({
                url: apiConfig.serverPath + 'api/filehighres'
            });

            e.incomeImageClick = function() {
               if(e.user.Income)
                {
                   $("#incomeFileInput").click();
                }
            }

            e.addBookFile = function(hash)
            {
                gp.mask(e);
                 $http.get(apiConfig.serverPath + "api/income/add/" + hash)
                .then(function (resp) {
                    gp.unmask(e);
                    e.income.Files = resp.data;
                     e.loadIsIncomeFilled();

                     e.loadCurrent();

                      if(e.user.StudentBookState == 1){
                        e.loadCurrent();
                      }
                });
            }

            // CALLBACKS

            e.incomeUploader.onWhenAddingFileFailed = function (item /*{File|FileLikeObject}*/, filter, options) {
                //console.info('onWhenAddingFileFailed', item, filter, options);
            };

            e.incomeUploader.onAfterAddingFile = function (fileItem) {


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


                 if(e.user.IncomeState == 1)
                        {
                            $mdDialog.show(
                                $mdDialog.confirm()
                                .title('Изменение данных справки о доходах')
                                .content("Ваши данные проверены, любое изменение потребует новой проверки! Продолжить?")
                                .ok("Да")
                                .cancel("Отмена")).then(function(result){

                                  if(result){

                                    gp.mask(e);
                                    fileItem.upload();

                                  }
                            });
                        } else{

                             gp.mask(e);
                            fileItem.upload();

                        }

            };

            e.incomeUploader.onCompleteItem = function (fileItem, response, status, headers) {

                gp.unmask(e);

               if (status == 200) {
                    e.addBookFile(response);
                   e.incomeUploader.clearQueue();

                  $mdToast.show(
                        $mdToast.simple()
                        .content('Файл успешно загружен и сохранен')
                        .position('top'));
                }
                else {
                    $mdToast.show(
                    $mdToast.simple()
                    .content(response)
                    .theme("error-toast")
                    .position('top'));
                }
            };


            e.updateIncomeClick = function(data){

                $http.put(apiConfig.serverPath +'api/income/set/'+ data)
                .then(function(resp){

                   // angular.extend(e.user, resp.data);

                   e.loadCurrent();

                    $mdToast.show(
                    $mdToast.simple()
                    .content('Информация о доходе сохранена')
                    .position('top'));

                      $mdDialog.hide();

                   // $scope.loadAchievements()
                   // listScope.loadAchievements();
                }, function(e){
                    console.log(e);
                });
            }

            e.loadIsIncomeFilled = function()
            {
               $http.get(apiConfig.serverPath +'api/students/isincomeFilled/'+ 0)
                    .success(function (data, status) {

                        e.isincomeFilled = data;

                  }).error(function (error) {

                    $mdToast.show(
                        $mdToast.simple()
                        .content(error.data.Message)
                        .theme("error-toast")
                        .position('top'));
                  });
            }
            e.loadIsIncomeFilled();

           
            e.loadCurrent();
        }
    ])
    .controller('IncomeDialogController', function ($scope, $mdDialog, image) {
        $scope.currentImage = image;

        $scope.closeDialog = function () {
            $mdDialog.cancel();
        };
    });
