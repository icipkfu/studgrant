'use strict';

/**
 * @ngdoc module
 * @name triangular.todo
 * @description
 *
 * The `triangular.extras` module handles all the extra pages
 */
angular.module('triAngularTodo', [])
.config(function ($translatePartialLoaderProvider, $stateProvider) {
    $translatePartialLoaderProvider.addPart('app/todo');

    $stateProvider
    .state('admin-panel-no-scroll.default.todo', {
        url: '/todo',
        templateUrl: 'app/todo/todo.tmpl.html',
        controller: 'TodoController'
    });
})
.run(function(SideMenu) {
    //SideMenu.addMenu({
    //    name: 'MENU.TODO.TITLE',
    //    icon: 'icon-done',
    //    state: 'admin-panel-no-scroll.default.todo',
    //    type: 'link',
    //    priority: 2.4,
    //});
});
