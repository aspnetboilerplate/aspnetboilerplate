define(function () {
    return {
        load: function (name, req, onload, config) {
            //TODO: Test more
            var url = '/api/ServiceProxies/?name=' + name;
            req([url], function (value) {
                onload(value);
            });
        }
    };
});