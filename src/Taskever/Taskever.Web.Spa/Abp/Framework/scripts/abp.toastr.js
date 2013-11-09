var abp = abp || {};
(function () {

    if (!toastr) {
        return;
    }

    var showNotification = function (type, title, message) {
        toastr[type](title, message);
    };

    abp.notify.info = function (title, message) {
        showNotification('info', title, message);
    };

})();