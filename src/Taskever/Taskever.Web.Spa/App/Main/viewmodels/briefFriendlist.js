define(
    ["jquery", "knockout", 'durandal/app', 'plugins/dialog', 'service!taskever/friendship', 'session'],
    function ($, ko, app, dialogs, friendshipService, session) {
        
    var friends = ko.mapping.fromJS([]);

    //var message = L("abp.message");

    return {
        friends: friends,
        currentUser: session.getCurrentUser(),
        activate: function () {
            friendshipService.getMyFriends({})
                .then(function (data) {
                    ko.mapping.fromJS(data, friends);
                });
        }
    };
});
