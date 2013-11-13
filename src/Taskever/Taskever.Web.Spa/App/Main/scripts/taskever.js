var taskever = taskever || { };
(function () {

    taskever.taskPriorities = {
        High: 5,
        AboveNormal: 4,
        Normal: 3,
        BelowNormal: 2,
        Low: 1
    };
    
    taskever.taskPrioritiesInverted = _.invert(taskever.taskPriorities);

}());