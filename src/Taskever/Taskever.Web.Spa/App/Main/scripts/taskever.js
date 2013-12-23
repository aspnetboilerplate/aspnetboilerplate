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
    
    //FRIENDSHIP STATUS
    taskever.friendshipStatus = {
        WaitingApprovalFromFriend: 0,
        WaitingApprovalFromUser: 1,
        Accepted: 2
    };

    //Used to keep session open TODO: Make this configurable and more-general
    taskever.keepSessionOpenTickDuration = 12000;
    var keepSessionOpen = function () {
        if (taskever.keepSessionOpenTickDuration <= 0) {
            return;
        }

        $.ajax({
            url: '/Account/KeepSessionOpen',
            type: 'POST',
            dataType: 'json',
            success: function () {
                abp.log.debug("Keeping session open... OK"); //TODO: Remo log?
            },
            complete: function () {
                if (taskever.keepSessionOpenTickDuration > 0) {
                    setTimeout(keepSessionOpen, taskever.keepSessionOpenTickDuration);
                }
            }
        });
    };
    setTimeout(keepSessionOpen, taskever.keepSessionOpenTickDuration);

}());