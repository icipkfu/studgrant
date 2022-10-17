'use strict'

angular.module("g-user-profile")
.factory("UserFileUploader", [
        'FileUploader', "API_CONFIG", function (fileUploader, apiConfig) {

            function userFilesUploader(fileProp, filesProp, multiple) {
                var _pinfoUploader = new FileUploader({
                    url: apiConfig.serverPath + 'api/file'
                });
                _pinfoUploader.filters.push({
                    name: 'imageFilter',
                    fn: function (item /*{File|FileLikeObject}*/, options) {
                        var type = '|' + item.type.slice(item.type.lastIndexOf('/') + 1) + '|';
                        return '|jpg|png|jpeg|bmp|gif|'.indexOf(type) !== -1;
                    }
                });

                // CALLBACKS

                _pinfoUploader.onWhenAddingFileFailed = function (item /*{File|FileLikeObject}*/, filter, options) {
                    console.info('onWhenAddingFileFailed', item, filter, options);
                };

                _pinfoUploader.onAfterAddingFile = function (fileItem) {
                    if (!multiple) {
                        var t = this;

                        if (t.queue.length > 1) {
                            t.removeFromQueue(t.queue[0]);
                        }
                    }
                    fileItem.upload();
                };

                _pinfoUploader.onCompleteItem = function (fileItem, response, status, headers) {

                    if (status == 200) {
                        fileProp = response;

                        if (multiple && !filesProp) {
                            filesProp = [];
                        }
                        filesProp.push(response);
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

            return {
                getUploader: userFilesUploader
            };

        }
]);