'use strict';

/**
 * @ngdoc function
 * @name InboxController
 * @module triAngularEmail
 * @kind function
 *
 *
 */
angular.module('triAngularEmail')
.controller('InboxController', function ($scope, $filter, $location, $state, $mdMedia, $mdBottomSheet, $stateParams, $mdDialog, $mdToast, emails, contacts) {
    // store the base state of where we are /inbox or /trash or /sent
    // this can be then used if we close / delete email to return to
    $scope.baseState = $state.current;

    $scope.inboxBasePath = $location.path();
    // store selected email if we have one
    $scope.selectedMail = null;
    // variable to store backup of emailGroups for search filtering
    var emailGroupsBackup = null;

    // create email groups using the emails from the resolve
    if(emails.status === 200) {
        $scope.emails = emails.data;

        $scope.emailGroups = [{
            name: $filter('translate')('EMAIL.INBOX.GROUPS.TODAY'),
            from: moment().startOf('day'),
            to: moment().endOf('day')
        },{
            name: $filter('translate')('EMAIL.INBOX.GROUPS.YESTERDAY'),
            from: moment().subtract(1, 'days').startOf('day'),
            to: moment().subtract(1, 'days').endOf('day')
        },{
            name: $filter('translate')('EMAIL.INBOX.GROUPS.OLDER'),
            from: moment().subtract(100, 'years').endOf('day'),
            to: moment().subtract(2, 'days').startOf('day')
        }];

        angular.forEach($scope.emailGroups, function(group) {
            group.emails = $filter('emailGroup')($scope.emails, group);
        });

        // create backup of emailGroups for search filtering
        emailGroupsBackup = angular.copy($scope.emailGroups);
    }

    // opens an email
    $scope.openMail = function(email) {
        $state.go($scope.baseState.name + '.email', {
            emailID: email.id
        });
        email.unread = false;
        $scope.selectedMail = email.id;
    };

    // returns back to email list
    $scope.openlist = function() {       
        $state.go($scope.baseState.name);
    };

    // returns back to email list
    $scope.delete = function(deleteEmail) {
        angular.forEach($scope.emailGroups, function(group) {
            var removeEmailIndex = null;
            angular.forEach(group.emails, function(email, index) {
                if(deleteEmail.id === email.id) {
                    removeEmailIndex = index;
                }
            });
            if(null !== removeEmailIndex) {
                group.emails.splice(removeEmailIndex, 1);
                $mdToast.show(
                    $mdToast.simple()
                    .content($filter('translate')('EMAIL.DELETED'))
                    .position('bottom right')
                    .hideDelay(3000)
                );
            }
        });

        $scope.openlist();
    };

    // opens the compose dialog
    $scope.composeClick = function($event) {
        $mdDialog.show({
            controller: 'EmailDialogController',
            templateUrl: 'app/email/email-dialog.tmpl.html',
            targetEvent: $event,
            locals: {
                title: $filter('translate')('EMAIL.NEW'),
                email: {
                    to: [],
                    cc: [],
                    bcc:[],
                    subject: '',
                    content: ''
                },
                contacts: contacts,
                getFocus: false
            }
        })
        .then(function(email) {
            $scope.sendEmail(email);
        }, function() {
            $mdToast.show(
                $mdToast.simple()
                .content($filter('translate')('EMAIL.CANCELED'))
                .position('bottom right')
                .hideDelay(3000)
            );
        });
    };

    $scope.sendEmail = function(email) {
        // make list of users that have been sent to
        var sentTo = [];
        angular.forEach(email.to, function(to) {
            sentTo.push(to.name);
        });
        $mdToast.show(
            $mdToast.simple()
            .content($filter('translate')('EMAIL.SENT', {to: sentTo.join(', ')}))
            .position('bottom right')
            .hideDelay(3000)
        );
    };

    $scope.$on('emailSearch', function(event, emailSearch) {
        for(var g in emailGroupsBackup) {
            $scope.emailGroups[g].emails = $filter('emailSearchFilter')(emailGroupsBackup[g].emails, emailSearch);
        }
    });


    function checkEmailList() {
        $scope.showEmailList = !($mdMedia('sm') && $state.current.resolve.email !== undefined);
    }

    // add a watch for when the url location changes
    $scope.$on('$locationChangeSuccess', checkEmailList);

    checkEmailList();
});