angular.module("g-user-profile-password", [])
    .controller("UserPasswordController", [
        "$scope", "$http", "API_CONFIG", "$mdToast", "UserProfileService",
        function ($scope, $http, apiConfig, $mdToast, userProfileService) {
            $scope.updatePasswordClick = function() {
                $http({
                    method: 'POST',
                    url: apiConfig.serverPath + "api/Account/ChangePassword/",
                    data: $scope.password
                }).
            success(function (data) {
                $mdToast.show(
                    $mdToast.simple()
                    .content("Пароль успешно изменен")
                    .position('top')
                );

                $scope.password.NewPassword = "";
                $scope.password.OldPassword = "";
                $scope.password.ConfirmPassword = "";

                 userProfileService.goTab(0);
            }).
            error(function (data) {

                var msg = "При смене пароля произошли ошибки";

                if(data && data.ModelState && data.ModelState[""] == "Incorrect password."){
                    if(data.ModelState[""] == "Incorrect password."){
                        msg = "Неверно указан старый пароль";
                    }
                }


                $mdToast.show(
                    $mdToast.simple()
                    .content(msg)
                    .position('top')
                    .hideDelay(5000)
                    .theme("error-toast")
                );
            });
        }
    }
]);
