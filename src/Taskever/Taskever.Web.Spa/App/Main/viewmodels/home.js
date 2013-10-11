define(['durandal/app', 'services/userActivity', 'durandal/composition'], function (app, userActivityService, composition) {
    var activities = ko.mapping.fromJS([]);

    return {
        activities: activities,
        activate: function () {
            userActivityService.getFallowedActivities({
                fallowerUserId: 1 //TODO: Must be the current user!
            }).then(function (data) {
                ko.mapping.fromJS(data.activities, activities);
                console.log(activities);
            });
        }
    };
});