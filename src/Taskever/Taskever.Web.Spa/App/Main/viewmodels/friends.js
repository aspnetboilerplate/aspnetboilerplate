define(["jquery", "knockout", 'durandal/app', 'plugins/dialog', 'service!friendship'], function ($, ko, app, dialogs, friendshipService) {
    var friends = ko.mapping.fromJS([]);

    //var message = L("abp.message");

    return {
        friends: friends,

        activate: function () {
            friendshipService.getMyFriends({})
                .then(function (data) {
                    ko.mapping.fromJS(data, friends);
                });
        }
    };
});