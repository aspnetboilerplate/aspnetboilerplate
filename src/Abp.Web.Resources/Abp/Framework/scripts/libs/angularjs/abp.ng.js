(function (abp, angular) {

    if (!angular) {
        return;
    }

    var abpModule = angular.module('abp', []);

    abpModule.config([
        '$httpProvider', function ($httpProvider) {
            $httpProvider.interceptors.push(['$q', function ($q) {

                var defaultError = {
                    message: 'Ajax request is not succeed!',
                    details: 'Error detail is not sent by server.'
                };

                return {

                    'request': function (config) {
                        if (endsWith(config.url, '.cshtml')) {
                            config.url = abp.appPath + 'AbpAppView/Load?viewUrl=' + config.url;
                        }

                        return config;
                    },

                    'response': function (response) {
                        if (!response.config || !response.config.abp || !response.data) {
                            return response;
                        }

                        var originalData = response.data;
                        var defer = $q.defer();

                        if (originalData.success === true) {
                            response.data = originalData.result;
                            defer.resolve(response);
                        } else { //data.success === false
                            var messagePromise = null;
                            if (originalData.error) {
                                if (originalData.error.details) {
                                    messagePromise = abp.message.error(originalData.error.details, originalData.error.message);
                                } else {
                                    messagePromise = abp.message.error(originalData.error.message);
                                }
                            } else {
                                originalData.error = defaultError;
                            }

                            abp.log.error(originalData.error.message + ' | ' + originalData.error.details);

                            response.data = originalData.error;
                            defer.reject(response);

                            if (originalData.unAuthorizedRequest && !originalData.targetUrl) {
                                if (messagePromise) {
                                    messagePromise.done(function () {
                                        location.reload();
                                    });
                                } else {
                                    location.reload();
                                }
                            }
                        }

                        if (originalData.targetUrl) {
                            if (messagePromise) {
                                messagePromise.done(function () {
                                    location.href = originalData.targetUrl;
                                });
                            } else {
                                location.href = originalData.targetUrl;
                            }
                        }

                        return defer.promise;
                    },

                    'responseError': function (error) {
                        abp.message.error(error.data, error.statusText);
                        abp.log.error(error);
                        return $q.reject(error);
                    }

                };
            }]);
        }
    ]);

    function endsWith(str, suffix) {
        if (suffix.length > str.length) {
            return false;
        }

        return str.indexOf(suffix, str.length - suffix.length) !== -1;
    }

})((abp || (abp = {})), (angular || undefined));