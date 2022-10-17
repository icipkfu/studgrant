'use strict';

/**
 * @ngdoc function
 * @name CalendarController
 * @module triAngularCalendar
 * @kind function
 *
 * @description
 *
 * Handles the main calendar page
 */

angular.module('triAngularCalendar')
.controller('CalendarController', function ($scope, $rootScope, $mdDialog, $mdToast, $filter, $element, triTheming, uiCalendarConfig) {
    $scope.calendarOptions = {
        contentHeight: 'auto',
        selectable: true,
        editable: true,
        header: false,
        viewRender: function(view) {
            // change day
            $scope.currentDay = view.calendar.getDate();
            // update toolbar with new day for month name
            $rootScope.$broadcast('calendar-changeday', $scope.currentDay);
            // update background image for month
            $scope.backgroundImage = 'assets/images/calendar/' + ($scope.currentDay.month()+1) + '.jpg';
        },
        dayClick: function(date, jsEvent, view) {
            $scope.currentDay = date;
        },
        eventClick: function(calEvent, jsEvent, view) {
            $mdDialog.show({
                controller: 'EventDialogController',
                templateUrl: 'app/calendar/event-dialog.tmpl.html',
                targetEvent: jsEvent,
                focusOnOpen: false,
                locals: {
                    dialogData: {
                        title: 'CALENDAR.EDIT-EVENT',
                        confirmButtonText: 'CALENDAR.SAVE'
                    },
                    event: calEvent,
                    edit: true
                }
            })
            .then(function(event) {
                var toastMessage = 'CALENDAR.EVENT.EVENT-UPDATED';
                if(event.deleteMe !== undefined) {
                    // remove the event from the calendar
                    uiCalendarConfig.calendars['triangular-calendar'].fullCalendar('removeEvents', event._id);
                    // change toast message
                    toastMessage = 'CALENDAR.EVENT.EVENT-DELETED';
                }
                else {
                    // update event
                    uiCalendarConfig.calendars['triangular-calendar'].fullCalendar('updateEvent', event);
                }

                // pop a toast
                $mdToast.show(
                    $mdToast.simple()
                    .content($filter('translate')(toastMessage))
                    .position('bottom right')
                    .hideDelay(2000)
                );
            });
        }
    };

    $scope.eventSources = [{
        events: []
    }];

    $scope.addEvent = function($event) {
        var inAnHour = moment($scope.currentDay).add(1, 'h');
        $mdDialog.show({
            controller: 'EventDialogController',
            templateUrl: 'app/calendar/event-dialog.tmpl.html',
            targetEvent: $event,
            focusOnOpen: false,
            locals: {
                dialogData: {
                    title: 'CALENDAR.ADD-EVENT',
                    confirmButtonText: 'CALENDAR.ADD'
                },
                event: {
                    title: $filter('translate')('CALENDAR.EVENT.NEW-EVENT'),
                    allDay: false,
                    start: $scope.currentDay,
                    end: inAnHour,
                    palette: 'cyan'
                },
                edit: false
            }
        })
        .then(function(event) {
            $scope.eventSources[0].events.push(event);
            $mdToast.show(
                $mdToast.simple()
                .content($filter('translate')('CALENDAR.EVENT.EVENT-CREATED'))
                .position('bottom right')
                .hideDelay(2000)
            );
        });
    };

    function createRandomEvents(number, startDate, endDate) {
        var eventNames = ['Pick up the kids', 'Remember the milk', 'Meeting with Morris', 'Car service',  'Go Surfing', 'Party at Christos house', 'Beer Oclock', 'Festival tickets', 'Laundry!', 'Haircut appointment', 'Walk the dog', 'Dentist :(', 'Board meeting', 'Go fishing'];
        for(var x = 0; x < number; x++) {
            var randomMonthDate = randomDate(startDate, endDate);
            var inAnHour = moment(randomMonthDate).add(1, 'h');
            var randomEvent = Math.floor(Math.random() * (eventNames.length - 0));
            var randomPalette = pickRandomProperty(triTheming.palettes);

            $scope.eventSources[0].events.push({
                title: eventNames[randomEvent],
                allDay: false,
                start: randomMonthDate,
                end: inAnHour,
                backgroundColor: triTheming.rgba(triTheming.palettes[randomPalette]['500'].value),
                borderColor: triTheming.rgba(triTheming.palettes[randomPalette]['500'].value),
                textColor: triTheming.rgba(triTheming.palettes[randomPalette]['500'].contrast),
                palette: randomPalette
            });
        }
    }

    // create 10 random events for the month
    createRandomEvents(100, moment().startOf('year'), moment().endOf('year'));

    function randomDate(start, end) {
        var startNumber = start.toDate().getTime();
        var endNumber = end.toDate().getTime();
        var randomTime = Math.random() * (endNumber - startNumber) + startNumber;
        return moment(randomTime);
    }

    function pickRandomProperty(obj) {
        var result;
        var count = 0;
        for (var prop in obj) {
            if (Math.random() < 1/++count) {
                result = prop;
            }
        }
        return result;
    }
});