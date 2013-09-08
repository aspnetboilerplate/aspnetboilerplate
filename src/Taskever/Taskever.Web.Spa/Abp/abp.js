define(['jquery'], function ($) {

    var ajax = function (opts) {
        var defaultOpts = {
            dataType: 'json',
            contentType: 'application/json'
        };

        var defer = $.Deferred();
        var mopts = $.extend({}, defaultOpts, opts);
        $.ajax(mopts)
            .then(function (data) {
                if (data.Message) {
                    defer.reject();
                    return;
                }

                if (mopts.processData) {
                    data = mopts.processData(data);
                }

                defer.resolve(data);
            }).fail(function () {
                defer.reject();
            });

        return defer.promise();
    };

    return {
        ajax: ajax
    };

});