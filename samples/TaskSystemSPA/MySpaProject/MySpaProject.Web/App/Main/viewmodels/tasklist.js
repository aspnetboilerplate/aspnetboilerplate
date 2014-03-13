define(['service!tasksystem/task'], function (taskService) {
    return function () {
        var that = this;

        that.tasks = ko.mapping.fromJS([]);

        that.activate = function () {
            taskService.getAllTasks({})
                .done(function (data) {
                    ko.mapping.fromJS(data.tasks, that.tasks);
                });
        };
    };
});