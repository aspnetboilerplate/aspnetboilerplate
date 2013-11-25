define(
    ['jquery', 'plugins/history', 'service!abp/user', 'service!taskever/friendship'],
    function ($, history, userService, friendshipService) {

        var _defaultUrlAgs = {
            activeSection: 'UserActivities'
        };

        return function () {
            var that = this;

            // Private variables //////////////////////////////////////////////////

            var _urlArgs;

            // Public fields //////////////////////////////////////////////////////

            that.user = ko.mapping.fromJS({});

            // Public methods /////////////////////////////////////////////////////

            that.canActivate = function (userId, urlArgs) {
                _urlArgs = $.extend({}, _defaultUrlAgs, urlArgs);
                return userService.getUser({
                    userId: userId
                }).done(function (data) {
                    ko.mapping.fromJS(data.user, that.user);
                    friendshipService.updateLastVisitTime({
                        friendUserId: userId
                    });
                });
            };

            that.attached = function (view, parent) {
                $('#UserViewTabs').tab();
                $('#UserViewTabs a').click(function (e) {
                    e.preventDefault();
                    $(this).tab('show');

                    //TODO: This is experimental now, find a more proper way of doing this!
                    history.navigate('user/' + that.user().id() + '?activeSection=' + $(this).attr('href').substr(1), {
                        trigger: false
                    });
                });

                $('#UserViewTabs a[href=#' + _urlArgs.activeSection + ']').tab('show');
            };
        };

    }
);