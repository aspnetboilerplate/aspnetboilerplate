(function () {
    var controllerId = 'app.views.users.index';
    angular.module('app').controller(controllerId, [
        'abp.services.app.user', 
        function (userService) {
            var vm = this;

            vm.users = [];

            abp.ui.setBusy(
                null,
                userService.getUsers().success(function(data) {
                    vm.users = data.items;
                })
            );
        }
    ]);
})();