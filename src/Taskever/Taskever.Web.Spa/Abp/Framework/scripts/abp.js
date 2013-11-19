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

    abp.log.currentLevel = abp.log.levels.INFO;

    abp.log.log = function (logObject, logLevel) {
        if (!window.console || !window.console.log) {
            return;
        }

        if (logLevel != undefined && logLevel < abp.log.currentLevel) {
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

    abp.notify.info = function () {
        abp.log.warn('abp.notify.info is not implemented!');
    };

    /* MESSAGE **************************************************/

    abp.message = {};

    abp.message.types = {
        INFO: 1,
        WARN: 2,
        ERROR: 3,
    };

    abp.message.show = function (type, message, title) {
        abp.log.warn('abp.message.show is not implemented! I shows alert as default!');
        alert((title || '') + "! " + message);
    };

    abp.message.info = function (message, title) {
        abp.message.show(abp.message.types.INFO, message, title);
    };

    abp.message.warn = function (message, title) {
        abp.message.show(abp.message.types.WARN, message, title);
    };

    abp.message.error = function (message, title) {
        abp.message.show(abp.message.types.ERROR, message, title);
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