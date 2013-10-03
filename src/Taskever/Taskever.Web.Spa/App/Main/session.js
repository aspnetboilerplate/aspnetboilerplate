define(["knockout"], function (ko) {

    var MyUserModel = function() {
        this.id = ko.observable(1);
        this.name = ko.observable('halilibrahimkalkan');
    };

    var currentUser = new MyUserModel();

    return {
        getCurrentUser: function () {
            return currentUser;
        }
    };
});