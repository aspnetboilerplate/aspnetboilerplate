//TODO: Move to framework?
requirejs.config({
    paths: {
        'text': '../../Scripts/text',
        'durandal': '../../Scripts/durandal',
        'plugins': '../../Scripts/durandal/plugins',
        'transitions': '../../Scripts/durandal/transitions',
        'abp': '../../Abp',
        'services': '/api/serviceproxies'
    }
});

define('jquery', function () { return jQuery; });
define('knockout', ko);

define(['durandal/system', 'durandal/app', 'durandal/viewLocator', 'durandal/viewEngine'], function (system, app, viewLocator, viewEngine) {
    system.debug(true); //TODO: remove in production code
    
    //TODO: Move to framework?
    viewEngine.convertViewIdToRequirePath = function (viewId) {
        return this.viewPlugin + '!/DurandalView/GetAppView?viewUrl=' + viewId + '.cshtml';
    }; 

    app.title = 'Task Ever';

    app.configurePlugins({
        router: true,
        dialog: true,
        widget: true
    });

    app.start().then(function () {
        viewLocator.useConvention();
        app.setRoot('viewmodels/layout', 'entrance');
    });
});