define(["jquery", "knockout", 'durandal/app', 'plugins/dialog', 'service!taskever/task', 'session'],
    function ($, ko, app, dialogs, taskService, session) {

        var maxTaskCount = 10;

        return function () {
            var that = this;

            // Private variables //////////////////////////////////////////////////

            var $view;

            // Public fields //////////////////////////////////////////////////////

            that.tasks = ko.mapping.fromJS([]);
            that.currentUserId = session.getCurrentUser().id();
            that.refreshing = ko.observable(false);

            // Public methods /////////////////////////////////////////////////////

            that.attached = function (view) {
                $view = $(view);
                that.refresh();
            };

            that.refresh = function() {
                if (that.refreshing()) {
                    return;
                }

                that.refreshing(true);
                abp.ui.setBusy($view, {
                    promise: taskService.getTasksByImportance({
                        assignedUserId: session.getCurrentUser().id(),
                        maxResultCount: maxTaskCount
                    }).done(function(data) {
                        ko.mapping.fromJS(data.tasks, that.tasks);
                    }).always(function() {
                        that.refreshing(false);
                    })
                });
            };

            that.createTask = function() {
                dialogs.show('viewmodels/task/create');
            };

        };
    });