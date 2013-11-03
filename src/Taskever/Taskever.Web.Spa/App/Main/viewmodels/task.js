define(
    ['jquery', 'service!taskever/task'],
    function ($, taskService) {

        return function () {
            var that = this;

            // Private variables //////////////////////////////////////////////////

            // Public fields //////////////////////////////////////////////////////

            that.task = ko.mapping.fromJS({});

            // Public methods /////////////////////////////////////////////////////

            that.canActivate = function (taskId) {
                return taskService.getTask({
                    id: taskId
                }).done(function(data) {
                    ko.mapping.fromJS(data.task, that.task);
                });
            };
        };
    }
);