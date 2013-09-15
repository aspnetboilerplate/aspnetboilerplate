define(['plugins/router', 'durandal/app'], function (router, app) {
    return {
        router: router,
        activate: function () {
            router.map([
                { route: '', title: 'First page', moduleId: 'viewmodels/home', nav: true },
                { route: 'secondpage', title: 'second page', moduleId: 'viewmodels/secondpage', nav: true }
            ]).buildNavigationModel();

            return router.activate();
        }
    };
});