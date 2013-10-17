define(["jquery", "knockout", 'durandal/app', 'plugins/dialog', 'service!taskever/task', 'session'], function ($, ko, app, dialogs, taskService, session) {
    var tasks = ko.mapping.fromJS([]);

    return {
        tasks: tasks,

        activate: function () {
            taskService.getTasksOfUser({
                userId: session.getCurrentUser().id()
            }).then(function(data) {
                ko.mapping.fromJS(data.tasks, tasks);
            });
        },

        showTaskCreateDialog: function () {
            dialogs.show('viewmodels/createTaskDialog')
                .then(function (data) {
                    if (data) {
                        if (data.assignedUserId() == session.getCurrentUser().id()) {
                            tasks.push(data);
                        }
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