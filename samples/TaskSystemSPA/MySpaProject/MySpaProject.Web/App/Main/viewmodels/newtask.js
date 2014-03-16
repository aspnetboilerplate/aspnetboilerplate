define(['service!tasksystem/person', 'service!tasksystem/task', 'plugins/history'], function (personService, taskService, history) {
    return function () {
        var that = this;

        that.people = ko.mapping.fromJS([]);

        that.task = {
            description: ko.observable(''),
            assignedPersonId: ko.observable(0)
        };

        that.canActivate = function () {
            return personService.getAllPeople().done(function (data) {
                ko.mapping.fromJS(data.people, that.people);
            });
        };

        that.saveTask = function () {
            taskService.createTask(ko.mapping.toJS(that.task)).done(function () {
                abp.notify.info('Task "' + that.task.description() + '" is created successfully.');
                history.navigate('');
            });
        };
    };
});