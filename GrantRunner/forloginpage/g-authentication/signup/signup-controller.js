'use strict';

angular.module('g-authentication')
    .controller('SignupController',// '$scope', '$state', '$mdToast', '$http', '$filter', 'API_CONFIG', 'g_progress', '$window'
        function ($scope, $state, $mdToast, $http, $filter, API_CONFIG, gProgress, $window) {

            function mask() {
                $scope.inProgress = true;
            };

            function unmask() {
                $scope.inProgress = false;
            }

            var userData = {
                  isAuthenticated: false,
                  username: '',
                  bearerToken: '',
                  expirationDate: null,
            };

            var createCookie = function(name, value, days) {
                var expires;
                if (days) {
                    var date = new Date();
                    date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
                    expires = "; expires=" + date.toGMTString();
                }
                else {
                    expires = "";
                }
                document.cookie = name + "=" + value + expires + "; path=/";
            }

            function setHttpAuthHeader() {
              $http.defaults.headers.common.Authorization = 'Bearer ' + userData.bearerToken;
              //$window.sessionStorage.setItem('token', userData.bearerToken);

              createCookie('authToken', userData.bearerToken, 365);
            }

            $scope.getUserData = function(){
              return userData;
            };

            $scope.authenticate = function(username, password, successCallback, errorCallback) {
                  var config = {
                    method: 'POST',
                    url: API_CONFIG.serverPath +'token',
                    headers: {
                      'Content-Type': 'application/x-www-form-urlencoded',
                    },
                    data: 'grant_type=password&username=' + username + '&password=' + password,
                  };

                  $http(config)
                    .success(function(data) {
                      userData.isAuthenticated = true;
                      userData.username = data.userName;
                      userData.bearerToken = data.access_token;
                      userData.expirationDate = new Date(data['.expires']);
                      setHttpAuthHeader();
                      if (typeof successCallback === 'function') {
                        successCallback();
                      }

                        window.location.href = 'https://studgrant.ru/my';
                       //$state.go('admin-panel.default.g-user-profile');
                    })
                    .error(function(data) {
                      if (typeof errorCallback === 'function') {
                        if (data.error_description) {
                          errorCallback(data.error_description);
                        } else {
                          errorCallback('Unable to contact server; please, try again later.');
                        }
                      }
                    });
            };

            $scope.signupClick = function() {
                gProgress.mask($scope);

                $http({
                        method: 'POST',
                        url: API_CONFIG.serverPath + 'api/Account/Register',
                        data: $scope.user
                    }).
                    success(function(data) {

                            $scope.authenticate($scope.user.email, $scope.user.password);

                            API_CONFIG.currUserId = data.CurrentUserId;

                        $mdToast.show(
                            $mdToast.simple()
                            .content($filter('translate')('Регистрация прошла успешно.'))
                            .position('top'));                           



                    }).
                    error(function(resp) {
                        gProgress.unmask($scope);
                        $mdToast.show(
                            $mdToast.simple()
                            .content($filter('translate')(resp.Message))
                            .position('top')
                            .hideDelay(1000)
                        );
                    });
            };
        });