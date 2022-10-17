'use strict';

/**
 * @ngdoc function
 * @name EventDialogController
 * @module triAngularCalendar
 * @kind function
 *
 * @description
 *
 * Handles actions in event dialog
 */

angular.module('triAngularCalendar')
.controller('EventDialogController', function ($scope, $mdDialog, $filter, triTheming, dialogData, event, edit) {
    $scope.dialogData = dialogData;
    $scope.event = event;
    $scope.colors = [];
    $scope.selectedColor = null;
    $scope.edit = edit;

    $scope.start = convertMomentToBits(event.start);
    if(event.end !== null) {
        $scope.end = convertMomentToBits(event.end);
    }

    createDateSelectOptions();

    $scope.colorChanged = function() {
        event.backgroundColor = $scope.selectedColor.backgroundColor;
        event.borderColor = $scope.selectedColor.backgroundColor;
        event.textColor = $scope.selectedColor.textColor;
        event.palette = $scope.selectedColor.palette;
    };

    // create colors
    angular.forEach(triTheming.palettes, function(palette, index) {
        var color = {
            name: $filter('capitalize')(index.replace(/-/g, ' ')),
            palette: index,
            backgroundColor: triTheming.rgba(palette['500'].value),
            textColor: triTheming.rgba(palette['500'].contrast)
        };

        $scope.colors.push(color);

        if(index === event.palette) {
            $scope.selectedColor = color;
            $scope.colorChanged();
        }
    });

    $scope.ok = function() {
        saveBitsToMoment($scope.start, event.start);
        if(event.end !== null) {
            saveBitsToMoment($scope.end, event.end);
        }
        $mdDialog.hide(event);
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };

    $scope.delete = function() {
        event.deleteMe = true;
        $mdDialog.hide(event);
    }

    $scope.allDayChanged = function() {
        // if all day turned on and event already saved we need to create a new date
        if(event.allDay === false && event.end === null) {
            event.end = moment(event.start);
            event.end.endOf('day');
            $scope.end = convertMomentToBits(event.end);
        }
    };

    function convertMomentToBits(moment) {
        return {
            year: moment.year(),
            month: moment.month(),
            date: moment.date(),
            hour: moment.hour(),
            minute: moment.minute()
        };
    }

    function saveBitsToMoment(bits, moment) {
        moment.year(bits.year);
        moment.month(bits.month);
        moment.date(bits.date);
        moment.hour(bits.hour);
        moment.minute(bits.minute);
    }

    function createDateSelectOptions() {
        // create options for date select boxes (this will be removed in favor of mdDatetime picker when it becomes available)
        $scope.dateSelectOptions = {
            years: [],
            months: ['CALENDAR.MONTHNAMES.JANUARY','CALENDAR.MONTHNAMES.FEBRUARY','CALENDAR.MONTHNAMES.MARCH','CALENDAR.MONTHNAMES.APRIL','CALENDAR.MONTHNAMES.MAY','CALENDAR.MONTHNAMES.JUNE','CALENDAR.MONTHNAMES.JULY','CALENDAR.MONTHNAMES.AUGUST','CALENDAR.MONTHNAMES.SEPTEMBER','CALENDAR.MONTHNAMES.OCTOBER','CALENDAR.MONTHNAMES.NOVEMBER', 'CALENDAR.MONTHNAMES.DECEMBER'],
            dates: [],
            hours: [],
            minutes: []
        };
        // years
        var now = new Date();
        for(var year = now.getFullYear() - 5; year <= now.getFullYear() + 5; year++) {
            $scope.dateSelectOptions.years.push(year);
        }
        // days
        for(var date = 1; date <= 31; date++) {
            $scope.dateSelectOptions.dates.push(date);
        }
        // hours
        for(var hour = 0; hour <= 23; hour++) {
            $scope.dateSelectOptions.hours.push(hour);
        }
        // minutes
        for(var minute = 0; minute <= 59; minute++) {
            $scope.dateSelectOptions.minutes.push(minute);
        }
    }
});