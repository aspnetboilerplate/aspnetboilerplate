define(["jquery"], function ($) {

    $.blockUI.defaults.css = {};

    $.blockUI.defaults.overlayCSS = {
        backgroundColor: '#000',
        opacity: 0.6,
        cursor: 'wait'
    };


    $(function () {

        $("#LoginForm").validate({
            rules: {
                Password: {
                    minlength: 3
                }
            }
        });
        
        $("#LoginForm").ajaxForm({
            beforeSubmit: function () {
                $("#LoginFormPanelBody").block({
                    message: '<div style=""><img src="/Images/loading.gif" /></div>'
                });
            },
            url: '/Account/LoginAjax',
            method: 'POST',
            success: function (data) {
                if (data.Success == false) {
                    alert(data.Error.Message);
                    return;
                }

                location.href = '/';
            },
            complete: function () {
                $("#LoginFormPanelBody").unblock();
            }
        });

        $("#RegisterForm").validate({
            rules: {
                Password: {
                    minlength: 3
                },
                PasswordRepeat: {
                    equalTo: "#RegisterPassword"
                }
            }
        });

    });

    return {

    };
});