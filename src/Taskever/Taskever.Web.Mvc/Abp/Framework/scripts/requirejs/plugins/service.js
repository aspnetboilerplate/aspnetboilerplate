define(function () {
    return {
        load: function (name, req, onload, config) {
            var url = '/api/ServiceProxies/?name=' + name;
            req([url], function (value) {
                onload(value);
            });
        }
    };
});