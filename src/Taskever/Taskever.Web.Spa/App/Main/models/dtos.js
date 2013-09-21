define(["knockout"], function (ko) {
    var dtos = dtos || {};

    dtos.TaskDto = function () {
        this.id = ko.observable();
        
        this.title = ko.observable();
        this.description = ko.observable();
        
        this.creationTime = ko.observable();
        this.creatorUserId = ko.observable();
        this.lastModificationTime = ko.observable();
        this.lastModifierUserId = ko.observable();
    };

    return dtos;
});