define(["jquery", "knockout", 'plugins/dialog', 'service!dto', 'service!taskever/task', 'service!taskever/friendship', 'session'],
    function($, ko, dialogs, dtos, taskService, friendshipService, session) {
        return function () {
            var that = this;

            // Public fields //////////////////////////////////////////////////////

            that.emailAddress = ko.observable('');
            
            // Public methods /////////////////////////////////////////////////////

            that.sendFriendshipRequest = function() {
                friendshipService.sendFriendshipRequest({
                    emailAddress: that.emailAddress()
                }).done(function () {
                    abp.notify.info("Sent friendship request!"); //TODO: Show notification!
                    dialogs.close(that);
                });
            };
        };
    });