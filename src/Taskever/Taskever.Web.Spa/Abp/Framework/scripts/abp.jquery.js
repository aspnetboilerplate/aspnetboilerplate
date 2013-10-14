var abp = abp || {};
(function ($) {

    /* JQUERY PLUGIN CONFIGURATIONS */
    if ($.blockUI) {
        $.blockUI.defaults.css = {};

        $.blockUI.defaults.overlayCSS = {
            backgroundColor: '#000',
            opacity: 0.6,
            cursor: 'wait'
        };
    }

    /* JQUERY ENHANCEMENTS ***************************************************/

    // abp.ajax -> uses $.ajax ------------------------------------------------

    //TODO: Think to implement success, error and complete callbacks
    abp.ajax = function (userOptions) {
        userOptions = userOptions || {};

        var defer = $.Deferred();
        var options = $.extend({}, abp.ajax.defaultOpts, userOptions);

        abpAjaxHelper.blockUI(options);
        $.ajax(options)
            .done(function (data) {
                abpAjaxHelper.handleData(data, userOptions, defer);
            }).fail(function () {
                defer.reject.apply(this, arguments);
            }).always(function () {
                abpAjaxHelper.unblockUI(options);
            });

        return defer.promise();
    };

    abp.ajax.defaultOpts = {
        dataType: 'json',
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

        options.complete = function () {
            abpAjaxHelper.unblockUI(options);
            userOptions.complete && userOptions.complete.apply(this, arguments);
        };

        return this.ajaxForm(options);
    };

    $.fn.abpAjaxForm.defaults = {
        method: 'POST',
    };

    /* PRIVATE METHODS *******************************************************/

    //Used on ajax request
    var abpAjaxHelper = {

        blockUI: function (options) {
            if ($.blockUI && options.blockUI) {
                if (options.blockUI === true) { //block whole page
                    $.blockUI(options.blockOptions);
                } else { //block an element
                    $(options.blockUI).block(options.blockOptions || { message: '<div style="width=50; height=50"><img src="/Abp/Framework/images/loading.gif" /></div>' });
                }
            }
        },

        unblockUI: function (options) {
            if ($.blockUI && options.blockUI) {
                if (options.blockUI === true) { //unblock whole page
                    $.unblockUI();
                } else { //unblock an element
                    $(options.blockUI).unblock();
                }
            }
        },

        handleData: function (data, userOptions, defer) {
            if (data) {

                if (data.success === false) {

                    if (data.error) {

                        alert(data.error.message); //TODO: Show a better message!

                        defer && defer.reject(data.error);
                        userOptions.error && userOptions.error(data.error);
                    }

                    if (data.unAuthorizedRequest) {
                        if (data.TargetUrl) {
                            location.href = data.targetUrl;
                        } else {
                            location.reload();
                        }
                    }

                    return;
                }
            }

            defer && defer.resolve(data);
            userOptions.success && userOptions.success(data);

            if (data.TargetUrl) {
                location.href = data.targetUrl;
            }
        }
    };

})(jQuery);