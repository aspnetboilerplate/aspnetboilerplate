var abp = abp || {};
(function ($) {
    if (!sweetAlert || !$) {
        return;
    }

    /* DEFAULTS *************************************************/

    abp.libs = abp.libs || {};
    abp.libs.sweetAlert = {
        config: {
            'default': {

            },
            info: {
                icon: 'info'
            },
            success: {
                icon: 'success'
            },
            warn: {
                icon: 'warning'
            },
            error: {
                icon: 'error'
            },
            confirm: {
                icon: 'warning',
                title: 'Are you sure?',
                buttons: ['Cancel', 'Yes']
            }
        }
    };

    /* MESSAGE **************************************************/

    var showMessage = function (type, message, title, isHtml, options) {

        if (!title) {
            title = message;
            message = undefined;
        }

        var messageContent = {
            title: title
        };

        if (isHtml) {
            var el = document.createElement('div');
            el.innerHTML = message;

            messageContent.content = el;
        } else {
            messageContent.text = message;
        }

        var opts = $.extend(
            {},
            abp.libs.sweetAlert.config['default'],
            abp.libs.sweetAlert.config[type],
            messageContent,
            options
        );

        return $.Deferred(function ($dfd) {
            sweetAlert(opts).then(function () {
                $dfd.resolve();
            });
        });
    };

    abp.message.info = function (message, title, isHtml, options) {
        return showMessage('info', message, title, isHtml, options);
    };

    abp.message.success = function (message, title, isHtml, options) {
        return showMessage('success', message, title, isHtml, options);
    };

    abp.message.warn = function (message, title, isHtml, options) {
        return showMessage('warn', message, title, isHtml, options);
    };

    abp.message.error = function (message, title, isHtml, options) {
        return showMessage('error', message, title, isHtml, options);
    };

    abp.message.confirm = function (message, titleOrCallback, callback, isHtml, options) {
        var messageContent;

        if (isHtml) {
            var el = document.createElement('div');
            el.innerHTML = message;

            messageContent = {
                content: el
            }
        } else {
            messageContent = {
                text: message
            }
        }

        if ($.isFunction(titleOrCallback)) {
            callback = titleOrCallback;
        } else if (titleOrCallback) {
            messageContent.title = titleOrCallback;
        };

        var opts = $.extend(
            {},
            abp.libs.sweetAlert.config['default'],
            abp.libs.sweetAlert.config.confirm,
            messageContent,
            options
        );

        return $.Deferred(function ($dfd) {
            sweetAlert(opts).then(function (isConfirmed) {
                callback && callback(isConfirmed);
                $dfd.resolve(isConfirmed);
            });
        });
    };

    abp.event.on('abp.dynamicScriptsInitialized', function () {
        abp.libs.sweetAlert.config.confirm.title = abp.localization.abpWeb('AreYouSure');
        abp.libs.sweetAlert.config.confirm.buttons = [abp.localization.abpWeb('Cancel'), abp.localization.abpWeb('Yes')];
    });

})(jQuery);