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

        //Add hash to return url
        $('#LoginReturnUrl').val($('#LoginReturnUrl').val() + location.hash);

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
                data: JSON.stringify({ emailAddress: $('#PasswordResetEmailAddress').val() })
            }).done(function () {
                $('#PasswordResetLinkModal').modal('hide');
            });
        });

        $('.taskever-screen-preview-image').fancybox();

    });

    return {

    };
});