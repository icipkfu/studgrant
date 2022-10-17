'use strict';

/**
 * @ngdoc function
 * @name EmailToolbarController
 * @module triAngularEmail
 * @kind function
 *
 * @description
 *
 * Handles all actions for email toolbar
 */

angular.module('triAngular')
.controller('DefaultToolbarController', function ($scope, $translate, $state, $element, $mdUtil, $mdSidenav, $timeout, SideMenu, APP, API_CONFIG, $window, $http) {
    $scope.menu = SideMenu.getMenu();


    $scope.toolbarTypeClass = function() {
        return $scope.extraClass;
    };

    $scope.$on('$stateChangeStart', initToolbar);

    function initToolbar() {
        $element.css('background-image', '');

        if($state.current.data !== undefined) {
            if($state.current.data.toolbar !== undefined) {
                if($state.current.data.toolbar.extraClass !== false) {
                    $scope.extraClass = $state.current.data.toolbar.extraClass;
                }

                if($state.current.data.toolbar.background) {
                    $element.css('background-image', 'url(' + $state.current.data.toolbar.background + ')');
                }
            }
        }
    }


    $scope.loadMenu = function() {


    var profileItem =  {
                             name: "Профиль",
                            icon: "icon-person",
                            type: "link",
                            state: "admin-panel.default.g-user-profile",
                            priority: 1
                        };

     var vuzItem =  {
                         name: "Образовательные организации",
                         icon: "icon-account-balance",
                         type: 'link',
                         state: "admin-panel.default.g-university-list",
                         priority: 2
                     };

      var timelineItem =  {
                             name: "Лента событий",
                             icon: "icon-access-time",
                             type: "link",
                             state: "admin-panel.default.g-timeline",
                             priority: 3
                         };

      var grantItem =  {
                                name: "Конкурсы",
                                icon: "icon-star-outline",
                                type: 'link',
                                state: "admin-panel.default.g-grants-list",
                                priority: 4
                        };

    var moderatorsItem =  {
                            name: "Модераторы",
                            icon: "icon-security",
                            type: 'link',
                            state: "admin-panel.default.g-user-moderators",
                            priority: 5
                        };

     var usersItem =  {
                        name: "Пользователи",
                        icon: "icon-people",
                        type: 'link',
                        state: "admin-panel.default.g-user-users",
                        priority: 6
                    };


    var settingsItem =  {
         name: "Настройки",
         icon: "icon-settings",
         type: 'link',
         state: "admin-panel.default.g-user-settings",
         priority: 7
    };

     var contactsItem =  {
         name: "Контакты",
         icon: "icon-quick-contacts-dialer",
         type: 'link',
         state: "admin-panel.default.g-contacts",
         priority: 7
    };

    var bankFilialItem =  {
        name: "Филиалы банка",
        icon: "icon-attach-money",
        type: 'link',
        state: "admin-panel.default.g-bankfilials",
        priority: 7
   };

      if(window.menu && window.menu.length==0){
         menu.push(profileItem);



         if($scope.isAdmin || $scope.isModerator || $scope.isAnyCurator)
         {
             menu.push(vuzItem);
         }

         if($scope.isAdmin)
         {
            menu.push(timelineItem);
            menu.push(settingsItem);
         }
        
         menu.push(grantItem);


         if($scope.isAdmin)
         {
            menu.push(moderatorsItem);
         }

         if($scope.isAdmin || $scope.isModerator)
         {
            menu.push(usersItem);

         }

         menu.push(contactsItem);

         if($scope.isAdmin)
         {
             menu.push(bankFilialItem);
         }
      }
      else{

         if(window.oldMenu) {} else {window.oldMenu = [];}


        if(window.oldMenu.length == 0) {

         window.oldMenu = [];

         oldMenu.push(profileItem);

         if($scope.isAdmin || $scope.isModerator || $scope.isAnyCurator)
         {
            oldMenu.push(vuzItem);
         }

         if($scope.isAdmin)
         {
            oldMenu.push(timelineItem);
            oldMenu.push(settingsItem);
         }
        
         oldMenu.push(grantItem);

          if($scope.isAdmin)
         {
             oldMenu.push(moderatorsItem);
         }

         if($scope.isAdmin || $scope.isModerator)
         {
            oldMenu.push(usersItem);
         }

         oldMenu.push(contactsItem);

         if($scope.isAdmin)
         {
             oldMenu.push(bankFilialItem);
         }


        }

      }


    }

      $scope.loadRole = function () {

              $http.get(API_CONFIG.serverPath + "api/students/getroleinfo")
                .then(function(resp){
                    $scope.roleInfo = resp.data;


                    if($scope.roleInfo) {
                       $scope.isAdmin = ($scope.roleInfo.Role == 5);
                    } else {
                      return
                    }

                    if($scope.roleInfo) {
                       $scope.isModerator = ($scope.roleInfo.Role == 4);
                    }

                    if($scope.roleInfo.UniversCurator.length >0 ) {
                       $scope.isAnyCurator = true;
                    }

                    if($scope.roleInfo.GrantsAdmin.length >= 0){
                        $scope.isGrantAdmin = true;
                    }


                    $scope.loadMenu();

                  });

              }


    window.loadUserInfoBar = $scope.loadInfo = function() {
               // gProgress.mask(e);


                $http.get(API_CONFIG.serverPath +'api/Students/1')
                .success(function (data, status) {
                    // gProgress.unmask(e);

                     $scope.userInfo = {
                        Name : data.LastName,
                        ImageLink : data.ImageLink

                     };

                     $scope.loadRole();

                  }).error(function (error) {

                     $scope.userInfo = {

                        Name : 'Новый пользователь',
                        ImageLink : 'assets/images/avatars/avatar-5.png'

                     };
                  });
              
            };

     $scope.loadInfo();
    initToolbar();

    $scope.switchLanguage = function(languageCode) {
        $translate.use(languageCode).then(function() {
        });
    };

    $scope.openSideNav = function(navID) {
        $mdUtil.debounce(function(){
            $mdSidenav(navID).toggle();
        }, 300)();
    };

    $scope.toggleNotificationsTab = function(tab) {
        $scope.$parent.$broadcast('triSwitchNotificationTab', tab);
        $scope.openSideNav('notifications');
    };

    $scope.profile = function() {
        $state.go('admin-panel.default.profile');
    };

    $scope.logout = function () {
        API_CONFIG.currUserId = null;
        API_CONFIG.currStudentId = null;
		
		//if ($window.sessionStorage.getItem('token')) {
		//     $window.sessionStorage.removeItem('token');
		//}		
        $http.defaults.headers.common.Authorization = null;


        $state.go('authentication.login');
    };

    $scope.$on('newMailNotification', function(){
        $scope.emailNew = true;
    });        

    // until we can get languages from angular-translate use APP constant
    $scope.languages = APP.languages;
});