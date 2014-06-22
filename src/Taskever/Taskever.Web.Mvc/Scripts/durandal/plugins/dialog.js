/**
 * Durandal 2.1.0 Copyright (c) 2012 Blue Spire Consulting, Inc. All Rights Reserved.
 * Available via the MIT license.
 * see: http://durandaljs.com or https://github.com/BlueSpire/Durandal for details.
 */
/**
 * The dialog module enables the display of message boxes, custom modal dialogs and other overlays or slide-out UI abstractions. Dialogs are constructed by the composition system which interacts with a user defined dialog context. The dialog module enforced the activator lifecycle.
 * @module dialog
 * @requires system
 * @requires app
 * @requires composition
 * @requires activator
 * @requires viewEngine
 * @requires jquery
 * @requires knockout
 */
define(['durandal/system', 'durandal/app', 'durandal/composition', 'durandal/activator', 'durandal/viewEngine', 'jquery', 'knockout'], function (system, app, composition, activator, viewEngine, $, ko) {
    var contexts = {},
        dialogCount = ko.observable(0),
        dialog;

    /**
     * Models a message box's message, title and options.
     * @class MessageBox
     */
    var MessageBox = function (message, title, options, autoclose, settings) {
        this.message = message;
        this.title = title || MessageBox.defaultTitle;
        this.options = options || MessageBox.defaultOptions;
        this.autoclose = autoclose || false;
        this.settings = $.extend({}, MessageBox.defaultSettings, settings);
    };

    /**
     * Selects an option and closes the message box, returning the selected option through the dialog system's promise.
     * @method selectOption
     * @param {string} dialogResult The result to select.
     */
    MessageBox.prototype.selectOption = function (dialogResult) {
        dialog.close(this, dialogResult);
    };

    /**
     * Provides the view to the composition system.
     * @method getView
     * @return {DOMElement} The view of the message box.
     */
    MessageBox.prototype.getView = function () {
        return viewEngine.processMarkup(MessageBox.defaultViewMarkup);
    };

    /**
     * Configures a custom view to use when displaying message boxes.
     * @method setViewUrl
     * @param {string} viewUrl The view url relative to the base url which the view locator will use to find the message box's view.
     * @static
     */
    MessageBox.setViewUrl = function (viewUrl) {
        delete MessageBox.prototype.getView;
        MessageBox.prototype.viewUrl = viewUrl;
    };

    /**
     * The title to be used for the message box if one is not provided.
     * @property {string} defaultTitle
     * @default Application
     * @static
     */
    MessageBox.defaultTitle = app.title || 'Application';

    /**
     * The options to display in the message box if none are specified.
     * @property {string[]} defaultOptions
     * @default ['Ok']
     * @static
     */
    MessageBox.defaultOptions = ['Ok'];

    
    MessageBox.defaultSettings = { buttonClass: "btn btn-default", primaryButtonClass: "btn-primary autofocus", secondaryButtonClass: "", "class": "modal-content messageBox", style: null };

    /**
    * Sets the classes and styles used throughout the message box markup.
    * @method setDefaults
    * @param {object} settings A settings object containing the following optional properties: buttonClass, primaryButtonClass, secondaryButtonClass, class, style.
    */
    MessageBox.setDefaults = function (settings) {
        $.extend(MessageBox.defaultSettings, settings);
    };

    MessageBox.prototype.getButtonClass = function ($index) {
        var c = "";
        if (this.settings) {
            if (this.settings.buttonClass) {
                c = this.settings.buttonClass;
            }
            if ($index() === 0 && this.settings.primaryButtonClass) {
                if (c.length > 0) {
                    c += " ";
                }
                c += this.settings.primaryButtonClass;
            }
            if ($index() > 0 && this.settings.secondaryButtonClass) {
                if (c.length > 0) {
                    c += " ";
                }
                c += this.settings.secondaryButtonClass;
            }
        }
        return c;
    };

    MessageBox.prototype.getClass = function () {
        if (this.settings) {
            return this.settings["class"];
        }
        return "messageBox";
    };

    MessageBox.prototype.getStyle = function () {
        if (this.settings) {
            return this.settings.style;
        }
        return null;
    };

    MessageBox.prototype.getButtonText = function (stringOrObject) {
        var t = $.type(stringOrObject);
        if (t === "string") {
            return stringOrObject;
        }
        else if (t === "object") {
            if ($.type(stringOrObject.text) === "string") {
                return stringOrObject.text;
            } else {
                system.error('The object for a MessageBox button does not have a text property that is a string.');
                return null;
            }
        }
        system.error('Object for a MessageBox button is not a string or object but ' + t + '.');
        return null;
    };

    MessageBox.prototype.getButtonValue = function (stringOrObject) {
        var t = $.type(stringOrObject);
        if (t === "string") {
            return stringOrObject;
        }
        else if (t === "object") {
            if ($.type(stringOrObject.text) === "undefined") {
                system.error('The object for a MessageBox button does not have a value property defined.');
                return null;
            } else {
                return stringOrObject.value;
            }
        }
        system.error('Object for a MessageBox button is not a string or object but ' + t + '.');
        return null;
    };

    /**
     * The markup for the message box's view.
     * @property {string} defaultViewMarkup
     * @static
     */
    MessageBox.defaultViewMarkup = [
        '<div data-view="plugins/messageBox" data-bind="css: getClass(), style: getStyle()">',
            '<div class="modal-header">',
                '<h3 data-bind="html: title"></h3>',
            '</div>',
            '<div class="modal-body">',
                '<p class="message" data-bind="html: message"></p>',
            '</div>',
            '<div class="modal-footer">',
                '<!-- ko foreach: options -->',
                '<button data-bind="click: function () { $parent.selectOption($parent.getButtonValue($data)); }, text: $parent.getButtonText($data), css: $parent.getButtonClass($index)"></button>',
                '<!-- /ko -->',
                '<div style="clear:both;"></div>',
            '</div>',
        '</div>'
    ].join('\n');

    function ensureDialogInstance(objOrModuleId) {
        return system.defer(function (dfd) {
            if (system.isString(objOrModuleId)) {
                system.acquire(objOrModuleId).then(function (module) {
                    dfd.resolve(system.resolveObject(module));
                }).fail(function (err) {
                    system.error('Failed to load dialog module (' + objOrModuleId + '). Details: ' + err.message);
                });
            } else {
                dfd.resolve(objOrModuleId);
            }
        }).promise();
    }

    /**
     * @class DialogModule
     * @static
     */
    dialog = {
        /**
         * The constructor function used to create message boxes.
         * @property {MessageBox} MessageBox
         */
        MessageBox: MessageBox,
        /**
         * The css zIndex that the last dialog was displayed at.
         * @property {number} currentZIndex
         */
        currentZIndex: 1050,
        /**
         * Gets the next css zIndex at which a dialog should be displayed.
         * @method getNextZIndex
         * @return {number} The next usable zIndex.
         */
        getNextZIndex: function () {
            return ++this.currentZIndex;
        },
        /**
         * Determines whether or not there are any dialogs open.
         * @method isOpen
         * @return {boolean} True if a dialog is open. false otherwise.
         */
        isOpen: ko.computed(function() {
            return dialogCount() > 0;
        }),
        /**
         * Gets the dialog context by name or returns the default context if no name is specified.
         * @method getContext
         * @param {string} [name] The name of the context to retrieve.
         * @return {DialogContext} True context.
         */
        getContext: function (name) {
            return contexts[name || 'default'];
        },
        /**
         * Adds (or replaces) a dialog context.
         * @method addContext
         * @param {string} name The name of the context to add.
         * @param {DialogContext} dialogContext The context to add.
         */
        addContext: function (name, dialogContext) {
            dialogContext.name = name;
            contexts[name] = dialogContext;

            var helperName = 'show' + name.substr(0, 1).toUpperCase() + name.substr(1);
            this[helperName] = function (obj, activationData) {
                return this.show(obj, activationData, name);
            };
        },
        createCompositionSettings: function (obj, dialogContext) {
            var settings = {
                model: obj,
                activate: false,
                transition: false
            };

            if (dialogContext.binding) {
                settings.binding = dialogContext.binding;
            }

            if (dialogContext.attached) {
                settings.attached = dialogContext.attached;
            }

            if (dialogContext.compositionComplete) {
                settings.compositionComplete = dialogContext.compositionComplete;
            }

            return settings;
        },
        /**
         * Gets the dialog model that is associated with the specified object.
         * @method getDialog
         * @param {object} obj The object for whom to retrieve the dialog.
         * @return {Dialog} The dialog model.
         */
        getDialog: function (obj) {
            if (obj) {
                return obj.__dialog__;
            }

            return undefined;
        },
        /**
         * Closes the dialog associated with the specified object.
         * @method close
         * @param {object} obj The object whose dialog should be closed.
         * @param {object} results* The results to return back to the dialog caller after closing.
         */
        close: function (obj) {
            var theDialog = this.getDialog(obj);
            if (theDialog) {
                var rest = Array.prototype.slice.call(arguments, 1);
                theDialog.close.apply(theDialog, rest);
            }
        },
        /**
         * Shows a dialog.
         * @method show
         * @param {object|string} obj The object (or moduleId) to display as a dialog.
         * @param {object} [activationData] The data that should be passed to the object upon activation.
         * @param {string} [context] The name of the dialog context to use. Uses the default context if none is specified.
         * @return {Promise} A promise that resolves when the dialog is closed and returns any data passed at the time of closing.
         */
        show: function (obj, activationData, context) {
            var that = this;
            var dialogContext = contexts[context || 'default'];

            return system.defer(function (dfd) {
                ensureDialogInstance(obj).then(function (instance) {
                    var dialogActivator = activator.create();

                    dialogActivator.activateItem(instance, activationData).then(function (success) {
                        if (success) {
                            var theDialog = instance.__dialog__ = {
                                owner: instance,
                                context: dialogContext,
                                activator: dialogActivator,
                                close: function () {
                                    var args = arguments;
                                    dialogActivator.deactivateItem(instance, true).then(function (closeSuccess) {
                                        if (closeSuccess) {
                                            dialogCount(dialogCount() - 1);
                                            dialogContext.removeHost(theDialog);
                                            delete instance.__dialog__;

                                            if (args.length === 0) {
                                                dfd.resolve();
                                            } else if (args.length === 1) {
                                                dfd.resolve(args[0]);
                                            } else {
                                                dfd.resolve.apply(dfd, args);
                                            }
                                        }
                                    });
                                }
                            };

                            theDialog.settings = that.createCompositionSettings(instance, dialogContext);
                            dialogContext.addHost(theDialog);

                            dialogCount(dialogCount() + 1);
                            composition.compose(theDialog.host, theDialog.settings);
                        } else {
                            dfd.resolve(false);
                        }
                    });
                });
            }).promise();
        },
        /**
         * Shows a message box.
         * @method showMessage
         * @param {string} message The message to display in the dialog.
         * @param {string} [title] The title message.
         * @param {string[]} [options] The options to provide to the user.
         * @param {boolean} [autoclose] Automatically close the the message box when clicking outside?
         * @param {Object} [settings] Custom settings for this instance of the messsage box, used to change classes and styles.
         * @return {Promise} A promise that resolves when the message box is closed and returns the selected option.
         */
        showMessage: function (message, title, options, autoclose, settings) {
            if (system.isString(this.MessageBox)) {
                return dialog.show(this.MessageBox, [
                    message,
                    title || MessageBox.defaultTitle,
                    options || MessageBox.defaultOptions,
                    autoclose || false,
                    settings || {}
                ]);
            }

            return dialog.show(new this.MessageBox(message, title, options, autoclose, settings));
        },
        /**
         * Installs this module into Durandal; called by the framework. Adds `app.showDialog` and `app.showMessage` convenience methods.
         * @method install
         * @param {object} [config] Add a `messageBox` property to supply a custom message box constructor. Add a `messageBoxView` property to supply custom view markup for the built-in message box. You can also use messageBoxViewUrl to specify the view url.
         */
        install: function (config) {
            app.showDialog = function (obj, activationData, context) {
                return dialog.show(obj, activationData, context);
            };

            app.closeDialog = function () {
                return dialog.close.apply(dialog, arguments);
            };

            app.showMessage = function (message, title, options, autoclose, settings) {
                return dialog.showMessage(message, title, options, autoclose, settings);
            };

            if (config.messageBox) {
                dialog.MessageBox = config.messageBox;
            }

            if (config.messageBoxView) {
                dialog.MessageBox.prototype.getView = function () {
                    return viewEngine.processMarkup(config.messageBoxView);
                };
            }

            if (config.messageBoxViewUrl) {
                dialog.MessageBox.setViewUrl(config.messageBoxViewUrl);
            }
        }
    };

    /**
     * @class DialogContext
     */
    dialog.addContext('default', {
        blockoutOpacity: 0.2,
        removeDelay: 200,
        /**
         * In this function, you are expected to add a DOM element to the tree which will serve as the "host" for the modal's composed view. You must add a property called host to the modalWindow object which references the dom element. It is this host which is passed to the composition module.
         * @method addHost
         * @param {Dialog} theDialog The dialog model.
         */
        addHost: function (theDialog) {
            var body = $('body');
            var blockout = $('<div class="modalBlockout"></div>')
                .css({ 'z-index': dialog.getNextZIndex(), 'opacity': this.blockoutOpacity })
                .appendTo(body);

            var host = $('<div class="modalHost"></div>')
                .css({ 'z-index': dialog.getNextZIndex() })
                .appendTo(body);

            theDialog.host = host.get(0);
            theDialog.blockout = blockout.get(0);

            if (!dialog.isOpen()) {
                theDialog.oldBodyMarginRight = body.css("margin-right");
                theDialog.oldInlineMarginRight = body.get(0).style.marginRight;

                var html = $("html");
                var oldBodyOuterWidth = body.outerWidth(true);
                var oldScrollTop = html.scrollTop();
                $("html").css("overflow-y", "hidden");
                var newBodyOuterWidth = $("body").outerWidth(true);
                body.css("margin-right", (newBodyOuterWidth - oldBodyOuterWidth + parseInt(theDialog.oldBodyMarginRight, 10)) + "px");
                html.scrollTop(oldScrollTop); // necessary for Firefox
            }
        },
        /**
         * This function is expected to remove any DOM machinery associated with the specified dialog and do any other necessary cleanup.
         * @method removeHost
         * @param {Dialog} theDialog The dialog model.
         */
        removeHost: function (theDialog) {
            $(theDialog.host).css('opacity', 0);
            $(theDialog.blockout).css('opacity', 0);

            setTimeout(function () {
                ko.removeNode(theDialog.host);
                ko.removeNode(theDialog.blockout);
            }, this.removeDelay);

            if (!dialog.isOpen()) {
                var html = $("html");
                var oldScrollTop = html.scrollTop(); // necessary for Firefox.
                html.css("overflow-y", "").scrollTop(oldScrollTop);

                if (theDialog.oldInlineMarginRight) {
                    $("body").css("margin-right", theDialog.oldBodyMarginRight);
                } else {
                    $("body").css("margin-right", '');
                }
            }
        },
        attached: function (view) {
            //To prevent flickering in IE8, we set visibility to hidden first, and later restore it
            $(view).css("visibility", "hidden");
        },
        /**
         * This function is called after the modal is fully composed into the DOM, allowing your implementation to do any final modifications, such as positioning or animation. You can obtain the original dialog object by using `getDialog` on context.model.
         * @method compositionComplete
         * @param {DOMElement} child The dialog view.
         * @param {DOMElement} parent The parent view.
         * @param {object} context The composition context.
         */
        compositionComplete: function (child, parent, context) {
            var theDialog = dialog.getDialog(context.model);
            var $child = $(child);
            var loadables = $child.find("img").filter(function () {
                //Remove images with known width and height
                var $this = $(this);
                return !(this.style.width && this.style.height) && !($this.attr("width") && $this.attr("height"));
            });

            $child.data("predefinedWidth", $child.get(0).style.width);

            var setDialogPosition = function (childView, objDialog) {
                //Setting a short timeout is need in IE8, otherwise we could do this straight away
                setTimeout(function () {
                    var $childView = $(childView);

                    objDialog.context.reposition(childView);

                    $(objDialog.host).css('opacity', 1);
                    $childView.css("visibility", "visible");

                    $childView.find('.autofocus').first().focus();
                }, 1);
            };

            setDialogPosition(child, theDialog);
            loadables.load(function () {
                setDialogPosition(child, theDialog);
            });

            if ($child.hasClass('autoclose') || context.model.autoclose) {
                $(theDialog.blockout).click(function () {
                    theDialog.close();
                });
            }
        },
        /**
         * This function is called to reposition the model view.
         * @method reposition
         * @param {DOMElement} view The dialog view.
         */
        reposition: function (view) {
            var $view = $(view),
                $window = $(window);

            //We will clear and then set width for dialogs without width set 
            if (!$view.data("predefinedWidth")) {
                $view.css({ width: '' }); //Reset width
            }
            var width = $view.outerWidth(false),
                height = $view.outerHeight(false),
                windowHeight = $window.height() - 10, //leave at least 10 pixels free
                windowWidth = $window.width() - 10, //leave at least 10 pixels free
                constrainedHeight = Math.min(height, windowHeight),
                constrainedWidth = Math.min(width, windowWidth);

            $view.css({
                'margin-top': (-constrainedHeight / 2).toString() + 'px',
                'margin-left': (-constrainedWidth / 2).toString() + 'px'
            });

            if (height > windowHeight) {
                $view.css("overflow-y", "auto").outerHeight(windowHeight);
            } else {
                $view.css({
                    "overflow-y": "",
                    "height": ""
                });
            }

            if (width > windowWidth) {
                $view.css("overflow-x", "auto").outerWidth(windowWidth);
            } else {
                $view.css("overflow-x", "");

                if (!$view.data("predefinedWidth")) {
                    //Ensure the correct width after margin-left has been set
                    $view.outerWidth(constrainedWidth);
                } else {
                    $view.css("width", $view.data("predefinedWidth"));
                }
            }
        }
    });

    return dialog;
});
