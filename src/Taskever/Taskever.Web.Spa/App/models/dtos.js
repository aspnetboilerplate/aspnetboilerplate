define(function () {
    var dtos = dtos || {};

    //TaskDto
    dtos.TaskDto = function () {
        this.title = ko.observable();
        this.description = ko.observable();
    };

    return dtos;
});