define(
    ["jquery", "knockout", 'durandal/app', 'plugins/dialog', 'plugins/history', 'service!taskever/friendship'],
    function ($, ko, app, dialogs, history, friendshipService) {

        var _defaultUrlAgs = {
            activeSection: 'MyFriends'
        };
        
        return function () {
            var that = this;

            // Private variables //////////////////////////////////////////////////

            var _urlArgs;
            
            // Public fields //////////////////////////////////////////////////////

            that.friends = ko.mapping.fromJS([]);

            // Public methods /////////////////////////////////////////////////////

            that.activate = function (urlArgs) {
                _urlArgs = $.extend({}, _defaultUrlAgs, urlArgs);
                friendshipService.getMyFriends({
                    
                }).then(function(data, response) {
                    ko.mapping.fromJS(data, that.friends);
                    console.log(response);
                });
            };
            
            that.attached = function (view, parent) {
                $('#FriendshipTabs').tab();
                $('#FriendshipTabs a').click(function (e) {
                    e.preventDefault();
                    $(this).tab('show');

                    //TODO: This is experimental now, find a more proper way of doing this!
                    history.navigate('friends?activeSection=' + $(this).attr('href').substr(1), {
                        trigger: false
                    });
                });

                $('#FriendshipTabs a[href=#' + _urlArgs.activeSection + ']').tab('show');
            };
        };

    }
);