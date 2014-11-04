angular
  .module('todoApp')
  .factory('todoFactory', ['$http', function ($http) {
      var urlBase = 'net.pipe://localhost/angjwcfSvc';
      var todoFactory = {};

      todoFactory.getTodos = function () {
		  var deferred = $.Deferred();
		  NamedPipeXmlHttp.xmlHttpRequest({ url: urlBase + '/list', method: 'HEAD', timeout: 15000, onload: function (obj) { alert(JSON.stringify(obj)); deferred.resolve(JSON.parse(obj.responseText)); }, onerror: function (obj) { alert(JSON.stringify(obj)); } });
		   return(deferred.promise());
      };
      todoFactory.getCountries = function () {
          return(urlBase + '/List');
      };

      return todoFactory;
  }])

