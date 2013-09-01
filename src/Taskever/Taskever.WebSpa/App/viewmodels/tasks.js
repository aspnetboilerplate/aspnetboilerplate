define(["jquery", "knockout", 'durandal/app'], function ($, ko, app) {

    var taskever = { models: { } };

    taskever.models.TaskDto = function () {
        this.title = ko.observable();
        this.description = ko.observable();
    };

    var tasks = ko.observableArray([]);

    return {
        tasks: tasks,
        activate: function () {
            $('#loading-message').show();
            tasks.removeAll();
            $.ajax({
                url: '/api/Tasks',
                dataType: 'json',
                type: 'GET',
                contentType: 'application/json',
                success: function (data) {
                    for (var i = 0; i < data.length; i++)
                    {
                        var task = new taskever.models.TaskDto();
                        task.title(data[i].Title);
                        task.description(data[i].Description);
                        tasks.push(task);
                    }
                },
                complete: function () {
                    console.log(1);
                    $('#loading-message').hide();
                }
            });
        },
        showTaskCreateDialog: function () {
            app.showDialog({
                viewUrl: 'views/createTask',
                saveNewTask: this.saveNewTask
            });
        },
        saveNewTask: function () {
            $.ajax({
                url: '/api/Tasks',
                dataType: 'json',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ //TODO: Add json2.js!
                    title: 'teeest title!!',
                    description: 'my descriptionnnnnn'
                }),
                success: function (data) {
                    console.log(data);
                },
                complete: function () {
                    console.log(3);
                }
            });
        }
    };
});