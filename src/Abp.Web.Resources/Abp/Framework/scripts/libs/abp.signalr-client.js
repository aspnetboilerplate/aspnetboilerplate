var abp = abp || {};
(function () {

    // Check if SignalR is defined
    if (!signalR) {
        return;
    }

    // Create namespaces
    abp.signalr = abp.signalr || {};
    abp.signalr.hubs = abp.signalr.hubs || {};

    // Configure the connection
    function configureConnection(connection) {
        // Set the common hub
        abp.signalr.hubs.common = connection;

        // Reconnect if hub disconnects
        connection.onclose(function (e) {
            if (e) {
                abp.log.debug('Connection closed with error: ' + e);
            }
            else {
                abp.log.debug('Disconnected');
            }

            if (!abp.signalr.autoConnect) {
                return;
            }

            setTimeout(function () {
                connection.start();
            }, 5000);
        });

        // Register to get notifications
        connection.on('getNotification', function (notification) {
            abp.event.trigger('abp.notifications.received', notification);
        });
    }

    // Connect to the server
    abp.signalr.connect = function () {
        var url = abp.signalr.url || '/signalr';

        // Start the connection.
        startConnection(url, configureConnection)
            .then(function (connection) {
                abp.log.debug('Connected to SignalR server!'); //TODO: Remove log
                abp.event.trigger('abp.signalr.connected');
                // Call the Register method on the hub.
                connection.invoke('register').then(function () {
                    abp.log.debug('Registered to the SignalR server!'); //TODO: Remove log
                });
            })
            .catch(function (error) {
                abp.log.debug(error.message);
            });
    };

    // Starts a connection with transport fallback - if the connection cannot be started using
    // the webSockets transport the function will fallback to the serverSentEvents transport and
    // if this does not work it will try longPolling. If the connection cannot be started using
    // any of the available transports the function will return a rejected Promise.
    function startConnection(url, configureConnection) {
        if (abp.signalr.remoteServiceBaseUrl) {
            url = abp.signalr.remoteServiceBaseUrl + url;
        }

        // Add query string: https://github.com/aspnet/SignalR/issues/680
        if (abp.signalr.qs) {
            url += '?' + abp.signalr.qs;
        }

        return function start(transport) {
            abp.log.debug(`Starting connection using ${signalR.TransportType[transport]} transport`);
            var connection = new signalR.HubConnection(url, { transport: transport });
            if (configureConnection && typeof configureConnection === 'function') {
                configureConnection(connection);
            }

            return connection.start()
                .then(function () {
                    return connection;
                })
                .catch(function (error) {
                    abp.log.debug(`Cannot start the connection using ${signalR.TransportType[transport]} transport. ${error.message}`);
                    if (transport !== signalR.TransportType.LongPolling) {
                        return start(transport + 1);
                    }

                    return Promise.reject(error);
                });
        }(signalR.TransportType.WebSockets);
    }

    abp.signalr.startConnection = startConnection;

    if (abp.signalr.autoConnect === undefined) {
        abp.signalr.autoConnect = true;
    }

    if (abp.signalr.autoConnect) {
        abp.signalr.connect();
    }

})();
