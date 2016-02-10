var abp = abp || {};
(function ($) {

    if (!$ || !$.connection) {
        return;
    }

    abp.signalr = abp.signalr || {};
    abp.signalr.hubs = abp.signalr.hubs || {};
    abp.signalr.hubs.common = $.connection.abpCommonHub;

    var commonHub = abp.signalr.hubs.common;
    if (!commonHub) {
        return;
    }

    commonHub.client.getNotification = function (notification) {
        abp.event.trigger('abp.notifications.received', notification);
    };

    $.connection.hub.start().done(function () {
        abp.log.debug('Connected to SignalR server!'); //TODO: Remove log
        commonHub.server.register().done(function () {
            abp.log.debug('Registered to the SignalR server!'); //TODO: Remove log
        });
    });

})(jQuery);