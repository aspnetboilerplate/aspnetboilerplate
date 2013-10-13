define(["knockout", 'service!dto', "service!user"], function (ko, dtos, userService) {
    var currentUser = ko.mapping.fromJS({});
    
    var start = function () {
        return userService.getCurrentUserInfo({ })
            .then(function(data) {
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