'use strict';

/**
 * @ngdoc filter
 * @name emailSearchFilter
 * @module triAngularEmail
 * @kind filter
 *
 * Filters emails when user searches using top search box
 */

angular.module('triAngularEmail')
.filter('emailSearchFilter', function() {
    return function(emails, emailSearch) {
        return emails.filter(function(email) {
            if(email.from.name.indexOf(emailSearch) > -1) {
                return email;
            }
            if(email.subject.indexOf(emailSearch) > -1) {
                return email;
            }
        });
    };
});