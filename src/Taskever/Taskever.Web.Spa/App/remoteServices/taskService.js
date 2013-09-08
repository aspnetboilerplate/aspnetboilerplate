define(['jquery', 'knockout', 'abp/abp'], function ($, ko, abp) {

    var getTasks = function () {
        return abp.ajax({
            url: '/api/services/Task/GetMyTasks',
            type: 'GET'
        });
    };

    var createTask = function(task) {
        return abp.ajax({
            url: '/api/services/Task/Create',
            type: 'POST',
            data: JSON.stringify(task)
        });
    };

    var deleteTask = function (taskId) {
        return abp.ajax({
            url: '/api/services/Task/Delete',
            type: 'GET',
            data: {
                taskId: taskId
            }
        });
    };
    
    return {
        getTasks: getTasks,
        createTask: createTask,
        deleteTask: deleteTask
    };
});
