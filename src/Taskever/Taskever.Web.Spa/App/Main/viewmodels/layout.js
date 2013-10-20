define(['plugins/router', 'session'], function (router, session) {
    return {
        currentUser: session.getCurrentUser(),
        router: router,
        activate: function () {
            router.map([
                { route: '', title: 'Home', moduleId: 'viewmodels/home', nav: true },
                { route: 'user/:id', title: 'User Profile', moduleId: 'viewmodels/user', nav: true },
                { route: 'friends', title: 'My Friends', moduleId: 'viewmodels/friends', nav: true }
            ]).buildNavigationModel();

            return router.activate();
        }
    };
});