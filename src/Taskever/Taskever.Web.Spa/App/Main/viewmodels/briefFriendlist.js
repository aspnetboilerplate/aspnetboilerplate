define(
    ["knockout", 'session', 'service!taskever/friendship'],
    function (ko, session, friendshipService) {
        
    return function () {
        var that = this;

        // Private variables //////////////////////////////////////////////////

        // Public fields //////////////////////////////////////////////////////

        that.friendships = ko.mapping.fromJS([]);

        // Public methods /////////////////////////////////////////////////////

        that.activate = function () {
            friendshipService.getFriendships({
                userId: session.getCurrentUser().id(),
                status: 2 //Accepted TODO: Define enums in client side!
            }).then(function (data) {
                ko.mapping.fromJS(data.friendships, that.friendships);
            });
        };
    };

});
