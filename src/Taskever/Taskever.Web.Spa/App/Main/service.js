define(function () {
    return {
        load: function (name, req, onload, config) {

            var areaName = '';
            if (name.indexOf('/') >= 0) {
                var spliitedArray = name.split('/');
                areaName = spliitedArray[0];
                name = spliitedArray[1];
            }
            
            //TODO: Test more
            //TODO: Make a better way of getting dto's
            var url;
            if (name == 'dto') {
                url = '/api/dtos';
            } else {
                url = '/api/ServiceProxies/?name=' + name + '&areaName=' + areaName;
            }

            req([url], function (value) {
                onload(value);
            });
        }
    };
});