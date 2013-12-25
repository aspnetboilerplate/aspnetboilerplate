define(
    ['durandal/app', "knockout", 'plugins/dialog', 'session', 'service!taskever/friendship'],
    function (app, ko, dialogs, session, friendshipService) {

        var maxFriendCount = 3;

        return function () {
            var that = this;

            // Private variables //////////////////////////////////////////////////

            var eventSubscriptions = [];

            // Public fields //////////////////////////////////////////////////////

            that.friendships = ko.mapping.fromJS([]);

            // Public methods /////////////////////////////////////////////////////

            that.activate = function () {
                friendshipService.getFriendshipsByMostActive({
                    maxResultCount: maxFriendCount
                }).then(function (data) {
                    ko.mapping.fromJS(data.friendships, that.friendships);
                });

                eventSubscriptions.push(app.on('te.friendship.visited', function (data) {
                    that.friendships.remove(function (friendship) {
                        return friendship.friend.id() == data.friend.id();
                    });
                    that.friendships.unshift({
                        friend: data.friend
                    });
                    if(that.friendships().length > maxFriendCount) {
                        that.friendships.pop();
                    }
                }));
                eventSubscriptions.push(app.on('te.friendship.delete', function (data) {
                    that.friendships.remove(function (friendship) {
                        return friendship.friend.id() == data.friend.id();
                    });
                }));

            };

            that.deactivate = function () {
                for (var i = 0; i < eventSubscriptions.length; i++) {
                    eventSubscriptions[i].off();
                }
            };

            that.showAddNewFriendDialog = function () {
                dialogs.show('viewmodels/friend/addFriendDialog');
            };
        };

    });
