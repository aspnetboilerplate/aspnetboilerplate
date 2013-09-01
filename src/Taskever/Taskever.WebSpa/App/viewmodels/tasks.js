define(["jquery", "knockout", 'durandal/app', 'models/dtos', 'remoteServices/taskService'], function ($, ko, app, dtos, taskService) {

    var tasks = ko.observableArray([]);
    $('#loading-message').hide();

    return {
        tasks: tasks,
        activate: function () {
            tasks.removeAll();
            $('#loading-message').show();

            //Calling service proxy!
            taskService.getTasks()
                .done(function(data) {
                    for (var i = 0; i < data.length; i++) {
                        tasks.push(data[i]);
                    }
                }).fail(function() {
                    //...
                }).always(function() {
                    $('#loading-message').hide();
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