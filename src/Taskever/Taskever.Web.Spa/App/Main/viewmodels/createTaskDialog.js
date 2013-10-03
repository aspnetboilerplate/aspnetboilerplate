define(["jquery", "knockout", 'durandal/app', 'plugins/dialog', 'models/dtos', 'services/task', 'services/friendship', 'session'],
    function($, ko, app, dialogs, dtos, taskService, friendshipService, session) {
        var ctor = function() {
            this.task = new dtos.TaskDto();
            this.users = ko.mapping.fromJS([]);
            this.currentUser = session.getCurrentUser();
            console.log(this.currentUser);
            console.log(this.currentUser.id());
        };

        ctor.prototype.activate = function() {
            var that = this;
            friendshipService.getMyFriends({ canAssignTask: true })
                .then(function(data) {
                    ko.mapping.fromJS(data, that.users);
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