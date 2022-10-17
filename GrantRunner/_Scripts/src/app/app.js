'use strict';

angular.module('triAngular', [
    // inject angular modules
    'ngAnimate', 'ngCookies', 'ngTouch', 'ngSanitize', 'ngMessages', 'ngMaterial',
    // inject extra 3rd party angular modules
    'ui.router', 'pascalprecht.translate',
     'LocalStorageModule', 'googlechart', 'chart.js', 'linkify', 'ui.calendar', 'angularMoment', 'textAngular', 'uiGmapgoogle-maps', 'hljs', 'ngResource',
    // inject our own triangular modules
    // 'triAngularIntroduction',
    'triAngularUI',
    //'triAngularAuthentication',
    'triAngularDashboards',
    //'triAngularEmail',
    'triAngularMenuLevels',
    'triAngularElements',
    'triAngularForms',
    'triAngularCharts',
    'triAngularMaps',
    'triAngularExtras',
    'triAngularTodo',
    'triAngularCalendar',
    'angularFileUpload',

    //grant modules
   //
    'g-universities', 'g-grants',
    "g-user-profile",
    "g-user-profile-password",
    "g-user-profile-achievement",
    "g-user-profile-personal-info",
    "g-authentication",
    "g-progress-module",    
    "ImageCropper",
    "g-timeline",
    "g-contacts",
    "g-bankfilials"

])
.constant('APP', {
    name: 'Грант 2.0',
    logo: '../../assets/images/newlogo.png',
    version: '1.4.0',
    languages: [{
        name: 'LANGUAGES.ENGLISH',
        key: 'en'
    },{
        name: 'LANGUAGES.FRENCH',
        key: 'fr'
    }],
    defaultSkin: 'cyan-cloud'
})
.constant('API_CONFIG', {
    'url': 'http://triangular-api.oxygenna.com/',
    'serverPath': 'https://studgrant.ru/',
    //TODO убрать как будет настроен менеджер пользователей
    'currUserId': null,
    'currStudentId' : null
})
/**
 *  SETUP TRANSLATIONS
 */
