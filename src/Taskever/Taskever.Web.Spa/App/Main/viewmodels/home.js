define(['durandal/app', 'session', 'service!taskever/userActivity'], function (app, session, userActivityService) {
    return function (){
        var that = this;

        that.activities = ko.mapping.fromJS([]);
        
        that.activate = function() {
            userActivityService.getFallowedActivities({
                fallowerUserId: session.getCurrentUser().id()
            }).done(function(data) {
                ko.mapping.fromJS(data.activities, that.activities);
            });
        };
    };
});