var abp = abp || {};
(function ($) {
    if (!sweetAlert || !jQuery) {
        return;
    }

    /* DEFAULTS *************************************************/

    abp.libs = abp.libs || {};
    abp.libs.sweetAlert = {
        config: {
            'default': {

            },
            info: {
                type: 'info'
            },
            success: {
                type: 'success'
            },
            warn: {
                type: 'warning'
            },
            error: {
                type: 'error'
            },
            confirm: {
                type: 'warning',
                title: abp.localization.abpWeb('AreYouSure'),
                showCancelButton: true,
                cancelButtonText: abp.localization.abpWeb('Cancel'),
                confirmButtonColor: "#DD6B55",
                confirmButtonText: abp.localization.abpWeb('Yes')
            }
        }
    };

    /* MESSAGE **************************************************/

    var showMessage = function (type, message, title) {
        if (!title) {
            title = message;
            message = undefined;
        }

        var opts = $.extend(
            {},
            abp.libs.sweetAlert.config.default,
            abp.libs.sweetAlert.config[type],
            {
                title: title,
                text: message
            }
        );

        sweetAlert(opts);
    };

    abp.message.info = function (message, title) {
        showMessage('info', message, title);
    };

    abp.message.success = function (message, title) {
        showMessage('success', message, title);
    };

    abp.message.warn = function (message, title) {
        showMessage('warn', message, title);
    };

    abp.message.error = function (message, title) {
        showMessage('error', message, title);
    };

    abp.message.confirm = function (message, titleOrCallback, callback) {

        var userOpts = {
            text: message
        };

        if ($.isFunction(titleOrCallback)) {
            callback = titleOrCallback;
        } else if (titleOrCallback) {
            userOpts.title = titleOrCallback;
        };

        var opts = $.extend(
            {},
            abp.libs.sweetAlert.config.default,
            abp.libs.sweetAlert.config.confirm,
            userOpts
        );

        sweetAlert(opts, function (isConfirmed) {
            callback && callback(isConfirmed);
        });
    };

})(jQuery);