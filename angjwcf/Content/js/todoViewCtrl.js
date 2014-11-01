app.controller('todoViewCtrl', ['$scope', '$http', 'todoFactory', function ($scope, $http, todoFactory) {

    $scope.todos;

    getTodo();

    function getTodo() {
        todoFactory.getTodos()
            .success(function (data) {
                $scope.todos = data;
            })
            .error(function (error) {
                $scope.status = 'Unable to load todo data: ' + error;
                console.log(error);
            });
    }
}]);