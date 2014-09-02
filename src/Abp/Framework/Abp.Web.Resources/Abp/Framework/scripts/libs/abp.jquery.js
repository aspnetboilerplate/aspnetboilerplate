var abp = abp || {};
(function ($) {

    /* JQUERY ENHANCEMENTS ***************************************************/

    // abp.ajax -> uses $.ajax ------------------------------------------------

    //TODO: Think to implement success, error and complete callbacks
    abp.ajax = function (userOptions) {
        userOptions = userOptions || {};
        var options = $.extend({}, abp.ajax.defaultOpts, userOptions);
        return $.Deferred(function ($dfd) {
            $.ajax(options)
                .done(function (data) {
                    abpAjaxHelper.handleData(data, userOptions, $dfd);
                }).fail(function () {
                    $dfd.reject.apply(this, arguments);
                });
        });
    };

    abp.ajax.defaultOpts = {
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json'
    };

    /* JQUERY PLUGIN ENHANCEMENTS ********************************************/

    /* jQuery Form Plugin 
     * http://www.malsup.com/jquery/form/
     */

    // abpAjaxForm -> uses ajaxForm ------------------------------------------

    $.fn.abpAjaxForm = function (userOptions) {
        userOptions = userOptions || {};

        var options = $.extend({}, $.fn.abpAjaxForm.defaults, userOptions);

        options.beforeSubmit = function () {
            abpAjaxHelper.blockUI(options);
            userOptions.beforeSubmit && userOptions.beforeSubmit.apply(this, arguments);
        };

        options.success = function (data) {
            abpAjaxHelper.handleData(data, userOptions);
        };

        //TODO: Error?

        options.complete = function () {
            abpAjaxHelper.unblockUI(options);
            userOptions.complete && userOptions.complete.apply(this, arguments);
        };

        return this.ajaxForm(options);
    };

    $.fn.abpAjaxForm.defaults = {
        method: 'POST'
    };

    /* PRIVATE METHODS *******************************************************/

    //TODO: Extract block/spin options

    //Used on ajax request
    var abpAjaxHelper = {

        blockUI: function (options) {
            if (options.blockUI) {
                if (options.blockUI === true) { //block whole page
                    abp.ui.setBusy();
                } else { //block an element
                    abp.ui.setBusy(options.blockUI);
                }
            }
        },

        unblockUI: function (options) {
            if (options.blockUI) {
                if (options.blockUI === true) { //unblock whole page
                    abp.ui.clearBusy();
                } else { //unblock an element
                    abp.ui.clearBusy(options.blockUI);
                }
            }
        },

        handleData: function (data, userOptions, $dfd) {
            if (data) {
                if (data.success === true) {
                    $dfd && $dfd.resolve(data.result, data);
                    userOptions.success && userOptions.success(data.result, data);
                } else { //data.success === false
                    if (data.error) {
                        abp.message.error(data.error.message);
                        $dfd && $dfd.reject(data.error);
                        userOptions.error && userOptions.error(data.error);
                    }

                    if (data.unAuthorizedRequest && !data.targetUrl) {
                        location.reload();
                    }
                }

                if (data.targetUrl) {
                    location.href = data.targetUrl;
                }
            } else { //no data sent to back
                $dfd && $dfd.resolve();
                userOptions.success && userOptions.success();
            }
        }
    };

})(jQuery);