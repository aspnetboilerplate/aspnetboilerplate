define(['durandal/app', 'session', 'service!taskever/userActivity'], function (app, session, userActivityService) {

    var _maxResultCountOnce = 20;

    return function () {
        var that = this;

        var _minShownActivityId = null;

        that.activities = ko.mapping.fromJS([]);
        that.hasMoreActivities = ko.observable(true);

        that.activate = function () {
            that.loadActivities();
        };

        that.loadActivities = function () {
            userActivityService.getFollowedActivities({
                userId: session.getCurrentUser().id(),
                beforeId: _minShownActivityId,
                maxResultCount: _maxResultCountOnce,
            }).done(function (data) {
                if (data.activities.length > 0) {
                    _minShownActivityId = _.last(data.activities).id;
                }

                if (data.activities.length < _maxResultCountOnce) {
                    that.hasMoreActivities(false);
                }

                for (var j = 0; j < data.activities.length; j++) {//TODO: Find a way of appending new items to an existing item!
                    that.activities.push(ko.mapping.fromJS(data.activities[j]));
                }
            });
        };
    };
});