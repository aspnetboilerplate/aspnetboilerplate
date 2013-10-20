define(
    ['jquery', 'plugins/history', 'service!abp/user', 'service!dto'],
    function ($, history, userService, dtos) {

        var _defaultUrlAgs = {
            activeSection: 'UserActivities'
        };

        return function () {
            var that = this;

            // Private variables //////////////////////////////////////////////////

            var _urlArgs;

            // Public fields //////////////////////////////////////////////////////

            that.userId = null;
            that.user = ko.mapping.fromJS(new dtos.abp.user.UserDto()); //TODO: Direkt dönüş değeri olmayanlar için namespace problemi var!!! Hatta hepsi için!

            // Public methods /////////////////////////////////////////////////////

            that.activate = function (userId, urlArgs) {
                that.userId = userId;
                _urlArgs = $.extend({}, _defaultUrlAgs, urlArgs);

                userService.getUser({
                    userId: userId
                }).then(function (data) {
                    ko.mapping.fromJS(data.user, that.user);
                });

            };

            that.attached = function (view, parent) {
                $('#UserViewTabs').tab();
                $('#UserViewTabs a').click(function (e) {
                    e.preventDefault();
                    $(this).tab('show');

                    //TODO: This is experimental now, find a more proper way of doing this!
                    history.navigate('user/' + that.userId + '?activeSection=' + $(this).attr('href').substr(1), {
                        trigger: false
                    });
                });

                $('#UserViewTabs a[href=#' + _urlArgs.activeSection + ']').tab('show');
            };
        };

    }
);