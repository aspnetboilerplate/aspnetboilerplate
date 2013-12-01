define(
    ['jquery', 'underscore', 'service!taskever/task', 'service!taskever/friendship', 'session', 'plugins/history', 'plugins/dialog'],
    function ($, _, taskService, friendshipService, session, history, dialogs) {
        
        return function () {
            var that = this;

            // Public fields //////////////////////////////////////////////////////

            that.task = ko.mapping.fromJS({});
            that.isEditable = false;

            // Public methods /////////////////////////////////////////////////////

            that.canActivate = function (taskId) {
                return taskService.getTask({
                    id: taskId
                }).done(function (data) {
                    ko.mapping.fromJS(data.task, that.task);
                    that.isEditable = data.isEditableByCurrentUser;
                });
            };

            that.deleteTask = function () {
                dialogs.showMessage(
                    that.task.title() + ' will be permanently deleted! Are you sure?',
                    'Delete the task?',
                    ['No', 'Yes']
                ).done(function (result) {
                    if (result == 'Yes') {
                        taskService.deleteTask({
                            id: that.task.id()
                        }).done(function () {
                            history.navigate('#');
                        });
                    }
                });
            };
        };
    }
);