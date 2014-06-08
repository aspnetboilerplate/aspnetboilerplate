var abp = abp || {};
(function () {

    if (!toastr) {
        return;
    }

    /* DEFAULTS *************************************************/

    toastr.options.positionClass = 'toast-bottom-right';

    /* NOTIFICATION *********************************************/

    var showNotification = function (type, message, title) {
        toastr[type](message, title);
    };

    abp.notify.success = function (message, title) {
        showNotification('success', message, title);
    };

    abp.notify.info = function (message, title) {
        showNotification('info', message, title);
    };

    abp.notify.warn = function (message, title) {
        showNotification('warning', message, title);
    };

    abp.notify.error = function (message, title) {
        showNotification('error', message, title);
    };

})();