(function () {
    var controllerId = 'app.views.questions.createDialog';
    angular.module('app').controller(controllerId, [
        'abp.services.app.question', '$modalInstance',
        function (questionService, $modalInstance) {
            var vm = this;

            vm.question = {
                title: '',
                text: ''
            };

            vm.save = function() {
                questionService
                    .createQuestion(vm.question)
                    .success(function() {
                        $modalInstance.close();
                    });
            };

            vm.cancel = function () {
                $modalInstance.dismiss('cancel');
            };
        }
    ]);
})();