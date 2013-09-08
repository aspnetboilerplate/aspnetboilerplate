define(function () {
    var dtos = dtos || {};

    //TaskDto
    dtos.TaskDto = function () {
        this.id = ko.observable();
        this.title = ko.observable();
        this.description = ko.observable();
    };

    return dtos;
});