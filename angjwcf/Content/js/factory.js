angular
  .module('todoApp')
  .factory('todoFactory', ['$http', function ($http) {
      var urlBase = 'http://localhost:8890/angjwcfSvc';
      var todoFactory = {};

      todoFactory.getTodos = function () {
		  //alert('i will fetch '+urlBase+'/List');
		 // var deferred = $.Deferred();
          //uScriptHelper.xmlHttpRequest({ url: urlBase + '/list', method: 'get', onload: function () { alert(this.responseText); return (this.responseText); } });
		  $.ajax({
		      url:urlBase+'/list',
		      dataType: 'json',
              type:'get',
		   success: function(dt)
		   { alert(JSON.stringify(dt)); deferred.resolve(dt);alert('hurray')},
		   error: function(jqXHR, textStatus, errorThrown ){
		   alert('Error '+JSON.stringify(jqXHR)+textStatus+errorThrown);
		   var err = eval('(' + xhr.responseText + ')');
		   alert(err.Message);}
		   });

		   //return(deferred.promise());

		  //promise.then(function(data){alert(data);return(data);},function(err){alert('oyei oyei something went wrong'});
         // return $http.get(urlBase+'/List');
      };
      todoFactory.getCountries = function () {
          return(urlBase + '/List');
      };

      return todoFactory;
  }])

