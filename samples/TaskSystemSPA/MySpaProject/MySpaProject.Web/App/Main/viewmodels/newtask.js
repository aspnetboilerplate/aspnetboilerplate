define(['service!tasksystem/person', 'service!tasksystem/task', 'plugins/history'],
    function (personService, taskService, history) {

        var localize = abp.localization.getSource('mySpaProject');

        return function () {
            var that = this;

            var _$form = null;

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

            that.attached = function (view, parent) {
                _$form = $(view).find('form');
                _$form.validate();
            };

            that.saveTask = function () {
                if (!_$form.valid()) {
                    return;
                }

                taskService.createTask(ko.mapping.toJS(that.task)).done(function () {
                    abp.notify.info(abp.utils.formatString(localize("TaskCreatedMessage"), that.task.description()));
                    history.navigate('');
                });
            };
        };
    });