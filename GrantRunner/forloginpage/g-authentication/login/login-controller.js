'use strict';

angular.module('g-authentication')
.controller('LoginController', ["$scope", "$state", "authenticationResource", "API_CONFIG", "$http", "$mdToast", "gProgress", "$window", '$stateParams',
        function ($scope, $state, authenticationResource, API_CONFIG, $http, $mdToast, gProgress, $window, $stateParams) {
            // create blank user variable for login form
            //$scope.user = {
            //    email: 'info@oxygenna.com',
            //    password: 'demo'
            //};

			var userData = {
				  isAuthenticated: false,
				  username: '',
				  bearerToken: '',
				  expirationDate: null,
			};

            if($stateParams.email){
                $scope.user = {
                	email : $stateParams.email
                };
            }

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
//			  $window.sessionStorage.setItem('token', userData.bearerToken);
			
              createCookie('authToken', userData.bearerToken, 365);
            }

			$scope.getUserData = function(){
			  return userData;
			};

			$scope.goForgot = function() {

				 $state.go('authentication.forgot');
			}

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
					  // $state.go('admin-panel.default.g-user-profile');
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

            // controller to handle login check
            $scope.loginClick = function () {
                gProgress.mask($scope);
                $http({
                    method: 'POST',
                    url: API_CONFIG.serverPath + 'api/Account/Login',
                    data: $scope.user
                }).
                success(function (data) {
                    gProgress.unmask($scope);

                    API_CONFIG.currUserId = data.CurrentUserId;
                    API_CONFIG.currStudentId = data.CurrentStudentId;

                    var pass = $scope.login.password.$viewValue;

                    if(pass && pass.length > 0){
                        pass = pass.trim();
                    }

					$scope.authenticate($scope.login.email.$viewValue, pass);


                }).
                error(function (resp) {
                    gProgress.unmask($scope);

                    var msg = 'Не удается выполнить вход';

                    if(resp && resp.Message){
                        msg =  resp.Message;
                    }
                    
                    $mdToast.show(
                        $mdToast.simple()
                        .content(msg)
                        .position('top')
                        .hideDelay(5000)
                    );
                });
                //authenticationResource.$post()
                //$state.go('admin-panel.default.introduction');
            };
        }]);
