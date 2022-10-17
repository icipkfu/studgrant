'use strict';

/**
 * @ngdoc filter
 * @name emailGroup
 * @module triAngularEmail
 * @kind filter
 *
 * Filters emails into groups like Yesterday, Last Week, etx
 */

angular.module('triAngularEmail')
.filter('emailGroup', function() {
    return function(emails, emailGroup) {
        return emails.filter(function(email) {
            var emailDate = moment(email.date, moment.ISO_8601);

            if(emailDate.isAfter(emailGroup.from) && emailDate.isBefore(emailGroup.to)) {
                return email;
            }
        });
    };
});