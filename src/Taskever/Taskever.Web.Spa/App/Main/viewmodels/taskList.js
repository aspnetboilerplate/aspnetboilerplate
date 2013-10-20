define(
    ["jquery", "knockout", 'session', 'service!taskever/task'],
    function ($, ko, session, taskService) {

        var _defaultActivationData = {
            assignedUserId: 0,
            taskStates: [],
            maxResultCount: 20
        };

        return function () {
            var that = this;

            // Private fields /////////////////////////////////////////////////////

            var _activationData = null;

            // Public fields //////////////////////////////////////////////////////

            that.tasks = ko.mapping.fromJS([]);

            // Public methods /////////////////////////////////////////////////////

            that.activate = function (activationData) {
                _activationData = $.extend({ }, _defaultActivationData, activationData);
                taskService.getTasks({
                    assignedUserId: _activationData.assignedUserId,
                    taskStates: _activationData.taskStates
                }).then(function (data) {
                    ko.mapping.fromJS(data.tasks, that.tasks);
                });
            };
        };
    }
);