define(["knockout", "service!abp/user"], function (ko, userService) {
    var currentUser = ko.mapping.fromJS({});
    
    var start = function () {
        return userService.getCurrentUserInfo({ }).done(function(data) {
            ko.mapping.fromJS(data.user, currentUser);
        });
    };

    return {
        start: start,
        getCurrentUser: function () {
            return currentUser;
        }
    };
});