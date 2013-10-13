define(["jquery", "knockout", 'durandal/app', 'plugins/dialog', 'service!task'], function ($, ko, app, dialogs, taskService) {
    var tasks = ko.mapping.fromJS([]);

    return {
        tasks: tasks,

        activate: function () {
            taskService.getTasksOfUser({ userId: 1 })
                .then(function(data) {
                    ko.mapping.fromJS(data.tasks, tasks);
                });
        }
    };
});