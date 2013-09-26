define(["jquery"], function ($) {

    $(function () {
        $("#LoginForm").validate({
            rules: {
                Password: {
                    minlength: 3
                }
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