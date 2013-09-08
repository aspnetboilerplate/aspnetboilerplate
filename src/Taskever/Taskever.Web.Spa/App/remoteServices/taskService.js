define(['jquery', 'knockout', 'abp/abp'], function ($, ko, abp) {

    var getTasks = function (tasks) {
        return abp.ajax({
            url: '/api/Tasks',
            type: 'GET'
        }).then(function(data) {
            ko.mapping.fromJS(data, tasks);
        });
    };

    var createTask = function(task) {
        return abp.ajax({
            url: '/api/Tasks',
            type: 'POST',
            data: JSON.stringify(task)
        });
    };
    
    return {
        getTasks: getTasks,
        createTask: createTask
    };
});
