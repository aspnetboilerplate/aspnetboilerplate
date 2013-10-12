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

    app.title = 'Task Ever';

    app.configurePlugins({
        router: true,
        dialog: true,
        widget: true
    });

    app.start().then(function () {
        viewLocator.useConvention();
        viewEngine.convertViewIdToRequirePath = function (viewId) {
            console.log(this.viewPlugin + '!/DurandalView/GetAppView?view=' + viewId + '.cshtml');
            return this.viewPlugin + '!' + viewId + this.viewExtension;
        };

        app.setRoot('viewmodels/layout', 'entrance');
    });
});