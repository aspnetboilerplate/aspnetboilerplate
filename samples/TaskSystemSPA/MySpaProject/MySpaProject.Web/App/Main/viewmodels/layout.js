define(['plugins/router'],
    function (router) {

        return {
            router: router,

            activate: function () {
                router.map([
                    { route: '', title: abp.localization.localize('TaskList', 'mySpaProject'), moduleId: 'viewmodels/tasklist', nav: true },
                    { route: 'newtask', title: abp.localization.localize('NewTask', 'mySpaProject'), moduleId: 'viewmodels/newtask', nav: true }
                ]).buildNavigationModel();

                return router.activate();
            }
        };
    });