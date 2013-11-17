define(["jquery", "knockout", 'plugins/dialog', 'service!dto', 'service!taskever/task', 'service!taskever/friendship', 'session'],
    function ($, ko, dialogs, dtos, taskService, friendshipService, session) {
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