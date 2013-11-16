define(['durandal/app', 'session', 'service!taskever/userActivity', 'knockout', 'underscore'],
    function (app, session, userActivityService, ko, _) {
        return function () {
            var that = this;

            var maxResultCountOnce = 2;

            var _userId;
            var _minShownActivityId = null;

            that.activities = ko.mapping.fromJS([]);
            that.hasMoreActivities = ko.observable(true);

            that.activate = function (activateData) {
                _userId = activateData.userId;
                that.loadActivities();
            };

            that.loadActivities = function () {
                userActivityService.getUserActivities({
                    userId: _userId,
                    maxResultCount: maxResultCountOnce,
                    beforeActivityId: _minShownActivityId
                }).done(function (data) {

                    if (data.activities.length > 0) {
                        _minShownActivityId = _.last(data.activities).id;
                    }

                    if (data.activities.length < maxResultCountOnce) {
                        that.hasMoreActivities(false);
                    }
                    
                    for (var j = 0; j < data.activities.length; j++) {//TODO: Find a way of appending new items to an existing item!
                        that.activities.push(ko.mapping.fromJS(data.activities[j]));
                    }
                });
            };
        };
    });