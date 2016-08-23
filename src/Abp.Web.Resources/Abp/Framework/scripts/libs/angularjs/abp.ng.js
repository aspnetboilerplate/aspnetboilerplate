(function (abp, angular) {

    if (!angular) {
        return;
    }

    abp.ng = abp.ng || {};

    abp.ng.http = {
        defaultError: {
            message: 'An error has occurred!',
            details: 'Error detail not sent by server.'
        },

        defaultError401: {
            message: 'You are not authenticated!',
            details: 'You should be authenticated (sign in) in order to perform this operation.'
        },

        defaultError403: {
            message: 'You are not authorized!',
            details: 'You are not allowed to perform this operation.'
        },

        defaultError404: {
            message: 'Resource not found!',
            details: 'The resource requested could not found on the server.'
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
            if (!targetUrl) {
                location.href = abp.appPath;
            } else {
                location.href = targetUrl;
            }
        },

        handleNonAbpErrorResponse: function (response, defer) {
            if (response.config.abpHandleError !== false) {
                switch (response.status) {
                    case 401:
                        abp.ng.http.handleUnAuthorizedRequest(
                            abp.ng.http.showError(abp.ng.http.defaultError401),
                            abp.appPath
                        );
                        break;
                    case 403:
                        abp.ng.http.showError(abp.ajax.defaultError403);
                        break;
                    case 404:
                        abp.ng.http.showError(abp.ajax.defaultError404);
                        break;
                    default:
                        abp.ng.http.showError(abp.ng.http.defaultError);
                        break;
                }
            }

            defer.reject(response);
        },

        handleUnAuthorizedRequest: function (messagePromise, targetUrl) {
            if (messagePromise) {
                messagePromise.done(function () {
                    abp.ng.http.handleTargetUrl(targetUrl || abp.appPath);
                });
            } else {
                abp.ng.http.handleTargetUrl(targetUrl || abp.appPath);
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
                    if (response.config.abpHandleError !== false) {
                        messagePromise = abp.ng.http.showError(originalData.error);
                    }
                } else {
                    originalData.error = defaultError;
                }

                abp.ng.http.logError(originalData.error);

                response.data = originalData.error;
                defer.reject(response);

                if (response.status == 401 && response.config.abpHandleError !== false) {
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
                        if (!response.data || !response.data.__abp) {
                            //Non ABP related return value
                            return response;
                        }

                        var defer = $q.defer();
                        abp.ng.http.handleResponse(response, defer);
                        return defer.promise;
                    },

                    'responseError': function (ngError) {
                        var defer = $q.defer();

                        if (!ngError.data || !ngError.data.__abp) {
                            abp.ng.http.handleNonAbpErrorResponse(ngError, defer);
                        } else {
                            abp.ng.http.handleResponse(ngError, defer);
                        }

                        return defer.promise;
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

    abp.event.on('abp.dynamicScriptsInitialized', function () {
        abp.ng.http.defaultError.message = abp.localization.abpWeb('DefaultError');
        abp.ng.http.defaultError.details = abp.localization.abpWeb('DefaultErrorDetail');
        abp.ng.http.defaultError401.message = abp.localization.abpWeb('DefaultError401');
        abp.ng.http.defaultError401.details = abp.localization.abpWeb('DefaultErrorDetail401');
        abp.ng.http.defaultError403.message = abp.localization.abpWeb('DefaultError403');
        abp.ng.http.defaultError403.details = abp.localization.abpWeb('DefaultErrorDetail403');
        abp.ng.http.defaultError404.message = abp.localization.abpWeb('DefaultError404');
        abp.ng.http.defaultError404.details = abp.localization.abpWeb('DefaultErrorDetail404');
    });

})((abp || (abp = {})), (angular || undefined));