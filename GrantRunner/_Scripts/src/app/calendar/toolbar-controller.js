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

angular.module('triAngularCalendar')
.controller('CalendarToolbarController', function ($scope, $state, $element, $mdUtil, $mdSidenav, uiCalendarConfig) {
    $scope.views = [{
        name: 'CALENDAR.TOOLBAR.VIEWS.MONTH',
        icon: 'icon-view-module',
        viewName: 'month',
        dateFormat: 'MMMM YYYY'
    },{
        name: 'CALENDAR.TOOLBAR.VIEWS.WEEK',
        icon: 'icon-view-week',
        viewName: 'agendaWeek',
        dateFormat: 'w'
    },{
        name: 'CALENDAR.TOOLBAR.VIEWS.DAY',
        icon: 'icon-view-day',
        viewName: 'agendaDay',
        dateFormat: 'Do MMMM YYYY'
    }];

    $scope.currentView = $scope.views[0];

    $scope.$on('calendar-changeday', function(event, date) {
        $scope.currentDay = date;
    });

    $scope.changeMonth = function(direction) {
        uiCalendarConfig.calendars['triangular-calendar'].fullCalendar(direction);
    };

    $scope.changeView = function(view) {
        $scope.currentView = view;
        uiCalendarConfig.calendars['triangular-calendar'].fullCalendar('changeView', view.viewName);
    };

    $scope.toolbarTypeClass = function() {
        return $scope.extraClass;
    };

    $scope.openSideNav = function(navID) {
        $mdUtil.debounce(function(){
            $mdSidenav(navID).toggle();
        }, 300)();
    };

    function initToolbar() {
        if($state.current.data !== undefined) {
            if($state.current.data.toolbar !== undefined) {
                if($state.current.data.toolbar.extraClass !== false) {
                    $scope.extraClass = $state.current.data.toolbar.extraClass;
                }
            }
        }
    }
    $scope.$on('$stateChangeStart', initToolbar);
    initToolbar();
});