.config(function ($stateProvider, $urlRouterProvider, $translateProvider, $translatePartialLoaderProvider, localStorageServiceProvider, APP) {
    /**
     *  each module loads its own translation file - making it easier to create translations
     *  also translations are not loaded when they aren't needed
     *  each module will have a il8n folder that will contain its translations
     */
    //$translateProvider.useLoader('$translatePartialLoader', {
    //    urlTemplate: '{part}/il8n/{lang}.json'
    //});

    //$translatePartialLoaderProvider.addPart('app');

    // make sure all values used in translate are sanitized for security
    //$translateProvider.useSanitizeValueStrategy('sanitize');

    // cache translation files to save load on server
    //$translateProvider.useLoaderCache(true);

    // get languages set in APP constant
    //var languageKeys = [];
    //for(var lang in APP.languages) {
    //    languageKeys.push(APP.languages[lang].key);
    //}
    /**
     *  try to detect the users language by checking the following
     *      navigator.language
     *      navigator.browserLanguage
     *      navigator.systemLanguage
     *      navigator.userLanguage
     */
    //$translateProvider
    //.registerAvailableLanguageKeys(languageKeys, {
    //    'en_US': 'en',
    //    'en_UK': 'en'
    //})
    //.use('en');

    //// store the users language preference in a cookie
    //$translateProvider.useLocalStorage();

        console.realWarn = console.warn;
    console.warn = function (message) {
        if (message.indexOf("ARIA") == -1) {
            console.realWarn.apply(console, arguments);
        }
    };


    $translateProvider.useSanitizeValueStrategy('sanitize');


    // setup public states & routes
    $stateProvider
    .state('admin-panel', {
        abstract: true,
        templateUrl: 'app/layouts/admin-panel/admin-panel.tmpl.html',
        data: {
            toolbar: {
                extraClass: '',
                background: false,
                shrink: true
            }
        }
    })

    .state('admin-panel.default', {
        abstract: true,
        views: {
            sidebarLeft: {
                templateUrl: 'components/sidebar-left/sidebar-left.tmpl.html',
                controller: 'SidebarLeftController'
            },
            toolbar: {
                templateUrl: 'components/toolbars/default.tmpl.html',
                controller: 'DefaultToolbarController'
            },
            content: {
                template: '<div id="admin-panel-content-view" flex ui-view></div>'
            }
        }
    })

    .state('admin-panel-no-scroll', {
        abstract: true,
        templateUrl: 'app/layouts/no-scroll/no-scroll.tmpl.html',
        data: {
            toolbar: {
                extraClass: '',
                background: false,
                shrink: true
            },
        }
    })

    .state('admin-panel-no-scroll.default', {
        abstract: true,
        views: {
            sidebarLeft: {
                templateUrl: 'components/sidebar-left/sidebar-left.tmpl.html',
                controller: 'SidebarLeftController'
            },
            toolbar: {
                templateUrl: 'components/toolbars/default.tmpl.html',
                controller: 'DefaultToolbarController'
            },
            content: {
                template: '<div flex ui-view layout="column"></div>'
            }
        },
    })

    .state('404', {
        url: '/404',
        templateUrl: '404.tmpl.html',
        controller: function($scope, $state, APP) {
            $scope.app = APP;

            $scope.goHome = function() {
                $state.go('admin-panel.default.g-user-profile');
            };
        }
    })

    .state('500', {
        url: '/500',
        templateUrl: '500.tmpl.html',
        controller: function($scope, $state, APP) {
            $scope.app = APP;

            $scope.goHome = function() {
                $state.go('authentication.login');
            };
        }
    });

    // set default routes when no path specified
    $urlRouterProvider.when('', '/user-profile/profile/');
    $urlRouterProvider.when('/', '/user-profile/profile/');

    // always goto 404 if route not found
    $urlRouterProvider.otherwise('/404');

    // set prefix for local storage
    localStorageServiceProvider
    .setPrefix('triAngular')
    .setStorageType('sessionStorage');
})
/**
 *  PALETTES & THEMES & SKINS oh my.....
 */
