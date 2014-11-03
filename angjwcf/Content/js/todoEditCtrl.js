app.controller('todoEditCtrl', ['$scope', '$http', 'todoFactory', function ($scope, $http, todoFactory) {
    
    $scope.todos;
    $scope.countries;

    
    $scope.getTodoData = function() {
        //alert('fetching');
        //todoFactory.getCountries()
        //    .success(function (data) {
        //        $scope.countries = data;
        //    })
        //    .error(function (error) {
        //        $scope.status = 'Unable to load country data: ' + error;
        //        console.log(error);
        //    });

        var promise=  todoFactory.getTodos();
            promise.then(function (data) {
			//alert(data);
                setInterval(function () {
                    $scope.todos = data;
                    $scope.$apply();
                }, 10);
                //$scope.todos.forEach(function (tdvm) {
                //    alert(JSON.stringify(tdvm));
                //});
            },function (error) {
                $scope.status = 'Unable to load todo data: ' + error;
                alert('unable to load data '+error);
            }, function (update) {
                alert('Got notification: ' + update);
            });

    }

    $scope.setDropDown = function (emp) {
        for (var index in $scope.countries) {
            if ($scope.countries[index].Id == emp.CountryId) {
                emp.Country = $scope.countries[index];
                return $scope.countries[index];
            }
        }
    }

    $scope.onSubmit = function () {
        var todos = $scope.todos;
        alert('http://localhost:8890/angjwcfSvc/Add');
        $http.post('http://localhost:8890/angjwcfSvc/Add', todos).success(function (data, status) {
            alert(data);
        });
    }
}]);