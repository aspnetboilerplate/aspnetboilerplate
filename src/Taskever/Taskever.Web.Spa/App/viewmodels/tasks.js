define(["jquery", "knockout", 'durandal/app', 'models/dtos', 'remoteServices/taskService'], function ($, ko, app, dtos, taskService) {
    var tasks = ko.mapping.fromJS([]);

    return {
        tasks: tasks,

        activate: function () {
            taskService.getTasks(tasks);
        },

        showTaskCreateDialog: function () {
            app.showDialog({
                viewUrl: 'views/createTask',
                saveNewTask: this.saveNewTask
            });
        },
        
        saveNewTask: function () {
            taskService.createTask({
                title: 'teeest title!!',
                description: 'my descriptionnnnnn'
            });
        }
    };
});