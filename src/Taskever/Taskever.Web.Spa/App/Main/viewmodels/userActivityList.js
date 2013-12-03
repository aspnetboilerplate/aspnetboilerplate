define(['durandal/app', 'session', 'service!taskever/userActivity', 'knockout', 'underscore'],
    function (app, session, userActivityService, ko, _) {

        var _maxResultCountOnce = 20;

        return function () {
            var that = this;

            // Private variables //////////////////////////////////////////////////

            var _userId;
            var _minShownId = null;

            // Public fields //////////////////////////////////////////////////////
            
            that.activities = ko.mapping.fromJS([]);
            that.hasMoreActivities = ko.observable(true);

            // Public methods /////////////////////////////////////////////////////
            
            that.activate = function (activateData) {
                _userId = activateData.userId;
                that.loadActivities();
            };

            that.loadActivities = function () {
                userActivityService.getFollowedActivities({
                    userId: _userId,
                    isActor: true,
                    beforeId: _minShownId,
                    maxResultCount: _maxResultCountOnce
                }).done(function (data) {
                    if (data.activities.length > 0) {
                        _minShownId = _.last(data.activities).id;
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