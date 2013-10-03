define(["knockout"], function (ko) {
    var dtos = dtos || {};

    //TODO: Auto create DTO objects for knockout!

    dtos.TaskDto = function () {
        this.id = ko.observable();
        
        this.title = ko.observable();
        this.description = ko.observable();
        this.assignedUserId = ko.observable();
        this.priority = ko.observable();
        
        this.creationTime = ko.observable();
        this.creatorUserId = ko.observable();
        this.lastModificationTime = ko.observable();
        this.lastModifierUserId = ko.observable();
    };

    return dtos;
});