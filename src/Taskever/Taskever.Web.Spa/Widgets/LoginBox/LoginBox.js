define(["jquery"], function ($) {

    $(function () {
        $("#LoginBoxForm").validate({
            rules: {
                Password: {
                    minlength: 3
                }
            }
        });
    });

    return {

    };
});