define(["jquery", "knockout", 'durandal/app', 'plugins/dialog', 'service!taskever/task', 'session'],
    function ($, ko, app, dialogs, taskService, session) {

        var maxTaskCount = 10;
        var tasks = ko.mapping.fromJS([]);

        return {
            tasks: tasks,
            currentUserId: session.getCurrentUser().id(),
            
            attached: function (view) {
                abp.ui.setBusy($(view), {
                    promise: taskService.getTasksByImportance({
                        assignedUserId: session.getCurrentUser().id(),
                        maxResultCount: maxTaskCount
                    }).done(function(data) {
                        ko.mapping.fromJS(data.tasks, tasks);
                    })
                });
            }
        };
    });