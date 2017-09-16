window.onload = function () {
    $.ajax({
        url: 'http://localhost:53515/Dashboard/getChartScannerData',
        method: "GET",
        success: function (data) {
            console.log(data);
            var Model = [];
            var RepairQTY = [];
            var Minimum = [];

            for (var i in data) {
                Model.push(data[i].Model);
                RepairQTY.push(data[i].RepairQTY);
                Minimum.push(data[i].Minimum);
            }

            var chartdata = {
                labels: Model,
                datasets: [
                    {
                        label: 'Repair QTY',
                        type: 'line',
                        fill: false,
                        lineTension: 0,
                        backgroundColor: "rgba(75,192,192,0.4)",
                        borderColor: "rgba(75,192,192,1)",
                        borderCapStyle: 'butt',
                        borderDash: [],
                        borderDashOffset: 0.0,
                        borderJoinStyle: 'miter',
                        pointBorderColor: "rgba(75,192,192,1)",
                        pointBackgroundColor: "#fff",
                        pointBorderWidth: 1,
                        pointHoverRadius: 5,
                        pointHoverBackgroundColor: "rgba(75,192,192,1)",
                        pointHoverBorderColor: "rgba(220,220,220,1)",
                        pointHoverBorderWidth: 2,
                        pointRadius: 1,
                        pointHitRadius: 10,
                        spanGaps: false,
                        data: RepairQTY
                    },
                    {
                        label: 'Minimum',
                        type: 'bar',
                        backgroundColor: 'rgba(255,99,132,0.2)',
                        borderColor: 'rgba(255,99,132,1)',
                        hoverBackgroundColor: 'rgba(200, 200, 200, 1)',
                        hoverBorderColor: 'rgba(200, 200, 200, 1)',
                        data: Minimum
                    }
                ]
            };
            //var tid = "#myChart" + i;
            var tid = "#myChartSC";
            var ctx = $(tid);
            var barGraph = new Chart(ctx, {
                type: 'bar',
                data: chartdata
            })
            $('#myChartSC').click(function (e) {
                var activeBars = barGraph.getElementsAtEvent(e)
                var firstPoint = activeBars[0];
                //if (firstPoint !== undefined)
                alert(chartdata.datasets[firstPoint._datasetIndex].data[firstPoint._index])
                var department = chartdata.labels[firstPoint._index];
                window.location.href = 'getDevice/' + '?param1=' + department
                //var url = "http://localhost:53515/Dashboard/getDevice/?" + department;
                //alert(url);
                //window.location.href = url;
                //var url = $(this).data('request-url');
                //var url = '@Url.Action(MVC.Membership.Permission.ActionNames.GrantRevoke, MVC.Membership.Permission.Name, new { area = "Membership", roleName= "Teste" }, null)';
                //window.location.href = url+'/'+department;
            });
            //}
        },
        error: function (data) {
            console.log(data);
        }
    });
};
