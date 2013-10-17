define(['durandal/app'], function (app) {

    var _userId;
    var _tabId;

    return {
        activate: function (userId, tabId) {
            _userId = userId;
            _tabId = tabId;
        },

        attached: function (view, parent) {
            $('#myTab').tab();
            $('#myTab a').click(function (e) {
                e.preventDefault();
                $(this).tab('show');
            });

            if (_tabId) {
                $('#myTab a[href=#' + _tabId + ']').tab('show');
            } else {
                $('#myTab a:first').tab('show');
            }
        }
    };
});