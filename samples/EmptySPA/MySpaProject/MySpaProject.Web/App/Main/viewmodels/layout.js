define(['plugins/router'], function (router) {
    return {
        router: router,
        
        activate: function () {
            router.map([
                { route: '', title: 'Page1', moduleId: 'viewmodels/page1', nav: true },
                { route: 'page2', moduleId: 'viewmodels/page2', nav: true }
            ]).buildNavigationModel();

            return router.activate();
        }
    };
});