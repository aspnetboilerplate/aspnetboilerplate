(function (abp, angular) {

    if (!angular) {
        return;
    }

    var abpModule = angular.module('abp', []);

    abpModule.config([
        '$httpProvider', function ($httpProvider) {
            $httpProvider.interceptors.push(function ($q) {

                var defaultError = {
                    message: 'Ajax request is not succeed!',
                    details: 'Error detail is not sent by server.'
                };

                return {
                    'response': function (response) {
                        if (!response.config || !response.config.abp || !response.data) {
                            return response;
                        }

                        var defer = $q.defer();

                        if (response.data.success === true) {
                            response.data = response.data.result;
                            defer.resolve(response);
                        } else { //data.success === false
                            if (response.data.error) {
                                abp.message.error(response.data.error.message);
                            } else {
                                response.data.error = defaultError;
                            }

                            abp.log.error(response.data.error.message + ' | ' + response.data.error.details);

                            response.data = response.data.error;
                            defer.reject(response);

                            if (response.data.unAuthorizedRequest && !response.data.targetUrl) {
                                location.reload();
                            }
                        }

                        if (response.data.targetUrl) {
                            location.href = data.targetUrl;
                        }

                        return defer.promise;
                    }
                };
            });
        }
    ]);

})((abp || (abp = {})), (angular || undefined));