define(['durandal/app', 'session', 'service!taskever/userActivity', 'knockout'], function (app, session, userActivityService, ko) {
    return function () {
        var that = this;

        that.activities = ko.mapping.fromJS([]);

        that.activate = function (activateData) {
            userActivityService.getUserActivities({
                userId: activateData.userId
            }).done(function (data) {

                //TODO: Make a general mechanism for live time!
                //Add liveTime to all activities
                for (var i = 0; i < data.activities.length; i++) {
                    data.activities[i].liveTime = '';
                }
                
                ko.mapping.fromJS(data.activities, that.activities);

                //Live times! 
                var refreshTimes = function() {
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