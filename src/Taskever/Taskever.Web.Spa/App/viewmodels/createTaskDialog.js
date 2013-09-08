define(["jquery", "knockout", 'durandal/app', 'plugins/dialog', 'models/dtos', 'remoteServices/taskService'], function ($, ko, app, dialogs, dtos, taskService) {
    var vm = function() {
        this.task = new dtos.TaskDto();
    };

    vm.prototype.saveNewTask = function () {
        var that = this;
        taskService.createTask(ko.toJS(that.task)).then(function () {
            dialogs.close(that, that.task);
        });
    };

    return vm;
});