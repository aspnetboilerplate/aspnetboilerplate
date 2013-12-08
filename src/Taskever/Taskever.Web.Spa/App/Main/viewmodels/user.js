define(
    ['jquery', 'plugins/history', 'service!taskever/friendship', 'service!taskever/user', 'plugins/dialog'],
    function ($, history, friendshipService, taskeverUserService, dialog) {

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
                return $.Deferred(function (dfd) {
                    taskeverUserService.getUserProfile({
                        userId: userId
                    }).done(function (result) {
                        if (result.canNotSeeTheProfile) {
                            dfd.resolve(false);
                            if (result.sentFriendshipRequest) {
                                abp.message.info('There is a waiting friendship request! You can see the profile of the ' + result.user.name + ' after this request is accepted.' );
                            } else {
                                dialog.show('viewmodels/friend/addFriendDialog', {
                                    emailAddress: result.user.emailAddress,
                                    message: 'You are not friend with ' + result.user.name + ', you can send a friendship request.'
                                });
                            }
                        } else {
                            ko.mapping.fromJS(result.user, that.user);
                            dfd.resolve(true);
                            friendshipService.updateLastVisitTime({ friendUserId: userId });
                        }
                    }).fail(function () {
                        dfd.resolve(false);
                    });
                });
            };

            that.attached = function (view, parent) {
                $('#UserViewTabs').tab();
                $('#UserViewTabs a').click(function (e) {
                    e.preventDefault();
                    $(this).tab('show');

                    //TODO: This is experimental now, find a more proper way of doing this!
                    history.navigate('user/' + that.user.id() + '?activeSection=' + $(this).attr('href').substr(1), {
                        trigger: false
                    });
                });

                $('#UserViewTabs a[href=#' + _urlArgs.activeSection + ']').tab('show');
            };
        };

    }
);