define(['plugins/router'], function (router) {
    return {
        router: router,
        
        activate: function () {
            router.map([
                { route: '', title: 'Home', moduleId: 'viewmodels/home', nav: true },
                { route: 'about', moduleId: 'viewmodels/about', nav: true }
            ]).buildNavigationModel();

            return router.activate();
        }
    };
});