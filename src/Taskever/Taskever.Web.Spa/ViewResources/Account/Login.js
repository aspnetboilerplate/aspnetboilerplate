define(["jquery"], function ($) {

    $(function () {
        $("#LoginForm").validate({
            rules: {
                Password: {
                    minlength: 3
                }
            }
            //TODO: errorPlacement: function (error, element) {
                //$(element).tooltipster('update', $(error).text());
                //$(element).tooltipster('show');
           //}
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