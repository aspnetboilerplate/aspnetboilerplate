//TODO: Move to framework?
requirejs.config({
    paths: {
        'text': '../../Scripts/text',
        'durandal': '../../Scripts/durandal',
        'plugins': '../../Scripts/durandal/plugins',
        'transitions': '../../Scripts/durandal/transitions',
        'abp': '../../Abp',
        'services': '/api/serviceproxies',
        'service': '/Abp/Framework/scripts/requirejs/plugins/service'
    }
});

define('jquery', function () { return jQuery; });
define('knockout', ko);

define(['durandal/system', 'durandal/app', 'durandal/viewLocator', 'durandal/viewEngine', 'session'], function (system, app, viewLocator, viewEngine, session) {
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

    session.start().then(function () {
        app.start().then(function () {
            viewLocator.useConvention();
            app.setRoot('viewmodels/layout', 'entrance');
        });
    });
});