define(
    ["jquery", "knockout", 'durandal/app', 'plugins/dialog', 'plugins/history', 'service!taskever/friendship', 'session'],
    function ($, ko, app, dialogs, history, friendshipService, session) {

        var _defaultUrlAgs = {
            activeSection: 'MyFriends'
        };

        return function () {
            var that = this;

            // Private variables //////////////////////////////////////////////////

            var _urlArgs;

            // Public fields //////////////////////////////////////////////////////

            that.friendships = ko.mapping.fromJS([]);

            // Public methods /////////////////////////////////////////////////////

            that.activate = function (urlArgs) { //TODO: Make this in canActivate?
                _urlArgs = $.extend({}, _defaultUrlAgs, urlArgs);
                friendshipService.getFriendships({
                    userId: session.getCurrentUser().id()
                }).done(function (data) {
                    ko.mapping.fromJS(data.friendships, that.friendships);
                });
            };

            that.attached = function () {
                $('#FriendshipTabs').tab();
                $('#FriendshipTabs a').click(function (e) {
                    e.preventDefault();
                    $(this).tab('show');

                    //TODO: This is experimental now, find a more proper way of doing this!
                    history.navigate('friends?activeSection=' + $(this).attr('href').substr(1), {
                        trigger: false
                    });
                });

                $('#FriendshipTabs a[href=#' + _urlArgs.activeSection + ']').tab('show');

                //TODO: DRY for codes below!

                //TODO: Prevent multiple-click (block ui?)!
                $('#MyFriends').on('change', 'input.checkbox-friendship-canAssignTask', function () {
                    var friendship = ko.dataFor(this);
                    friendshipService.changeFriendshipProperties({
                        id: friendship.id(),
                        canAssignTask: friendship.canAssignTask()
                    });
                }); //TODO Do something on done?

                //TODO: Prevent multiple-click (block ui?)!
                $('#MyFriends').on('change', 'input.checkbox-friendship-fallowActivities', function () {
                    var friendship = ko.dataFor(this);
                    friendshipService.changeFriendshipProperties({
                        id: friendship.id(),
                        fallowActivities: friendship.fallowActivities()
                    }); //TODO Do something on done?
                });
            };

            that.showAddNewFriendDialog = function () {
                dialogs.show('viewmodels/addFriendDialog');
            };

            that.removeFriendship = function (friendship) {
                friendshipService.removeFriendship({
                    id: friendship.id()
                }).done(function () {
                    alert('removed!'); //TODO: Show notification!
                    that.friendships.remove(friendship);
                });
            };

            that.acceptFriendship = function (friendship) {
                friendshipService.acceptFriendship({
                    id: friendship.id()
                }).done(function () {
                    alert('accepted!'); //TODO: Show notification!
                    friendship.status(2);
                });
            };

            that.rejectFriendship = function (friendship) {
                friendshipService.rejectFriendship({
                    id: friendship.id()
                }).done(function () {
                    alert('rejected!'); //TODO: Show notification!
                    that.friendships.remove(friendship);
                });
            };
            
            that.cancelFriendshipRequest = function (friendship) {
                friendshipService.cancelFriendshipRequest({
                    id: friendship.id()
                }).done(function () {
                    alert('cancelled!'); //TODO: Show notification!
                    that.friendships.remove(friendship);
                });
            };

            // Private methods ////////////////////////////////////////////////

        };

    }
);