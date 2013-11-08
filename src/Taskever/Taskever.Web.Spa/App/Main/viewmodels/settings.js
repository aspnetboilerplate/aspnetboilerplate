define(
    ["knockout", 'session', 'service!abp/user', 'dropzone'],
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

            that.acceptProfilePicture = function () {
                console.log('accepting...');
                abp.ajax({
                    url: '/ProfileImage/AcceptTempProfileImage'
                });
            };

            that.attached = function (view) {
                $(view).find("form.dropzone").dropzone({
                    url: '/ProfileImage/UploadTempProfileImage',
                    maxFilesize: 1,
                    addRemoveLinks: true,
                    maxFiles: 1,
                    removedfile: function (file) { //TODO: Find a better way!
                        abp.ajax({
                            url: '/ProfileImage/RemoveTempProfileImage'
                        });
                        
                        var _ref;
                        return (_ref = file.previewElement) != null ? _ref.parentNode.removeChild(file.previewElement) : void 0;
                    }
                });
            };
        };

    });
