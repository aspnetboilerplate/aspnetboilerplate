define(["jquery", "knockout", 'durandal/app', 'plugins/dialog', 'models/dtos', 'services/task'], function ($, ko, app, dialogs, dtos, taskService) {
    var vm = function() {
        this.task = new dtos.TaskDto();
    };

    vm.prototype.saveNewTask = function () {
        var that = this;
        taskService.createTask(ko.mapping.toJS(that.task))
            .then(function(data) {
                dialogs.close(that, ko.mapping.fromJS(data));
            });
    };

    return vm;
});