(function () { // Angular encourages module pattern, good!

    var query = function () {
        this.connectionId = "";
        this.path = "C:\\";
    }

    var app = angular.module('myApp', ['kendo.directives']);
    app.controller('myCtrl', ['$http', '$scope', function ($http, $scope) {

        var hub = $.connection.myHub; // create a proxy to signalr hub on web server

        hub.client.hello = function () {
            alert('hello');
        };

        hub.client.ReportStep = function (message) {
            $scope.message = $('<div/>').text(message).html();
            if ($scope.message.substring(0, 2) == "執行")
            {
                $scope.running = false;
            }
            $scope.$apply();
        };

        var mainGridOptionsDataSource = [];
        hub.client.ReportResult = function (path, fileCount, totalSize) {
            mainGridOptionsDataSource.push({
                "path": $('<div/>').text(path).html(),
                "fileCount": fileCount,
                "totalSize": totalSize
            });
            $scope.mainGridOptions.dataSource = new kendo.data.DataSource({
                data: mainGridOptionsDataSource,
                pageSize: 10
            });
        };

        $scope.queryInfo = new query();

        $.connection.hub.start().done(function () {
            hub.server.register().done(function (e) {
                $scope.queryInfo.connectionId = $.connection.hub.id;
                if (e != "") {
                    alert(e);
                }
            });
        });

        $scope.running = false;

        $scope.start = function () {
            mainGridOptionsDataSource = [];
            $scope.mainGridOptions.dataSource = new kendo.data.DataSource({
                data: mainGridOptionsDataSource
            });

            var url = "http://localhost:36344/StartRun";
            $http.post(url, $scope.queryInfo)
                .success(function (data, status) {
                    $scope.running = true;
                })
                .error(function (data, status) {
                });
        };

        $scope.stop = function () {
            var url = "http://localhost:36344/StopRun";
            $http.post(url, $scope.queryInfo)
                .success(function (data, status) {
                })
                .error(function (data, status) {
                });
        };


        $scope.mainGridOptions = {
            pageable: true,
            columns: [
                { field: "path", title: "伺服器路徑", width: "500px" },
                { field: "fileCount", title: "檔案數", width: "50px" },
                { field: "totalSize", title: "共使用位元組", width: "200px" },
            ]
        };

    }]);

})();