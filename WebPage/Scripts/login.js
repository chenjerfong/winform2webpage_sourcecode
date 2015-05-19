(function () { // Angular encourages module pattern, good!
    var app = angular.module('myApp', []);
    app.controller('myCtrl', ['$http', '$scope', function ($http, $scope) {

        $scope.login = function () {
            var url = "http://localhost:36344/Authorized";
            $http.post(url)
                .success(function (data, status) {
                    alert("ok");
                })
                .error(function (data, status) {
                    alert(status);
                });
        };

    }]);

})();