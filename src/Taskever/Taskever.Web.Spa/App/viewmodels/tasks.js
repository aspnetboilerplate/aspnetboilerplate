define(["jquery", "knockout", 'durandal/app', 'plugins/dialog', 'models/dtos', 'remoteServices/taskService'], function ($, ko, app, dialogs, dtos, taskService) {
    var tasks = ko.mapping.fromJS([]);

    return {
        tasks: tasks,

        activate: function () {
            taskService.getTasks(tasks);
        },

        showTaskCreateDialog: function () {
            dialogs.show('viewmodels/createTaskDialog').then(function (data) {
                tasks.push(data);
            });
        }
    };
});