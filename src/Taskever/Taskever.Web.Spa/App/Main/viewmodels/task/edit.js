define(
    ['durandal/app', 'jquery', 'underscore', 'service!taskever/task', 'service!taskever/friendship', 'session', 'plugins/history'],
    function (app, $, _, taskService, friendshipService, session, history) {

        return function () {
            var that = this;

            // Private fields /////////////////////////////////////////////////////

            var $view;

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

                        that.task.title(decodeHtml(that.task.title()));
                        that.task.description(decodeHtml(that.task.description()));
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

            that.attached = function(view, parent) {
                $view = $(view);
            };

            that.updateTask = function () {
                abp.ui.setBusy($view, {
                    promise: taskService.updateTask({
                        id: that.task.id(),
                        title: that.task.title(),
                        description: that.task.description(),
                        assignedUserId: that.task.assignedUserId(),
                        priority: that.task.priority(),
                        privacy: that.task.privacy(),
                        state: that.task.state(),
                    }).done(function() {
                        abp.notify.info('Task has been updated', 'Updated');
                        history.navigate('task/' + that.task.id());
                        app.trigger('te.task.update', {
                            task: that.task
                        });
                    })
                });
            };

            that.cancelUpdate = function () {
                history.navigateBack();
            };
            
            //Private functions
            
            //This method is copied from http://stackoverflow.com/questions/5796718/html-entity-decode
            //TODO: Replace to a nicer one?
            var decodeHtml = (function () {
                // this prevents any overhead from creating the object each time
                var element = document.createElement('div');

                function decodeHtmlEntities(str) {
                    if (str && typeof str === 'string') {
                        // strip script/html tags
                        str = str.replace(/<script[^>]*>([\S\s]*?)<\/script>/gmi, '');
                        str = str.replace(/<\/?\w(?:[^"'>]|"[^"]*"|'[^']*')*>/gmi, '');
                        element.innerHTML = str;
                        str = element.textContent;
                        element.textContent = '';
                    }

                    return str;
                }

                return decodeHtmlEntities;
            })();
        };
    }
);