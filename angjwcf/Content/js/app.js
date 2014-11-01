'use strict';

var app = angular.module('todoApp', ['ui.router', 'ngResource', 'ui.bootstrap', 'mgcrea.ngStrap'])

      .config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {
          $urlRouterProvider.otherwise('/');
          $stateProvider
            .state('home', {
                url: '/',
                templateUrl: 'templates/todoEdit.html',
                controller: 'todoEditCtrl'
            })
            .state('edit2', {
                url: '/edit2',
                templateUrl: 'templates/todoEdit2.html',
                controller: 'todoEditCtrl'
            })
            .state('view', {
                url: '/view',
                templateUrl: 'templates/viewTodo.html',
                controller: 'todoViewCtrl'
            })
            .state('contact', {
                url: '/contact',
                templateUrl: 'contact.html',
                controller: 'quizCtrl'
            })
      }])
.config(function ($datepickerProvider) {
    angular.extend($datepickerProvider.defaults, {
        dateFormat: 'dd MMM, yyyy',
        startWeek: 1
    });
})
//.config(['$urlRouteProvider', '$stateProvider', function($urlRouteProvider, $stateProvider){
//    $urlRouteProvider.otherwise('/');
//    $stateProvider
//        .state('quiz', {
//            url:'/',
//            templateUrl: 'templates/quiz.html'
//        })
//        .state('result', {
//            url:'/result',
//            templateUrl: 'templates/result.html'
//        })
//}])
