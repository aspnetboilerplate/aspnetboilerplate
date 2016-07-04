(function (abp, angular) {

    if (!angular) {
        return;
    }

    abp.ng = abp.ng || {};

    abp.ng.http = {
        defaultError: {
            message: 'Ajax request did not succeed!',
            details: 'Error detail not sent by server.'
        },

        logError: function (error) {
            abp.log.error(error);
        },

        showError: function (error) {
            if (error.details) {
                return abp.message.error(error.details, error.message || abp.ng.http.defaultError.message);
            } else {
                return abp.message.error(error.message || abp.ng.http.defaultError.message);
            }
        },

        handleTargetUrl: function (targetUrl) {
            location.href = targetUrl;
        },

        handleUnAuthorizedRequest: function (messagePromise, targetUrl) {
            if (messagePromise) {
                messagePromise.done(function () {
                    if (!targetUrl) {
                        location.reload();
                    } else {
                        abp.ng.http.handleTargetUrl(targetUrl);
                    }
                });
            } else {
                if (!targetUrl) {
                    location.reload();
                } else {
                    abp.ng.http.handleTargetUrl(targetUrl);
                }
            }
        },

        handleResponse: function (response, defer) {
            var originalData = response.data;

            if (originalData.success === true) {
                response.data = originalData.result;
                defer.resolve(response);

                if (originalData.targetUrl) {
                    abp.ng.http.handleTargetUrl(originalData.targetUrl);
                }
            } else if (originalData.success === false) {
                var messagePromise = null;

                if (originalData.error) {
                    messagePromise = abp.ng.http.showError(originalData.error);
                } else {
                    originalData.error = defaultError;
                }

                abp.ng.http.logError(originalData.error);

                response.data = originalData.error;
                defer.reject(response);

                if (originalData.unAuthorizedRequest) {
                    abp.ng.http.handleUnAuthorizedRequest(messagePromise, originalData.targetUrl);
                }
            } else { //not wrapped result
                defer.resolve(response);
            }
        }
    }

    var abpModule = angular.module('abp', []);

    abpModule.config([
        '$httpProvider', function ($httpProvider) {
            $httpProvider.interceptors.push(['$q', function ($q) {

                return {

                    'request': function (config) {
                        if (endsWith(config.url, '.cshtml')) {
                            config.url = abp.appPath + 'AbpAppView/Load?viewUrl=' + config.url + '&_t=' + abp.pageLoadTime.getTime();
                        }

                        return config;
                    },

                    'response': function (response) {
                        if (!response.config || !response.config.abp || !response.data) {
                            return response;
                        }

                        var defer = $q.defer();

                        abp.ng.http.handleResponse(response, defer);

                        return defer.promise;
                    },

                    'responseError': function (ngError) {
                        var error = {
                            message: ngError.data || abp.ng.http.defaultError.message,
                            details: ngError.statusText || abp.ng.http.defaultError.details,
                            responseError: true
                        }

                        abp.ng.http.showError(error);

                        abp.ng.http.logError(error);

                        return $q.reject(ngError);
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