define(["jquery"], function ($) {

    $(function () {

        $("#ResetPasswordForm").validate({
            rules: {
                Password: {
                    minlength: 3
                },
                PasswordRepeat: {
                    equalTo: "#ResetPassword"
                }
            }
        });

        $("#ResetPasswordForm").abpAjaxForm({
            blockUI: '#ResetPasswordFormPanelBody'
        });

    });

    return {

    };
});