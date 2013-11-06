define(["knockout", 'service!dto', "service!abp/user"], function (ko, dtos, userService) {
    var currentUser = ko.mapping.fromJS({});
    
    var start = function () {
        return userService.getCurrentUserInfo({ }).done(function(data) {
            ko.mapping.fromJS(data.user, currentUser);
            console.log(currentUser);
        });
    };

    return {
        start: start,
        getCurrentUser: function () {
            return currentUser;
        }
    };
});