define(['plugins/router'],
    function (router) {

        var languages = [
            {
                name: 'tr',
                displayName: 'Türkçe',
                iconClass: 'famfamfam-flag-tr'
            },
            {
                name: 'en',
                displayName: 'English',
                iconClass: 'famfamfam-flag-england'
            }
        ];

        return new function () {
            var that = this;

            that.router = router;

            that.getLanguageFlagClass = function (lang) {
                for (var i = 0; i < languages.length; i++) {
                    if (lang.indexOf(languages[i].name) == 0) {
                        return languages[i].iconClass;
                    }
                }

                return '';
            };

            that.getLanguageName = function (lang) {
                for (var i = 0; i < languages.length; i++) {
                    if (lang.indexOf(languages[i].name) == 0) {
                        return languages[i].displayName;
                    }
                }

                return '';
            };

            that.activate = function () {
                router.map([
                    { route: '', title: abp.localization.localize('HomePage', 'MySpaProject'), moduleId: 'viewmodels/home', nav: true },
                    { route: 'about', title: abp.localization.localize('About', 'MySpaProject'), moduleId: 'viewmodels/about', nav: true }
                ]).buildNavigationModel();

                return that.router.activate();
            };
        };
    });