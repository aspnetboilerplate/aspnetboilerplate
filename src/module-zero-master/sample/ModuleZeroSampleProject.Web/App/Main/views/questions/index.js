(function () {
    var controllerId = 'app.views.questions.index';
    angular.module('app').controller(controllerId, [
        'abp.services.app.question', '$modal',
        function (questionService, $modal) {
            var vm = this;

            vm.permissions = {
                canCreateQuestions: abp.auth.hasPermission("CanCreateQuestions")
            };

            vm.sortingDirections = ['CreationTime DESC', 'VoteCount DESC', 'ViewCount DESC', 'AnswerCount DESC'];

            vm.questions = [];
            vm.totalQuestionCount = 0;
            vm.sorting = 'CreationTime DESC';

            vm.loadQuestions = function (append) {
                var skipCount = append ? vm.questions.length : 0;
                abp.ui.setBusy(
                    null,
                    questionService.getQuestions({
                        skipCount: skipCount,
                        sorting: vm.sorting
                    }).success(function (data) {
                        if (append) {
                            for (var i = 0; i < data.items.length; i++) {
                                vm.questions.push(data.items[i]);
                            }
                        } else {
                            vm.questions = data.items;
                        }

                        vm.totalQuestionCount = data.totalCount;
                    })
                );
            };

            vm.showNewQuestionDialog = function () {
                var modalInstance = $modal.open({
                    templateUrl: abp.appPath + 'App/Main/views/questions/createDialog.cshtml',
                    controller: 'app.views.questions.createDialog as vm',
                    size: 'md'
                });

                modalInstance.result.then(function () {
                    vm.loadQuestions();
                });
            };

            vm.sort = function (sortingDirection) {
                vm.sorting = sortingDirection;
                vm.loadQuestions();
            };

            vm.showMore = function () {
                vm.loadQuestions(true);
            };

            vm.loadQuestions();
        }
    ]);
})();