define(['durandal/app', 'session', 'service!taskever/userActivity'], function (app, session, userActivityService) {
    var activities = ko.mapping.fromJS([]);

    return {
        activities: activities,
        activate: function () {
            userActivityService.getFallowedActivities({
                fallowerUserId: session.getCurrentUser().id()
            }).then(function (data) {
                ko.mapping.fromJS(data.activities, activities);
            });
        }
    };
});