.config(function ($mdThemingProvider, triThemingProvider, triSkinsProvider, APP) {
    /**
     *  PALETTES
     */
    $mdThemingProvider.definePalette('white', {
        '50': 'ffffff',
        '100': 'ffffff',
        '200': 'ffffff',
        '300': 'ffffff',
        '400': 'ffffff',
        '500': 'ffffff',
        '600': 'ffffff',
        '700': 'ffffff',
        '800': 'ffffff',
        '900': 'ffffff',
        'A100': 'ffffff',
        'A200': 'ffffff',
        'A400': 'ffffff',
        'A700': 'ffffff',
        'contrastDefaultColor': 'dark'    // whether, by default, text (contrast)
    });

    $mdThemingProvider.definePalette('black', {
        '50': 'e1e1e1',
        '100': 'b6b6b6',
        '200': '8c8c8c',
        '300': '646464',
        '400': '4d4d4d',
        '500': '3a3a3a',
        '600': '2f2f2f',
        '700': '232323',
        '800': '1a1a1a',
        '900': '121212',
        'A100': 'ffffff',
        'A200': 'ffffff',
        'A400': 'ffffff',
        'A700': 'ffffff',
        'contrastDefaultColor': 'light'    // whether, by default, text (contrast)
    });

    /**
     *  SKINS
     */

    // CYAN CLOUD SKIN
    triThemingProvider.theme('cyan')
    .primaryPalette('cyan')
    .accentPalette('amber')
    .warnPalette('deep-orange');

    triThemingProvider.theme('white-cyan')
    .primaryPalette('white')
    .accentPalette('cyan', {
        'default': '500'
    })
    .warnPalette('deep-orange');

    triSkinsProvider.skin('cyan-cloud', 'Cyan Cloud')
    .sidebarTheme('cyan')
    .toolbarTheme('white-cyan')
    .logoTheme('cyan')
    .contentTheme('cyan');

    // RED DWARF SKIN
    triThemingProvider.theme('red')
    .primaryPalette('red')
    .accentPalette('amber')
    .warnPalette('purple');

    triThemingProvider.theme('white-red')
    .primaryPalette('white')
    .accentPalette('red', {
        'default': '500'
    })
    .warnPalette('purple');

    triSkinsProvider.skin('red-dwarf', 'Red Dwarf')
    .sidebarTheme('red')
    .toolbarTheme('white-red')
    .logoTheme('red')
    .contentTheme('red');

    // PLUMB PURPLE SKIN
    triThemingProvider.theme('purple')
    .primaryPalette('purple')
    .accentPalette('deep-orange')
    .warnPalette('amber');

    triThemingProvider.theme('white-purple')
    .primaryPalette('white')
    .accentPalette('purple', {
        'default': '400'
    })
    .warnPalette('deep-orange');

    triSkinsProvider.skin('plumb-purple', 'Plumb Purple')
    .sidebarTheme('purple')
    .toolbarTheme('white-purple')
    .logoTheme('purple')
    .contentTheme('purple');

    // DARK KNIGHT SKIN
    triThemingProvider.theme('dark')
    .primaryPalette('black')
    .accentPalette('amber')
    .warnPalette('deep-orange')
    .dark();

    triSkinsProvider.skin('dark-knight', 'Dark Knight')
    .sidebarTheme('dark')
    .toolbarTheme('dark')
    .logoTheme('dark')
    .contentTheme('dark');

    // BATTLESHIP GREY SKIN
    triThemingProvider.theme('blue-grey')
    .primaryPalette('blue-grey')
    .accentPalette('amber')
    .warnPalette('orange');

    triThemingProvider.theme('white-blue-grey')
    .primaryPalette('white')
    .accentPalette('blue-grey', {
        'default': '400'
    })
    .warnPalette('orange');

    triSkinsProvider.skin('battleship-grey', 'Battleship Grey')
    .sidebarTheme('blue-grey')
    .toolbarTheme('white-blue-grey')
    .logoTheme('blue-grey')
    .contentTheme('blue-grey');

    // ZESTY ORANGE SKIN
    triThemingProvider.theme('orange')
    .primaryPalette('orange' , {
        'default': '800'
    })
    .accentPalette('lime')
    .warnPalette('amber');

    triThemingProvider.theme('white-orange')
    .primaryPalette('white')
    .accentPalette('orange', {
        'default': '500'
    })
    .warnPalette('lime');

    triSkinsProvider.skin('zesty-orange', 'Zesty Orange')
    .sidebarTheme('orange')
    .toolbarTheme('white-orange')
    .logoTheme('orange')
    .contentTheme('orange');


    // INDIGO ISLAND SKIN
    triThemingProvider.theme('indigo')
    .primaryPalette('indigo' , {
        'default': '600'
    })
    .accentPalette('red')
    .warnPalette('lime');

    triSkinsProvider.skin('indigo-island', 'Indigo Island')
    .sidebarTheme('indigo')
    .toolbarTheme('indigo')
    .logoTheme('indigo')
    .contentTheme('indigo');

    // KERMIT GREEN SKIN
    triThemingProvider.theme('light-green')
    .primaryPalette('light-green' , {
        'default': '400'
    })
    .accentPalette('amber')
    .warnPalette('deep-orange');

    triThemingProvider.theme('white-light-green')
    .primaryPalette('white')
    .accentPalette('light-green', {
        'default': '400'
    })
    .warnPalette('deep-orange');

    triSkinsProvider.skin('kermit-green', 'Kermit Green')
    .sidebarTheme('light-green')
    .toolbarTheme('white-light-green')
    .logoTheme('light-green')
    .contentTheme('light-green');


    /**
     *  FOR DEMO PURPOSES ALLOW SKIN TO BE SAVED IN A COOKIE
     *  This overrides any skin set in a call to triSkinsProvider.setSkin if there is a cookie
     *  REMOVE LINE BELOW FOR PRODUCTION SITE
     */
    triSkinsProvider.useSkinCookie(true);

    /**
     *  SET DEFAULT SKIN
     */
    triSkinsProvider.setSkin(APP.defaultSkin);
})
.config(function (ChartJsProvider) {
    // Configure all charts to use material design colors
    ChartJsProvider.setOptions({
        colours: [
            '#4285F4',    // blue
            '#DB4437',    // red
            '#F4B400',    // yellow
            '#0F9D58',    // green
            '#AB47BC',    // purple
            '#00ACC1',    // light blue
            '#FF7043',    // orange
            '#9E9D24',    // browny yellow
            '#5C6BC0'     // dark blue
        ],
        responsive: true,
    });
})

