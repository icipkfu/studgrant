angular.module("g-user-profile")
    .controller("RecordBookController", [
        "$scope", "$http", "API_CONFIG", "$q", "gProgress", "$mdToast", "FileUploader", "$mdDialog",
        function (e, $http, apiConfig, q, gp, $mdToast, FileUploader, $mdDialog) {
            var getUrl = apiConfig.serverPath + "api/recordBook/1",
                postUrl = getUrl;

            $http.get(getUrl).then(function (resp) {
               e.recordbook.Files = resp.data;
            });

            e.openImage = function (image, $event) {
                e.currentImage = image;
                $mdDialog.show({
                    controller: 'RecordBookDialogController',
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
                $http.get(apiConfig.serverPath + "api/recordBook/delete/" + item.Hash)
                .then(function (resp) {
                    e.recordbook.Files = resp.data;

                    e.loadIsRecordBookFilled();

                    e.loadCurrent();

                     if(e.user.StudentBookState == 1){
                        e.loadCurrent();
                     }

                });
            }



            e.askDelete = function(item){


              if(e.recordbook.Files.length < 3){

                 $mdDialog.show(
                                $mdDialog.confirm()
                                .title('Удаление скана зачетной книжки')
                                .content("Нельзя удалить последние 2 скана зачетной книжки. Если требуется их удалить - загрузите сначала новые")
                                .ok("Ок")).then(function(result){


                            });

                    return;

              }


                 if(e.user.StudentBookState == 1)
                        {
                            $mdDialog.show(
                                $mdDialog.confirm()
                                .title('Изменение данных зачетной книжки')
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

            var bookUploader = e.bookUploader = new FileUploader({
                url: apiConfig.serverPath + 'api/filehighres'
            });

            e.bookImageClick = function() {
              //   if(this.mouseActive)
              //  {
                   $("#recordBookFileInput").click();
              //  }
            }

            e.addBookFile = function(hash)
            {
                gp.mask(e);
                 $http.get(apiConfig.serverPath + "api/recordBook/add/" + hash)
                .then(function (resp) {
                    gp.unmask(e);
                    e.recordbook.Files = resp.data;
                     e.loadIsRecordBookFilled();

                     e.loadCurrent();

                      if(e.user.StudentBookState == 1){
                        e.loadCurrent();
                      }
                });
            }

            // CALLBACKS

            e.bookUploader.onWhenAddingFileFailed = function (item /*{File|FileLikeObject}*/, filter, options) {
                //console.info('onWhenAddingFileFailed', item, filter, options);
            };

            e.bookUploader.onAfterAddingFile = function (fileItem) {


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


                 if(e.user.StudentBookState == 1)
                        {
                            $mdDialog.show(
                                $mdDialog.confirm()
                                .title('Изменение данных зачетной книжки')
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

            e.bookUploader.onCompleteItem = function (fileItem, response, status, headers) {

                gp.unmask(e);

               if (status == 200) {
                    e.addBookFile(response);
                   e.bookUploader.clearQueue();

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

            e.loadIsRecordBookFilled = function()
            {
               $http.get(apiConfig.serverPath +'api/students/isrecordbookFilled/'+ 0)
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
            e.loadIsRecordBookFilled();
        }
    ])
    .controller('RecordBookDialogController', function ($scope, $mdDialog, image) {
        $scope.currentImage = image;

        $scope.closeDialog = function () {
            $mdDialog.cancel();
        };
    });
