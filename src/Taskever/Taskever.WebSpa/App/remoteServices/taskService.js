define(['jquery', 'models/dtos'], function ($, dtos) {

    var getTasks = function() {
        var defer = $.Deferred();
        
        $.ajax({
            url: '/api/Ta2sks',
            dataType: 'json',
            type: 'GET',
            contentType: 'application/json',
            success: function (data) {
                if (data.Message) {
                    defer.reject();
                    return;
                }

                var tasks = [];
                for (var i = 0; i < data.length; i++) {
                    var task = new dtos.TaskDto();
                    task.title(data[i].Title);
                    task.description(data[i].Description);
                    tasks.push(task);
                }

                defer.resolve(tasks);
            },
            error : function () {
                defer.reject();
            }
        });

        return defer.promise();
    };
    
    return {
        getTasks: getTasks
    };
});
