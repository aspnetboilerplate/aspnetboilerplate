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

    var showMessage = function (type, message, title, callback, options) {
        options = options || {};
        var messageContent = {};
        if(title){
            messageContent.title = title;
        }

        options.reverseButtons = true;

        if (options.isHtml) {
            options.html = message;
        } else {
            options.text = message;
        }

        var opts = $.extend(
            {},
            abp.libs.sweetAlert.config['default'],
            abp.libs.sweetAlert.config[type],
            messageContent,
            options
        );

        return $.Deferred(($dfd) => {
            Swal.fire(opts).then((result) => {
                callback && callback(result);
                $dfd.resolve(result)
            });
        });
    };

    abp.message.info = function (message, title, options) {
        return showMessage('info', message, title, null, options);
    };

    abp.message.success = function (message, title, options) {
        return showMessage('success', message, title, null, options);
    };

    abp.message.warn = function (message, title, options) {
        return showMessage('warn', message, title, null, options);
    };

    abp.message.error = function (message, title, options) {
        return showMessage('error', message, title, null, options);
    };

    abp.message.confirm = function (message, title, callback, options) {
        options = options || {};
        options.showCancelButton = true;

        const confirmFunc = (result) => {
            let isCancelled = result.dismiss === Swal.DismissReason.cancel;

            return callback && callback(result.isConfirmed, isCancelled);
        }

        return showMessage('confirm', message, title, confirmFunc, options);
    };

    abp.event.on('abp.dynamicScriptsInitialized', function () {
        abp.libs.sweetAlert.config.confirm.title = abp.localization.abpWeb('AreYouSure');
        abp.libs.sweetAlert.config.confirm.confirmButtonText = abp.localization.abpWeb('Yes');
        abp.libs.sweetAlert.config.confirm.cancelButtonText = abp.localization.abpWeb('Cancel');
        abp.libs.sweetAlert.config.confirm.denyButtonText = abp.localization.abpWeb('No');
    });

})(jQuery);