angular
  .module('todoApp')
  .factory('todoFactory', ['$http', function ($http) {
      var urlBase = 'http://localhost:8890/angjwcfSvc';
      var todoFactory = {};

      todoFactory.getTodos = function () {
		  var deferred = $.Deferred();
		  uScriptHelper.xmlHttpRequest({ url: urlBase + '/list', method: 'GET', timeout: 15000, onload: function (obj) { /*alert(JSON.stringify(obj));*/ deferred.resolve(JSON.parse(obj.responseText)); }, onerror: function (obj) { alert(JSON.stringify(obj)); deferred.reject(obj); } });
		   return(deferred.promise());
      };
      todoFactory.getCountries = function () {
          return(urlBase + '/List');
      };

      return todoFactory;
  }])

