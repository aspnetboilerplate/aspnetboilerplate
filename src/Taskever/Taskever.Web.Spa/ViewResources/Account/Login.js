define(["jquery"], function ($) {

    $(function () {

        //Login form

        $("#LoginForm").validate({
            rules: {
                Password: {
                    minlength: 3
                }
            }
        });

        $("#LoginForm").abpAjaxForm({
            blockUI: '#LoginFormPanelBody'
        });

        //Registration form

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

        $("#RegisterForm").abpAjaxForm({
            blockUI: '#RegisterFormPanelBody'
        });

        $('#ForgotPasswordLink').click(function () {
            $('#PasswordResetLinkModal').modal('show');
        });

        $('#PasswordResetLinkModalSubmitButton').click(function () {
            abp.ajax({
                url: '/Account/SendPasswordResetLink',
                data: {
                    EmailAddress: $('#PasswordResetEmailAddress').val()
                }
            }).done(function () {
                $('#PasswordResetLinkModal').modal('hide');
            });
        });

    });

    return {

    };
});