var abp = abp || {};
(function () {

    //TODO: Make blockUI optional?

    abp.ui.setBusy = function (elm, options) {
        if (!elm) {
            abp.ui.block();
            $('body').spin({
                lines: 11,
                length: 0,
                width: 10,
                radius: 20,
                corners: 1.0,
                trail: 60,
                speed: 1.2
            });
        } else {
            var $elm = $(elm);
            var $busyIndicator = $elm.find('.abp-busy-indicator');
            if ($busyIndicator.length) {
                $busyIndicator.spin({
                    lines: 11,
                    length: 0,
                    width: 4,
                    radius: 7,
                    corners: 1.0,
                    trail: 60,
                    speed: 1.2
                });
            } else {
                abp.ui.block(elm);
                $elm.spin({
                    lines: 11,
                    length: 0,
                    width: 10,
                    radius: 20,
                    corners: 1.0,
                    trail: 60,
                    speed: 1.2
                });
            }
        }

        if (options.promise) {
            options.promise.always(function() {
                abp.ui.clearBusy(elm);
            });
        }
    };
    
    abp.ui.clearBusy = function (elm) {
        if (!elm) {
            abp.ui.unblock();
            $('body').spin(false);
        } else {
            var $elm = $(elm);
            var $busyIndicator = $elm.find('.abp-busy-indicator');
            if ($busyIndicator.length) {
                $busyIndicator.spin(false);
            } else {
                abp.ui.unblock(elm);
                $elm.spin(false);
            }
        }
    };

})();