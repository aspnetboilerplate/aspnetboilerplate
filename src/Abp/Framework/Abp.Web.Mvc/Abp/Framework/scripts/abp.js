var abp = abp || {};
(function () {

    /* LOGGING ***************************************************/

    abp.log = {};

    abp.log.levels = {
        DEBUG: 1,
        INFO: 2,
        WARN: 3,
        ERROR: 4,
        FATAL: 5
    };

    abp.log.level = abp.log.levels.DEBUG;

    abp.log.log = function (logObject, logLevel) {
        if (!window.console || !window.console.log) {
            return;
        }

        if (logLevel != undefined && logLevel < abp.log.level) {
            return;
        }

        console.log(logObject);
    };

    abp.log.debug = function (logObject) {
        abp.log.log("DEBUG: ", abp.log.levels.DEBUG);
        abp.log.log(logObject, abp.log.levels.DEBUG);
    };

    abp.log.info = function (logObject) {
        abp.log.log("INFO: ", abp.log.levels.INFO);
        abp.log.log(logObject, abp.log.levels.INFO);
    };

    abp.log.warn = function (logObject) {
        abp.log.log("WARN: ", abp.log.levels.WARN);
        abp.log.log(logObject, abp.log.levels.WARN);
    };

    abp.log.error = function (logObject) {
        abp.log.log("ERROR: ", abp.log.levels.ERROR);
        abp.log.log(logObject, abp.log.levels.ERROR);
    };

    abp.log.fatal = function (logObject) {
        abp.log.log("FATAL: ", abp.log.levels.FATAL);
        abp.log.log(logObject, abp.log.levels.FATAL);
    };

    /* NOTIFICATION *********************************************/

    abp.notify = {};

    abp.notify.success = function (title, message) {
        abp.log.warn('abp.notify.success is not implemented!');
    };

    abp.notify.info = function (title, message) {
        abp.log.warn('abp.notify.info is not implemented!');
    };

    abp.notify.warn = function (title, message) {
        abp.log.warn('abp.notify.warn is not implemented!');
    };

    abp.notify.error = function (title, message) {
        abp.log.warn('abp.notify.error is not implemented!');
    };

    /* MESSAGE **************************************************/

    abp.message = {};

    abp.message.info = function (message, title) {
        abp.log.warn('abp.message.info is not implemented!');
        alert((title || '') + ' ' + message);
    };

    abp.message.warn = function (message, title) {
        abp.log.warn('abp.message.warn is not implemented!');
        alert((title || '') + ' ' + message);
    };

    abp.message.error = function (message, title) {
        abp.log.warn('abp.message.error is not implemented!');
        alert((title || '') + ' ' + message);
    };

    /* UI *******************************************************/

    abp.ui = {};

    /* UI BLOCK */

    abp.ui.block = function (elm) {
        abp.log.warn('abp.ui.block is not implemented!');
    };

    abp.ui.unblock = function (elm) {
        abp.log.warn('abp.ui.unblock is not implemented!');
    };

    /* UI BUSY */

    abp.ui.setBusy = function (elm, options) {
        abp.log.warn('abp.ui.setBusy is not implemented!');
    };

    abp.ui.clearBusy = function (elm) {
        abp.log.warn('abp.ui.clearBusy is not implemented!');
    };

})();