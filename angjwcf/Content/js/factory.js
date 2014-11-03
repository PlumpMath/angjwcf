angular
  .module('todoApp')
  .factory('todoFactory', ['$http', function ($http) {
      var urlBase = 'http://localhost:8890/angjwcfSvc';
      var todoFactory = {};

      todoFactory.getTodos = function () {
		  var deferred = $.Deferred();
          uScriptHelper.xmlHttpRequest({ url: urlBase + '/list', method: 'get', onload: function (obj) { /*alert(JSON.parse(obj.responseText));*/ deferred.resolve(JSON.parse(obj.responseText)); } });
		   return(deferred.promise());
      };
      todoFactory.getCountries = function () {
          return(urlBase + '/List');
      };

      return todoFactory;
  }])

