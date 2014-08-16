var abp = abp || {};
(function () {

    if (!$.blockUI) {
        return;
    }

    $.extend($.blockUI.defaults, {
        message: ' ',
        css: { },
        overlayCSS: {
            backgroundColor: '#AAA',
            opacity: 0.3,
            cursor: 'wait'    
        }
    });
    
    abp.ui.block = function (elm) {
        if (!elm) {
            $.blockUI();
        } else {
            $(elm).block();
        }
    };

    abp.ui.unblock = function (elm) {
        if (!elm) {
            $.unblockUI();
        } else {
            $(elm).unblock();
        }
    };

})();