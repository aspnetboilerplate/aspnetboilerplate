define(["jquery", "knockout", 'plugins/dialog', 'service!dto', 'service!taskever/task', 'service!taskever/friendship', 'session'],
    function ($, ko, dialogs, dtos, taskService, friendshipService, session) {
        return function () {
            var that = this;

            // Private fields /////////////////////////////////////////////////////

            var _$form;

            // Public fields //////////////////////////////////////////////////////

            that.task = {
                title: ko.observable(),
                description: ko.observable(),
                assignedUserId: ko.observable(),
                priority: ko.observable(),
                privacy: ko.observable(),
            };

            that.users = ko.mapping.fromJS([]);

            // Public methods /////////////////////////////////////////////////////

            that.canActivate = function () {
                return friendshipService.getFriendships({
                    userId: session.getCurrentUser().id(),
                    canAssignTask: true
                }).done(function (result) {
                    var users = $.map(result.friendships, function (friendship) { return friendship.friend; });
                    ko.mapping.fromJS(users, that.users);
                    that.users.unshift(session.getCurrentUser());
                });
            };

            that.attached = function (view, parent) {
                _$form = $(view).find('form');
                _$form.validate();
            };

            that.saveNewTask = function () {
                if (!_$form.valid()) {
                    return;
                }

                taskService.createTask({
                    task: ko.mapping.toJS(that.task)
                }).done(function (result) {
                    dialogs.close(that, ko.mapping.fromJS(result.task));
                });
            };

            that.cancel = function () {
                dialogs.close(that);
            };
        };
    });