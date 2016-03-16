var abp = abp || {};
(function ($) {

    if (!$ || !$.connection) {
        return;
    }

    //TODO: Check if ABP's SignalR hub is enabled..?

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

    //TODO: Check connection state and only connect if not connected?
    $.connection.hub.start().done(function () {
        abp.log.debug('Connected to SignalR server!'); //TODO: Remove log
    });

})(jQuery);