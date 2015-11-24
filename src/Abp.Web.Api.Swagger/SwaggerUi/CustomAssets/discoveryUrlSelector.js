(function () {
    var DiscoveryUrlSelector = Backbone.View.extend({
        render: function() {
            // Don't re-render on subsequent reloads
            var defaultVal = this.$el.val()
            if (this.$el.prop('tagName') != 'SELECT') {
                var select = $('<select id="input_baseUrl" name="baseUrl"></select>');
                select
                    .css('margin', '0')
                    .css('border', '1px solid gray')
                    .css('padding', '3px')
                    .css('width', '400px')
                    .css('font-size', '0.9em');

                var rootUrl = this.options.rootUrl;
                _.each(this.options.discoveryPaths, function(path) {
                    var option = $('<option>' + rootUrl + "/" + path + '</option>');
                    select.append(option);
                });

                select.val(defaultVal);
                this.$el.replaceWith(select);
            }
            return this;
        }
    });

    new DiscoveryUrlSelector({
        el: $('#input_baseUrl'),
        rootUrl: swashbuckleConfig.rootUrl,
        discoveryPaths: swashbuckleConfig.discoveryPaths
    }).render();
})();