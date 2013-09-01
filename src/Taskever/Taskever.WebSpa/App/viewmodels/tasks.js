define(["jquery", "knockout"], function ($, ko) {

    var taskever = { models: { } };

    taskever.models.TaskDto = function () {
        this.title = ko.observable();
        this.description = ko.observable();
    };

    var tasks = ko.observableArray([]);

    return {
        tasks: tasks,
        activate: function () {
            $('.loading-message').show();
            tasks.removeAll();
            $.ajax({
                url: '/api/Tasks',
                dataType: 'json',
                type: 'GET',
                contentType: 'application/json',
                success: function (data) {
                    $('.loading-message').hide();
                    for (var i = 0; i < data.length; i++)
                    {
                        var task = new taskever.models.TaskDto();
                        task.title(data[i].Title);
                        task.description(data[i].Description);
                        tasks.push(task);
                    }
                }
            });
        }
    };
});