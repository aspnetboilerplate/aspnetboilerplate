var abp = abp || {};
(function () {

    abp.log = { };

    abp.log.log = function (logObject) {
        if (!window.console || !window.console.log) {
            return;
        }

        console.log(logObject);
    };

    abp.log.info = function (logObject) {
        abp.log.log("INFO: ");
        abp.log.log(logObject);
    };

    abp.notify = {};

    abp.notify.info = function () {
        abp.log.info('abp.notify.info is not implemented!');
    };

})();