var abp = abp || {};
(function () {

    /* LOG */
    
    abp.log = {};

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

    abp.log.warn = function (logObject) {
        abp.log.log("WARN: ");
        abp.log.log(logObject);
    };
    
    /* NOTIFICATION API */
    
    abp.notify = {};

    abp.notify.info = function () {
        abp.log.warn('abp.notify.info is not implemented!');
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