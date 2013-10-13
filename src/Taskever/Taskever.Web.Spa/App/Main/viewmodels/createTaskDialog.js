define(["jquery", "knockout", 'plugins/dialog', 'service!dto', 'service!task', 'service!friendship', 'session'],
    function($, ko, dialogs, dtos, taskService, friendshipService, session) {
        var ctor = function() {
            this.task = new dtos.TaskDto();
            this.users = ko.mapping.fromJS([]);
        };

        ctor.prototype.activate = function() {
            var that = this;
            friendshipService.getMyFriends({ canAssignTask: true })
                .then(function(data) {
                    ko.mapping.fromJS(data, that.users);
                    that.users.unshift(session.getCurrentUser()); //Me !
                });
        };

        ctor.prototype.saveNewTask = function() {
            var that = this;
            taskService.createTask(ko.mapping.toJS(that.task))
                .then(function(data) {
                    dialogs.close(that, ko.mapping.fromJS(data));
                });
        };

        return ctor;
    });