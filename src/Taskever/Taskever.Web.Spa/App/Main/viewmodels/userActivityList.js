define(['durandal/app', 'session', 'service!taskever/userActivity', 'knockout'], function (app, session, userActivityService, ko) {
    return function () {
        var that = this;

        var maxResultCountOnce = 20;

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
                    _minShownActivityId = data.activities[data.activities.length - 1].id;
                }
                
                if (data.activities.length < maxResultCountOnce) {
                    that.hasMoreActivities(false);
                }

                //TODO: Make a general mechanism for live time!
                //Add liveTime to all activities
                for (var i = 0; i < data.activities.length; i++) {
                    data.activities[i].liveTime = '';
                }

                //TODO: Find a way of appending new items to an existing item!
                for (var j = 0; j < data.activities.length; j++) {
                    that.activities.push(ko.mapping.fromJS(data.activities[j]));
                }

                //Live times! 
                var refreshTimes = function () {
                    //TODO: Optimize
                    //TODO: Use moment.js as dependency!
                    //TODO: Show exact time on mouse over (as tooltip/title)!
                    for (var i = 0; i < that.activities().length; i++) {
                        that.activities()[i].liveTime(moment(that.activities()[i].creationTime()).fromNow());
                    }
                    setTimeout(refreshTimes, 1000);
                };

                refreshTimes();
            });
        };
    };
});