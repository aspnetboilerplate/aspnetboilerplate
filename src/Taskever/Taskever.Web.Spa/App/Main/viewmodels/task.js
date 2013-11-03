define(
    ['jquery', 'service!taskever/task', 'service!taskever/friendship', 'session'],
    function ($, taskService, friendshipService, session) {

        return function () {
            var that = this;

            // Private variables //////////////////////////////////////////////////

            var gotUsers = false; //TODO: Can check users.length?

            // Public fields //////////////////////////////////////////////////////

            that.task = ko.mapping.fromJS({});
            that.mode = ko.observable('view'); //Can be "view" or "edit"
            that.users = ko.mapping.fromJS([]); //Users to assign the task

            // Public methods /////////////////////////////////////////////////////

            that.canActivate = function (taskId) {
                return taskService.getTask({
                    id: taskId
                }).done(function (data) {
                    ko.mapping.fromJS(data.task, that.task);

                    //This code is trying to find new assigned user from users and update task.assignedUser since it's not observable!
                    //TODO: Find a better way of doing it!!!
                    that.task.assignedUserId.subscribe(function (newValue) {
                        var userCount = that.users().length;
                        for (var i = 0; i < userCount; i++) {
                            var user = that.users()[i];
                            if (user.id() == newValue) {
                                that.task.assignedUser.id(user.id());
                                that.task.assignedUser.name(user.name());
                                that.task.assignedUser.surname(user.surname());
                                break;
                            }
                        }
                    });
                });
            };

            that.changeToEditMode = function () {
                if (gotUsers) {
                    that.mode('edit');
                    return;
                }

                friendshipService.getFriendships({
                    userId: session.getCurrentUser().id(),
                    canAssignTask: true
                }).done(function (result) {
                    var users = $.map(result.friendships, function (friendship) { return friendship.friend; });
                    ko.mapping.fromJS(users, that.users);
                    that.users.unshift(session.getCurrentUser());
                    that.mode('edit');
                    gotUsers = true;
                });
            };

            that.updateTask = function () {
                taskService.updateTask({
                    id: that.task.id(),
                    title: that.task.title(),
                    description: that.task.description(),
                    assignedUserId: that.task.assignedUserId(),
                    priority: that.task.priority(),
                    state: that.task.state(),
                }).done(function () {
                    //TODO: show a notification?
                    that.mode('view');
                });
            };
        };
    }
);