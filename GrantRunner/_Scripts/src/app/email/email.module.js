'use strict';

/**
 * @ngdoc module
 * @name triAngularEmail
 * @description
 *
 * The `triangular.email` module adds email inbox pages
 */
angular.module('triAngularEmail', [])
.constant('EMAIL_ROUTES', [{
    state: 'admin-panel-no-scroll.email.inbox',
    name: 'MENU.EMAIL.INBOX',
    url: '/email/inbox',
    icon: 'icon-inbox'
},{
    state: 'admin-panel-no-scroll.email.trash',
    name: 'MENU.EMAIL.TRASH',
    url: '/email/trash',
    icon: 'icon-remove-circle'
},{
    state: 'admin-panel-no-scroll.email.sent',
    name: 'MENU.EMAIL.SENT',
    url: '/email/sent',
    icon: 'icon-mail'
}])
.config(function ($translatePartialLoaderProvider, $stateProvider, $provide, EMAIL_ROUTES) {
    $translatePartialLoaderProvider.addPart('app/email');

    $stateProvider
    .state('admin-panel-no-scroll.email', {
        abstract: true,
        views: {
            sidebarLeft: {
                templateUrl: 'components/sidebar-left/sidebar-left.tmpl.html',
                controller: 'SidebarLeftController'
            },
            toolbar: {
                templateUrl: 'app/email/toolbar.tmpl.html',
                controller: 'EmailToolbarController'
            },
            content: {
                template: '<div flex ui-view layout="column" class="overflow-hidden"></div>'
            }
        },
    });

    angular.forEach(EMAIL_ROUTES, function(route) {
        $stateProvider
        .state(route.state, {
            url: route.url,
            templateUrl: 'app/email/inbox.tmpl.html',
            controller: 'InboxController',
            resolve: {
                emails: function($http, API_CONFIG) {
                    return $http({
                        method: 'GET',
                        url: API_CONFIG.url + 'email/inbox',
                    });
                },
                contacts: function($http, API_CONFIG) {
                    return $http({
                        method: 'GET',
                        url: API_CONFIG.url + 'email/contacts',
                    });
                }
            }
        });
    });

    angular.forEach(EMAIL_ROUTES, function(route) {
        $stateProvider
        .state(route.state + '.email', {
            url: '/mail/:emailID',
            templateUrl: 'app/email/email.tmpl.html',
            controller: 'EmailController',
            resolve: {
                email: function($stateParams, emails) {
                    emails = emails.data;
                    var foundEmail = false;
                    for(var i = 0; i < emails.length; i++) {
                        if(emails[i].id === $stateParams.emailID) {
                            foundEmail = emails[i];
                            break;
                        }
                    }
                    return foundEmail;
                }
            },
            onEnter: function($state, email){
                if (false === email) {
                    $state.go(route.state);
                }
            },
        });
    });

    /***
    * Setup Editor Toolbar here
    ***/
    $provide.decorator('taOptions', ['taRegisterTool', 'taTranslations', '$delegate', function(taRegisterTool, taTranslations, taOptions){
        taOptions.toolbar = [['bold', 'italics', 'underline', 'insertLink']];

        taOptions.classes = {
            focussed: 'focussed',
            toolbar: 'editor-toolbar',
            toolbarGroup: 'editor-group',
            toolbarButton: 'md-button',
            toolbarButtonActive: '',
            disabled: '',
            textEditor: 'form-control',
            htmlEditor: 'form-control'
        };
        return taOptions;
    }]);

    $provide.decorator('taTools', ['$delegate', function(taTools){
        taTools.bold.iconclass = 'icon-format-bold';
        taTools.italics.iconclass = 'icon-format-italic';
        taTools.underline.iconclass = 'icon-format-underline';
        taTools.insertLink.iconclass = 'icon-insert-link';
        return taTools;
    }]);

})
.run(function(SideMenu, $translate, EMAIL_ROUTES) {
    var emailMenu = {
        name: 'MENU.EMAIL.EMAIL',
        icon: 'icon-email',
        type: 'dropdown',
        priority: 2.2,
        children: []
    };

    angular.forEach(EMAIL_ROUTES, function(route) {
        emailMenu.children.push({
            name: route.name,
            state: route.state,
            type: 'link',
        });
    });

    //SideMenu.addMenu(emailMenu);

    //SideMenu.addMenu({
    //    type: 'divider',
    //    priority: 2.3
    //});

});

