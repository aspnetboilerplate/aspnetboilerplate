define(['durandal/app'], function (app) {
    return {
        attached: function (view, parent) {
            $('#myTab').tab();
            $('#myTab a').click(function (e) {
                e.preventDefault();
                $(this).tab('show');
            });
        }
    };
});