define(["jquery", "knockout", 'durandal/app', 'plugins/dialog', 'models/dtos', 'services/task'], function ($, ko, app, dialogs, dtos, taskService) {
    var tasks = ko.mapping.fromJS([]);

    return {
        tasks: tasks,

        activate: function () {
            taskService.getTasksOfUser({userId : 1})
                .then(function(data) {
                    ko.mapping.fromJS(data.tasks, tasks);
                });
        },

        showTaskCreateDialog: function () {
            dialogs.show('viewmodels/createTaskDialog')
                .then(function (data) {
                    if (data) {
                        tasks.push(data);
                    }
                });
        },
        
        deleteTask: function (item) {
            console.log(item);
            taskService.deleteTask(item.id())
                .then(function() {
                    tasks.remove(item);
                });
        }
    };
});