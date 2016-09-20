/* Adapter for jTable (http://jtable.org) to ASP.NET Boilerplate (http://aspnetboilerplate.com)
 * by Halil ibrahim Kalkan (http://halilibrahimkalkan.com).
 */
(function ($) {

    if (!$ || !$.hik || !$.hik.jtable) {
        return;
    }

    //Reference to base object members
    var base = {
        _create: $.hik.jtable.prototype._create
    };

    //Extending jTable to adapt ASP.NET Boilerplate
    $.extend(true, $.hik.jtable.prototype, {

        //Override _create function to change actions according to Abp system
        _create: function () {
            var self = this;
            base._create.apply(self, arguments);

            if (self.options.actions.listAction) {
                self._adaptListActionforAbp();
            }

            if (self.options.actions.createAction) {
                self._adaptCreateActionforAbp();
            }

            if (self.options.actions.updateAction) {
                self._adaptUpdateActionforAbp();
            }

            if (self.options.actions.deleteAction) {
                self._adaptDeleteActionforAbp();
            }
        },

        //LIST ACTION ADAPTER
        _adaptListActionforAbp: function () {
            var self = this;
            var originalListAction = self.options.actions.listAction;
            self.options.actions.listAction = function (postData, jtParams) {
                return $.Deferred(function ($dfd) {

                    var input = $.extend({}, postData, {
                        skipCount: jtParams.jtStartIndex,
                        maxResultCount: jtParams.jtPageSize,
                        sorting: jtParams.jtSorting
                    });

                    originalListAction.method(input)
                        .done(function (result) {
                            $dfd.resolve({
                                "Result": "OK",
                                "Records": result.items || result[originalListAction.recordsField],
                                "TotalRecordCount": result.totalCount,
                                originalResult: result
                            });
                        })
                        .fail(function (error) {
                            self._handlerForFailOnAbpRequest($dfd, error);
                        });
                });
            };
        },

        //CREATE ACTION ADAPTER
        _adaptCreateActionforAbp: function () {
            var self = this;
            var originalCreateAction = self.options.actions.createAction;
            self.options.actions.createAction = function (postData) {
                return $.Deferred(function ($dfd) {

                    var input = $.extend({}, postData);

                    originalCreateAction.method(input)
                        .done(function (result) {
                            $dfd.resolve({
                                "Result": "OK",
                                "Record": originalCreateAction.recordField ? result[originalCreateAction.recordField] : result,
                                originalResult: result
                            });
                        })
                        .fail(function (error) {
                            self._handlerForFailOnAbpRequest($dfd, error);
                        });
                });
            };
        },

        //UPDATE ACTION ADAPTER
        _adaptUpdateActionforAbp: function () {
            var self = this;
            var originalUpdateAction = self.options.actions.updateAction;
            self.options.actions.updateAction = function (postData) {
                return $.Deferred(function ($dfd) {

                    var input = $.extend({}, postData);

                    originalUpdateAction.method(input)
                        .done(function (result) {
                            var jtableResult = {
                                "Result": "OK",
                                originalResult: result
                            };

                            if (originalUpdateAction.returnsRecord) {
                                if (originalUpdateAction.recordField) {
                                    jtableResult.Record = result[originalUpdateAction.recordField];
                                } else {
                                    jtableResult.Record = result;
                                }
                            }

                            $dfd.resolve(jtableResult);
                        })
                        .fail(function (error) {
                            self._handlerForFailOnAbpRequest($dfd, error);
                        });
                });
            };
        },

        //DELETE ACTION ADAPTER
        _adaptDeleteActionforAbp: function () {
            var self = this;
            var originalDeleteAction = self.options.actions.deleteAction;
            self.options.actions.deleteAction = function (postData) {
                return $.Deferred(function ($dfd) {

                    var input = $.extend({}, postData);

                    originalDeleteAction.method(input)
                        .done(function (result) {
                            $dfd.resolve({
                                "Result": "OK",
                                originalResult: result
                            });
                        })
                        .fail(function (error) {
                            self._handlerForFailOnAbpRequest($dfd, error);
                        });
                });
            };
        },

        _handlerForFailOnAbpRequest: function ($dfd, error) {
            if (error && error.message) {
                $dfd.resolve({
                    Result: "ERROR",
                    Message: error.message
                });
            } else {
                $dfd.reject(error);
            }
        },

        //Disable showing error messages
        _showError: function (message) {
            //do nothing since Abp handles error messages!
        }

    });

    //Overriding some defaults
    $.extend(true, $.hik.jtable.prototype.options, {
        pageList: "minimal"
    });

})(jQuery);