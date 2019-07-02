(function (define) {
  define(['toastr', 'abp-web-resources'], function (toastr, abp) {
    return (function () {

      if (!toastr) {
        return;
      }

      if (!abp) {
        return;
      }

      /* DEFAULTS *************************************************/

      toastr.options.positionClass = 'toast-bottom-right';

      /* NOTIFICATION *********************************************/

      var showNotification = function (type, message, title, options) {
        toastr[type](message, title, options);
      };

      abp.notify.success = function (message, title, options) {
        showNotification('success', message, title, options);
      };

      abp.notify.info = function (message, title, options) {
        showNotification('info', message, title, options);
      };

      abp.notify.warn = function (message, title, options) {
        showNotification('warning', message, title, options);
      };

      abp.notify.error = function (message, title, options) {
        showNotification('error', message, title, options);
      };

      return abp;
    })();
  });
}(typeof define === 'function' && define.amd
  ? define
  : function (deps, factory) {
    if (typeof module !== 'undefined' && module.exports) {
      module.exports = factory(require('toastr'), require('abp-web-resources'));
    } else {
      window.abp = factory(window.toastr, window.abp);
    }
  }));
