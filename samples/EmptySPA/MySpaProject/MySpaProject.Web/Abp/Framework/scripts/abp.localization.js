var abp = abp || {};
(function () {
    abp.localization = abp.localization || {};

    abp.localization.localize = function(key, sourceName) {
        return abp.localization.values[sourceName][key];
    };

    abp.localization.getSource = function(sourceName) {
        return function(key) {
            return abp.localization.localize(key, sourceName);
        };
    };

})();