define(
    ["knockout", 'session', 'service!abp/user'],
    function (ko, session, userService) {
        
    return function () {
        var that = this;

        // Private variables //////////////////////////////////////////////////

        // Public fields //////////////////////////////////////////////////////
        
        that.passwordChange = {
            currentPassword: ko.observable(''),
            newPassword: ko.observable(''),
            newPasswordRepeat: ko.observable('')
        };

        // Public methods /////////////////////////////////////////////////////

        that.updatePassword = function () {
            userService.changeSettings(
                ko.mapping.toJS(that.passwordChange)
            ).done(function () {
                alert('ok');
            });
        };
    };

});
