var abp = abp || {};
(function () {

    if (!toastr) {
        return;
    }

    /* DEFAULTS *************************************************/

    toastr.options.positionClass = 'toast-bottom-right';

    /* NOTIFICATION *********************************************/

    var showNotification = function (type, title, message) {
        toastr[type](title, message);
    };

    abp.notify.success = function (title, message) {
        showNotification('success', title, message);
    };

    abp.notify.info = function (title, message) {
        showNotification('info', title, message);
    };

    abp.notify.warn = function (title, message) {
        showNotification('warning', title, message);
    };

    abp.notify.error = function (title, message) {
        showNotification('error', title, message);
    };

})();