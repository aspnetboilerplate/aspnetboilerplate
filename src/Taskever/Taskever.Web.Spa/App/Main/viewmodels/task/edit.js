define(
    ['jquery', 'underscore', 'service!taskever/task', 'service!taskever/friendship', 'session', 'plugins/history', 'plugins/dialog'],
    function ($, _, taskService, friendshipService, session, history, dialogs) {

        return function () {
            var that = this;

            // Public fields //////////////////////////////////////////////////////

            that.task = ko.mapping.fromJS({});
            that.users = ko.mapping.fromJS([]); //Users to assign the task

            // Public methods /////////////////////////////////////////////////////

            that.canActivate = function (taskId) {
                return $.Deferred(function (dfd) {
                    taskService.getTask({
                        id: taskId
                    }).done(function (data) {
                        ko.mapping.fromJS(data.task, that.task);

                        that.task.privacyEditable = ko.observable(that.task.assignedUserId() == session.getCurrentUser().id());
                        that.task.assignedUserId.subscribe(function (newValue) {
                            if (newValue == session.getCurrentUser().id()) {
                                that.task.privacyEditable(true);
                            } else {
                                that.task.privacyEditable(false);
                                that.task.privacy(taskever.taskPrivacy.Protected);
                            }
                        });

                    }).fail(function () {
                        dfd.resolve(false);
                    }).then(function () {
                        friendshipService.getFriendships({
                            userId: session.getCurrentUser().id(),
                            canAssignTask: true
                        }).done(function (result) {
                            var users = $.map(result.friendships, function (friendship) { return friendship.friend; });
                            ko.mapping.fromJS(users, that.users);
                            that.users.unshift(session.getCurrentUser());
                            dfd.resolve(true);
                        }).fail(function () {
                            dfd.resolve(false);
                        });
                    });
                });
            };

            that.updateTask = function () {
                taskService.updateTask({
                    id: that.task.id(),
                    title: that.task.title(),
                    description: that.task.description(),
                    assignedUserId: that.task.assignedUserId(),
                    priority: that.task.priority(),
                    privacy: that.task.privacy(),
                    state: that.task.state(),
                }).done(function () {
                    abp.notify.info('Task has been updated', 'Updated');
                    history.navigate('task/' + that.task.id());
                });
            };

            that.cancelUpdate = function () {
                history.navigateBack();
            };
        };
    }
);