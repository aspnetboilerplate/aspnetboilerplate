define(function () {
    return {
        load: function (name, req, onload, config) {
            var url = '/api/AbpServiceProxies/Get?name=' + name;
            req([url], function (value) {
                onload(value);
            });
        }
    };
});