.directive('ngThumb', ['$window', function($window) {
        var helper = {
            support: !!($window.FileReader && $window.CanvasRenderingContext2D),
            isFile: function(item) {
                return angular.isObject(item) && item instanceof $window.File;
            },
            isImage: function(file) {
                var type =  '|' + file.type.slice(file.type.lastIndexOf('/') + 1) + '|';
                return '|jpg|png|jpeg|bmp|gif|'.indexOf(type) !== -1;
            }
        };

        return {
            restrict: 'A',
            template: '<canvas/>',
            link: function(scope, element, attributes) {
                if (!helper.support) return;

                var params = scope.$eval(attributes.ngThumb);

                if (!helper.isFile(params.file)) return;
                if (!helper.isImage(params.file)) return;

                var canvas = element.find('canvas');
                var reader = new FileReader();

                reader.onload = onLoadFile;
                reader.readAsDataURL(params.file);

                function onLoadFile(event) {
                    var img = new Image();
                    img.onload = onLoadImage;
                    img.src = event.target.result;
                }

                function onLoadImage() {
                    var width = params.width || this.width / this.height * params.height;
                    var height = params.height || this.height / this.width * params.width;
                    canvas.attr({ width: width, height: height });
                    canvas[0].getContext('2d').drawImage(this, 0, 0, width, height);
                }
            }
        };
    }])

.directive('onFinishRender', function ($timeout) {
        return {
            restrict: 'A',
            link: function (scope, element, attr) {
                if (scope.$last === true) {
                    $timeout(function () {
                        scope.$emit('ngRepeatFinished');
                    });
                }
            }
        }
    })

.directive('ngEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {

             var rows = event.target.value.split("\n");

             if(event.which === 13) { 

               if(rows.length >=3){ 
                 event.preventDefault();
               }
           }

            if(rows[rows.length -1].length > (71 * (4 - rows.length))){
                event.target.value = event.target.value.substr(0, event.target.value.length - rows[rows.length -1].length + (71 * (4 - rows.length)))
            }
           
        });
    };
})

.directive('validNumber', function() {
  return {
    require: '?ngModel',
    link: function(scope, element, attrs, ngModelCtrl) {
      if(!ngModelCtrl) {
        return; 
      }

      ngModelCtrl.$parsers.push(function(val) {
        if (angular.isUndefined(val)) {
            var val = '';
        }
        var clean = val.replace( /[^0-9]+/g, '');
        if (val !== clean) {
          ngModelCtrl.$setViewValue(clean);
          ngModelCtrl.$render();
        }
        return clean;
      });

      element.bind('keypress', function(event) {
        if(event.keyCode === 32) {
          event.preventDefault();
        }
      });
    }
  };
})

