define(
    ["knockout", 'session', 'service!abp/user', 'plugins/dialog'],
    function (ko, session, userService, dialogs) {

        return function () {
            var that = this;

            // Private variables //////////////////////////////////////////////////

            // Public fields //////////////////////////////////////////////////////

            that.user = session.getCurrentUser();

            that.passwordChange = {
                currentPassword: ko.observable(''),
                newPassword: ko.observable(''),
                newPasswordRepeat: ko.observable('')
            };

            // Public methods /////////////////////////////////////////////////////

            that.updatePassword = function () {
                userService.changePassword(
                    ko.mapping.toJS(that.passwordChange)
                ).done(function () {
                    abp.notify.info('Your password has been successfully changed');
                });
            };

            that.showUploadProfilePictureDialog = function() {
                dialogs.show('viewmodels/uploadProfilePictureDialog').then(function (imageUrl) {
                    if(imageUrl) {
                        that.user.profileImage(imageUrl + '?timestamp=' + new Date().getTime());
                    }
                });
            };
        };

    });
