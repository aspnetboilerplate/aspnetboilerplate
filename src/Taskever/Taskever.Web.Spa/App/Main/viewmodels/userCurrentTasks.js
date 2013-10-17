define(["jquery", "knockout", 'durandal/app', 'plugins/dialog', 'service!taskever/task', 'session'], function ($, ko, app, dialogs, taskService, session) {
    var tasks = ko.mapping.fromJS([]);

    return {
        tasks: tasks,

        activate: function () {
            taskService.getTasksOfUser({
                userId: session.getCurrentUser().id()
            }).then(function (data) {
                ko.mapping.fromJS(data.tasks, tasks);
            });
        }
    };
});