.directive('validCode', function() {
  return {
    require: '?ngModel',
    link: function(scope, element, attrs, ngModelCtrl) {
      if(!ngModelCtrl) {
        return; 
      }

      ngModelCtrl.$parsers.push(function(val) {
        if (angular.isUndefined(val)) {
            var val = '';
        }
        var clean = val.replace( /[^0-9-]+/g, '');
        if (val !== clean) {
          ngModelCtrl.$setViewValue(clean);
          ngModelCtrl.$render();
        }
        return clean;
      });

      element.bind('keypress', function(event) {
        if(event.keyCode === 32) {
          event.preventDefault();
        }
      });
    }
  };
})
.directive("directiveWhenScrolled", function() {
  return function(scope, elm, attr) {
    var raw = elm[0];


    elm.bind('scroll', function() {
      if (raw.scrollTop + raw.offsetHeight >= raw.scrollHeight) {
        scope.$apply(attr.directiveWhenScrolled);
      }
    });
  };
})

.directive('validDate', function() {
  return {
    require: '?ngModel',
    link: function(scope, element, attrs, ngModelCtrl) {
      if(!ngModelCtrl) {
        return; 
      }

      ngModelCtrl.$parsers.push(function(val) {
        if (angular.isUndefined(val)) {
            var val = '';
        }
        var clean = val.replace( /[^0-9.]+/g, '');
        if (val !== clean) {
          ngModelCtrl.$setViewValue(clean);
          ngModelCtrl.$render();
        }
        return clean;
      });

      element.bind('keypress', function(event) {
        if(event.keyCode === 32) {
          event.preventDefault();
        }
      });
    }
  };
})
.run(function ($rootScope, $window, $http, $location, API_CONFIG, $mdToast) {

    function getCookie(c_name) {
        if (document.cookie.length > 0) {
            var c_start = document.cookie.indexOf(c_name + "=");
            if (c_start != -1) {
                c_start = c_start + c_name.length + 1;
                var c_end = document.cookie.indexOf(";", c_start);
                if (c_end == -1) {
                    c_end = document.cookie.length;
                }
                return unescape(document.cookie.substring(c_start, c_end));
            }
        }
        return "";
    }


    function getTestAuth() {

                $http.get(API_CONFIG.serverPath +'api/grantstudent/isparticipant/' + 21)
                .success(function (data, status) {

                    var test = '';
                    
                  }).error(function () {

                    $mdToast.show(
                        $mdToast.simple()
                        .content('Вам нужно авторизоваться на сайте')
                        .theme("error-toast")
                        .position('top'));

                      window.location.href = "#/login/";
                      window.location.reload();

                  });

    }


    var authToken = getCookie('authToken');

    if(authToken){
          $http.defaults.headers.common.Authorization = 'Bearer ' + authToken;
    }


//    if ($window.sessionStorage.getItem('token')) {
//            $http.defaults.headers.common.Authorization = 'Bearer ' + $window.sessionStorage.getItem('token');
//    }



    $rootScope.$on( '$stateChangeStart', function(e, toState  , toParams
                                               , fromState, fromParams) {

    	var isLogin = (toState.controller === "LoginController") || (toState.controller === "SignupController") || (toState.controller === "ForgotController");

        if(isLogin){

           return; // no need to redirect anymore
        }

    	if ($http.defaults.headers.common.Authorization) {
    				getTestAuth();
    	}
    	else {
    			  //alert('DENY : Redirecting to Login');
    			  //event.preventDefault();
                   window.location.href = "#/login/";
                   window.location.reload();
    	}


	});


     // e.addMenu({
     //     name: "Образовательные организации",
     //     icon: "icon-account-balance",
     //     type: 'link',
            //    state: "admin-panel.default.g-university-list",

            //});


    // add a class to the body if we are on windows
    if($window.navigator.platform.indexOf('Win') !== -1) {
        $rootScope.bodyClasses = ['os-windows'];
    }
});

