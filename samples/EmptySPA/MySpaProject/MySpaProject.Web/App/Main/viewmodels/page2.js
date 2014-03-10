define(['service!personaltts/region'], function (regionService) {

    return function () {
        var that = this;

        that.regions = ko.mapping.fromJS([]);
        
        that.activate = function () {
            that.loadRegions();
        };

        that.loadRegions = function () {
            regionService.getRegions().done(function(data) {
                ko.mapping.fromJS(data.regions, that.regions);
            });
        };
    };
});