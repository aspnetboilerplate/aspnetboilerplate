define(
    ["jquery", "knockout", 'session', 'service!taskever/task'],
    function ($, ko, session, taskService) {

        var _defaultActivationData = {
            assignedUserId: 0,
            taskStates: []
        };

        var _maxResultCount = 20;

        return function () {
            var that = this;

            // Private fields /////////////////////////////////////////////////////

            var _activationData;
            var _skipCount = 0;

            // Public fields //////////////////////////////////////////////////////

            that.tasks = ko.mapping.fromJS([]);
            that.hasPreviousPage = ko.observable(false);
            that.hasNextPage = ko.observable(false);

            // Public methods /////////////////////////////////////////////////////

            that.activate = function (activationData) {
                _activationData = $.extend({ }, _defaultActivationData, activationData);
                loadData();
            };

            that.gotoPreviousPage = function() {
                _skipCount = Math.max(0, _skipCount - _maxResultCount);
                loadData();
            };

            that.gotoNextPage = function() {
                _skipCount += that.tasks().length;
                loadData();
            };
            
            // Private methods ///////////////////////////////////////////////////
            
            var loadData = function () {
                taskService.getTasks({
                    assignedUserId: _activationData.assignedUserId,
                    taskStates: _activationData.taskStates,
                    maxResultCount: _maxResultCount,
                    skipCount: _skipCount
                }).done(function (data) {
                    ko.mapping.fromJS(data.tasks, that.tasks);
                    that.hasPreviousPage(_skipCount > 0);
                    that.hasNextPage(data.tasks.length >= _maxResultCount);
                });

            };
        };
    }
);