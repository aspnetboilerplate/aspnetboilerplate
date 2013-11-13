var taskever = taskever || { };
(function () {

    var localize = abp.localization.getSource('taskever');
    
    //TASK PRIORITY
    taskever.taskPriority = {
        High: 5,
        AboveNormal: 4,
        Normal: 3,
        BelowNormal: 2,
        Low: 1
    };
    taskever.taskPriorityInverted = _.invert(taskever.taskPriority);
    taskever.taskPriority.getLocalizedText = function (priorityValue) {
        return localize('TaskPriority_' + taskever.taskPriorityInverted[priorityValue]);
    };
    
    //TASK PRIVACY
    taskever.taskPrivacy = {
        Private: 1,
        Protected: 2
    };
    taskever.taskPrivacyInverted = _.invert(taskever.taskPrivacy);
    taskever.taskPrivacy.getLocalizedText = function (privacyValue) {
        return localize('TaskPrivacy_' + taskever.taskPrivacyInverted[privacyValue]);
    };

    //TASK STATUS
    taskever.taskState = {
        New: 1,
        WorkingOn: 2,
        Completed: 3
    };
    taskever.taskStateInverted = _.invert(taskever.taskState);
    taskever.taskState.getLocalizedText = function (stateValue) {
        return localize('TaskState_' + taskever.taskStateInverted[stateValue]);
    };

}());