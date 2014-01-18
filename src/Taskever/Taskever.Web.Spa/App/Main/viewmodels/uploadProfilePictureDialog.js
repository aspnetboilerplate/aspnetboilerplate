define(["jquery", "knockout", 'plugins/dialog'],
    function ($, ko, dialogs) {
        return function () {
            var that = this;

            // Public fields //////////////////////////////////////////////////////

            // Public methods /////////////////////////////////////////////////////

            that.attached = function(view, parent) {
                $(view).find('form').ajaxForm({
                    beforeSend: function () {
                        
                    },
                    uploadProgress: function (event, position, total, percentComplete) {
                        
                    },
                    success: function (data) {
                        dialogs.close(that, data.result.imageUrl); //TODO: Why data.resut!!!
                    }
                });
            };

            that.cancel = function () {
                dialogs.close(that);
            };
        };
    });