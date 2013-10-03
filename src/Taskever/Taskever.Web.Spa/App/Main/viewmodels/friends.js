define(["jquery", "knockout", 'durandal/app', 'plugins/dialog', 'models/dtos', 'services/friendship'], function ($, ko, app, dialogs, dtos, friendshipService) {
    var friends = ko.mapping.fromJS([]);

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