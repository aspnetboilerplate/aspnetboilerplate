define(['plugins/router'], function (router) {
    return {
        router: router,
        
        activate: function () {
            router.map([
                { route: '', title: 'Task list', moduleId: 'viewmodels/tasklist', nav: true },
                { route: 'newtask', title: 'New task', moduleId: 'viewmodels/newtask', nav: true }
            ]).buildNavigationModel();

            return router.activate();
        }
    };
});