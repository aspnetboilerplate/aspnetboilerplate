define(
    ['jquery', 'plugins/history'],
    function($, history) {

        var _defaultUrlAgs = {
            activeSection: 'UserActivities'
        };

        return function() {
            var that = this;


            // Private variables //////////////////////////////////////////////////

            var _urlArgs;

            // Public fields //////////////////////////////////////////////////////

            that.userId = null;

            // Public methods /////////////////////////////////////////////////////

            that.activate = function (userId, urlArgs) {
                that.userId = userId;
                _urlArgs = $.extend({ }, _defaultUrlAgs, urlArgs);
            };

            that.attached = function (view, parent) {
                $('#myTab').tab();
                $('#myTab a').click(function(e) {
                    e.preventDefault();
                    $(this).tab('show');

                    //TODO: This is experimental now, find a more proper way of doing this!
                    history.navigate('user/' + that.userId + '?activeSection=' + $(this).attr('href').substr(1), {
                        trigger: false
                    });
                });

                $('#myTab a[href=#' + _urlArgs.activeSection + ']').tab('show');
            };
        };

    }
);