/*!
 * Angular Material Design
 * https://github.com/angular/material
 * @license MIT
 * v0.5.0
 */
(function() {
angular.module('ngMaterial', ["ng","ngAnimate","ngAria","material.core","material.decorators","material.animations","material.components.backdrop","material.components.bottomSheet","material.components.button","material.components.card","material.components.checkbox","material.components.content","material.components.dialog","material.components.divider","material.components.icon","material.components.list","material.components.progressCircular","material.components.progressLinear","material.components.radioButton","material.components.sidenav","material.components.slider","material.components.sticky","material.components.subheader","material.components.swipe","material.components.switch","material.components.tabs","material.components.textField","material.components.toast","material.components.toolbar","material.components.tooltip","material.components.whiteframe","material.services.aria","material.services.attrBind","material.services.compiler","material.services.interimElement","material.services.media","material.services.registry","material.services.theming"]);})();

(function() {
  /**
   * Angular Mds initialization function that validates environment
   * requirements.
   */
  angular.module('material.core', [] )
    .run(function validateEnvironment() {

      if (typeof Hammer === 'undefined') {
        throw new Error(
          'ngMaterial requires HammerJS to be preloaded.'
        );
      }

    });




})();

(function() {
angular.module('material.core')
.constant('$mdConstant', {
  KEY_CODE: {
    ENTER: 13,
    ESCAPE: 27,
    SPACE: 32,
    LEFT_ARROW : 37,
    UP_ARROW : 38,
    RIGHT_ARROW : 39,
    DOWN_ARROW : 40
  }
});
})();

(function() {
angular.module('material.core')
.factory('$mdUtil', ['$cacheFactory', function($cacheFactory) {
  var SPECIAL_CHARS_REGEXP = /([\:\-\_]+(.))/g;
  /* for nextUid() function below */
  var uid = ['0','0','0'];

  var Util;
  return Util = {
    now: window.performance ? angular.bind(window.performance, window.performance.now) : Date.now,

    /**
     * Checks if the specified element has an ancestor (ancestor being parent, grandparent, etc)
     * with the given attribute defined. 
     *
     * Also pass in an optional `limit` (levels of ancestry to scan), default 4.
     */
    ancestorHasAttribute: function ancestorHasAttribute(element, attrName, limit) {
      limit = limit || 4;
      var current = element;
      while (limit-- && current.length) {
        if (current[0].hasAttribute && current[0].hasAttribute(attrName)) {
          return true;
        }
        current = current.parent();
      }
      return false;
    },

    /**
     * Checks to see if the element or its parents are disabled.
     * @param element DOM element to start scanning for `disabled` attribute
     * @param limit Number of parent levels that should be scanned; defaults to 4
     * @returns {*} Boolean
     */
    isParentDisabled: function isParentDisabled(element, limit) {
      return Util.ancestorHasAttribute(element, 'disabled', limit);
    },

    /**
     * Checks if two elements have the same parent
     */
    elementIsSibling: function elementIsSibling(element, otherElement) {
      return element.parent().length && 
        (element.parent()[0] === otherElement.parent()[0]);
    },

    /**
     * Converts snake_case to camelCase.
     * @param name Name to normalize
     */
    camelCase: function camelCase(name) {
      return name
        .replace(SPECIAL_CHARS_REGEXP, function(_, separator, letter, offset) {
          return offset ? letter.toUpperCase() : letter;
        });
    },

    /**
     * Selects 'n' words from a string
     * for use in an HTML attribute
     */
    stringFromTextBody: function stringFromTextBody(textBody, numWords) {
      var string = textBody.trim();

      if(string.split(/\s+/).length > numWords){
        string = textBody.split(/\s+/).slice(1, (numWords + 1)).join(" ") + '...';
      }
      return string;
    },

    /**
     * Publish the iterator facade to easily support iteration and accessors
     * @see iterator below
     */
    iterator: iterator,

    /**
     * @see cacheFactory below
     */
    cacheFactory: cacheFactory,

    // Returns a function, that, as long as it continues to be invoked, will not
    // be triggered. The function will be called after it stops being called for
    // N milliseconds. If `immediate` is passed, trigger the function on the
    // leading edge, instead of the trailing.
    debounce: function debounce(func, wait, immediate) {
      var timeout;
      return function debounced() {
        var context = this, args = arguments;
        clearTimeout(timeout);
        timeout = setTimeout(function() {
          timeout = null;
          if (!immediate) func.apply(context, args);
        }, wait);
        if (immediate && !timeout) func.apply(context, args);
      };
    },

    // Returns a function that can only be triggered every `delay` milliseconds.
    // In other words, the function will not be called unless it has been more
    // than `delay` milliseconds since the last call.
    throttle: function throttle(func, delay) {
      var recent;
      return function throttled() {
        var context = this;
        var args = arguments;
        var now = Util.now();

        if (!recent || recent - now > delay) {
          func.apply(context, args);
          recent = now;
        }
      };
    },

    /**
     * Wraps an element with a tag
     *
     * @param el element to wrap
     * @param tag tag to wrap it with
     * @param [className] optional class to apply to the wrapper
     * @returns new element
     *
     */
    wrap: function(el, tag, className) {
      if(el.hasOwnProperty(0)) { el = el[0]; }
      var wrapper = document.createElement(tag);
      wrapper.className += className;
      wrapper.appendChild(el.parentNode.replaceChild(wrapper, el));
      return angular.element(wrapper);
    },

    /**
     * nextUid, from angular.js.
     * A consistent way of creating unique IDs in angular. The ID is a sequence of alpha numeric
     * characters such as '012ABC'. The reason why we are not using simply a number counter is that
     * the number string gets longer over time, and it can also overflow, where as the nextId
     * will grow much slower, it is a string, and it will never overflow.
     *
     * @returns an unique alpha-numeric string
     */
    nextUid: function() {
      var index = uid.length;
      var digit;

      while(index) {
        index--;
        digit = uid[index].charCodeAt(0);
        if (digit == 57 /*'9'*/) {
          uid[index] = 'A';
          return uid.join('');
        }
        if (digit == 90  /*'Z'*/) {
          uid[index] = '0';
        } else {
          uid[index] = String.fromCharCode(digit + 1);
          return uid.join('');
        }
      }
      uid.unshift('0');
      return uid.join('');
    },

    // Stop watchers and events from firing on a scope without destroying it,
    // by disconnecting it from its parent and its siblings' linked lists.
    disconnectScope: function disconnectScope(scope) {
      if (!scope) return;

      // we can't destroy the root scope or a scope that has been already destroyed
      if (scope.$root === scope) return;
      if (scope.$$destroyed ) return;

      var parent = scope.$parent;
      scope.$$disconnected = true;

      // See Scope.$destroy
      if (parent.$$childHead === scope) parent.$$childHead = scope.$$nextSibling;
      if (parent.$$childTail === scope) parent.$$childTail = scope.$$prevSibling;
      if (scope.$$prevSibling) scope.$$prevSibling.$$nextSibling = scope.$$nextSibling;
      if (scope.$$nextSibling) scope.$$nextSibling.$$prevSibling = scope.$$prevSibling;

      scope.$$nextSibling = scope.$$prevSibling = null;

    },

    // Undo the effects of disconnectScope above.
    reconnectScope: function reconnectScope(scope) {
      if (!scope) return;

      // we can't disconnect the root node or scope already disconnected
      if (scope.$root === scope) return;
      if (!scope.$$disconnected) return;

      var child = scope;

      var parent = child.$parent;
      child.$$disconnected = false;
      // See Scope.$new for this logic...
      child.$$prevSibling = parent.$$childTail;
      if (parent.$$childHead) {
        parent.$$childTail.$$nextSibling = child;
        parent.$$childTail = child;
      } else {
        parent.$$childHead = parent.$$childTail = child;
      }
    }
  };

  /*
   * iterator is a list facade to easily support iteration and accessors
   *
   * @param items Array list which this iterator will enumerate
   * @param reloop Boolean enables iterator to consider the list as an endless reloop
   */
  function iterator(items, reloop) {
    var trueFn = function() { return true; };

    reloop = !!reloop;
    var _items = items || [ ];

    // Published API
    return {
      items: getItems,
      count: count,

      inRange: inRange,
      contains: contains,
      indexOf: indexOf,
      itemAt: itemAt,

      findBy: findBy,

      add: add,
      remove: remove,

      first: first,
      last: last,
      next: next,
      previous: previous,

      hasPrevious: hasPrevious,
      hasNext: hasNext

    };

    /*
     * Publish copy of the enumerable set
     * @returns {Array|*}
     */
    function getItems() {
      return [].concat(_items);
    }

    /*
     * Determine length of the list
     * @returns {Array.length|*|number}
     */
    function count() {
      return _items.length;
    }

    /*
     * Is the index specified valid
     * @param index
     * @returns {Array.length|*|number|boolean}
     */
    function inRange(index) {
      return _items.length && ( index > -1 ) && (index < _items.length );
    }

    /*
     * Can the iterator proceed to the next item in the list; relative to
     * the specified item.
     *
     * @param item
     * @returns {Array.length|*|number|boolean}
     */
    function hasNext(item) {
      return item ? inRange(indexOf(item) + 1) : false;
    }

    /*
     * Can the iterator proceed to the previous item in the list; relative to
     * the specified item.
     *
     * @param item
     * @returns {Array.length|*|number|boolean}
     */
    function hasPrevious(item) {
      return item ? inRange(indexOf(item) - 1) : false;
    }

    /*
     * Get item at specified index/position
     * @param index
     * @returns {*}
     */
    function itemAt(index) {
      return inRange(index) ? _items[index] : null;
    }

    /*
     * Find all elements matching the key/value pair
     * otherwise return null
     *
     * @param val
     * @param key
     *
     * @return array
     */
    function findBy(key, val) {
      return _items.filter(function(item) {
        return item[key] === val;
      });
    }

    /*
     * Add item to list
     * @param item
     * @param index
     * @returns {*}
     */
    function add(item, index) {
      if ( !item ) return -1;

      if (!angular.isNumber(index)) {
        index = _items.length;
      }

      _items.splice(index, 0, item);

      return indexOf(item);
    }

    /*
     * Remove item from list...
     * @param item
     */
    function remove(item) {
      if ( contains(item) ){
        _items.splice(indexOf(item), 1);
      }
    }

    /*
     * Get the zero-based index of the target item
     * @param item
     * @returns {*}
     */
    function indexOf(item) {
      return _items.indexOf(item);
    }

    /*
     * Boolean existence check
     * @param item
     * @returns {boolean}
     */
    function contains(item) {
      return item && (indexOf(item) > -1);
    }

    /*
     * Find the next item. If reloop is true and at the end of the list, it will 
     * go back to the first item. If given ,the `validate` callback will be used
     * determine whether the next item is valid. If not valid, it will try to find the
     * next item again.
     * @param item
     * @param {optional} validate
     * @returns {*}
     */
    function next(item, validate) {
      validate = validate || trueFn;

      if (contains(item)) {
        var index = indexOf(item) + 1,
        found = inRange(index) ? _items[ index ] : (reloop ? first() : null);

        return validate(found) ? found : next(found, validate);
      }

      return null;
    }

    /*
     * Find the previous item. If reloop is true and at the beginning of the list, it will 
     * go back to the last item. If given ,the `validate` callback will be used
     * determine whether the previous item is valid. If not valid, it will try to find the
     * previous item again.
     * @param item
     * @param {optional} validate
     * @returns {*}
     */
    function previous(item, validate) {
      validate = validate || trueFn;

      if (contains(item)) {
        var index = indexOf(item) - 1,
        found = inRange(index) ? _items[ index ] : (reloop ? last() : null);

        return validate(found) ? found : previous(found, validate);
      }

      return null;
    }

    /*
     * Return first item in the list
     * @returns {*}
     */
    function first() {
      return _items.length ? _items[0] : null;
    }

    /*
     * Return last item in the list...
     * @returns {*}
     */
    function last() {
      return _items.length ? _items[_items.length - 1] : null;
    }
  }

  /*
   * Angular's $cacheFactory doesn't have a keys() method,
   * so we add one ourself.
   */
  function cacheFactory(id, options) {
    var cache = $cacheFactory(id, options);

    var keys = {};
    cache._put = cache.put;
    cache.put = function(k,v) {
      keys[k] = true;
      return cache._put(k, v);
    };
    cache._remove = cache.remove;
    cache.remove = function(k) {
      delete keys[k];
      return cache._remove(k);
    };

    cache.keys = function() {
      return Object.keys(keys);
    };

    return cache;
  }
}]);

/* 
 * Since removing jQuery from the demos, some code that uses `element.focus()` is broken.
 *
 * We need to add `element.focus()`, because it's testable unlike `element[0].focus`.
 *
 * TODO(ajoslin): This should be added in a better place later.
 */

angular.element.prototype.focus = angular.element.prototype.focus || function() {
  if (this.length) {
    this[0].focus();
  }
  return this;
};
angular.element.prototype.blur = angular.element.prototype.blur || function() {
  if (this.length) {
    this[0].blur();
  }
  return this;
};
})();

(function() {
angular.module('material.decorators', [])
.config(['$provide', function($provide) {
  $provide.decorator('$$rAF', ['$delegate', '$rootScope', rAFDecorator]);

  function rAFDecorator($$rAF, $rootScope) {

    /**
     * Use this to debounce events that come in often.
     * The debounced function will always use the *last* invocation before the
     * coming frame.
     *
     * For example, window resize events that fire many times a second:
     * If we set to use an raf-debounced callback on window resize, then
     * our callback will only be fired once per frame, with the last resize
     * event that happened before that frame.
     *
     * @param {function} callback function to debounce
     */
    $$rAF.debounce = function(cb) {
      var queueArgs, alreadyQueued, queueCb, context;
      return function debounced() {
        queueArgs = arguments;
        context = this;
        queueCb = cb;
        if (!alreadyQueued) {
          alreadyQueued = true;
          $$rAF(function() {
            queueCb.apply(context, queueArgs);
            alreadyQueued = false;
          });
        }
      };
    };

    return $$rAF;
  }
}]);
})();

(function() {
/*
 * @ngdoc module
 * @name material.components.animate
 * @description
 *
 * Ink and Popup Effects
 */
angular.module('material.animations', ['material.core'])
  .service('$mdEffects', [ 
    '$rootElement', 
    '$$rAF', 
    '$sniffer',
    '$q',
    MdEffects
  ]);

/*
 * @ngdoc service
 * @name $mdEffects
 * @module material.components.animate
 *
 * @description
 * The `$mdEffects` service provides a simple API for various
 * Material Design effects.
 *
 * @returns A `$mdEffects` object with the following properties:
 * - `{function(element,styles,duration)}` `inkBar` - starts ink bar
 * animation on specified DOM element
 * - `{function(element,parentElement,clickElement)}` `popIn` - animated show of element overlayed on parent element
 * - `{function(element,parentElement)}` `popOut` - animated close of popup overlay
 *
 */
function MdEffects($rootElement, $$rAF, $sniffer, $q) {

  var webkit = /webkit/i.test($sniffer.vendorPrefix);
  function vendorProperty(name) {
    return webkit ? 
      ('webkit' + name.charAt(0).toUpperCase() + name.substring(1)) :
      name;
  }

  var self;
  // Publish API for effects...
  return self = {
    popIn: popIn,

    /* Constants */
    TRANSITIONEND_EVENT: 'transitionend' + (webkit ? ' webkitTransitionEnd' : ''),
    ANIMATIONEND_EVENT: 'animationend' + (webkit ? ' webkitAnimationEnd' : ''),

    TRANSFORM: vendorProperty('transform'),
    TRANSITION: vendorProperty('transition'),
    TRANSITION_DURATION: vendorProperty('transitionDuration'),
    ANIMATION_PLAY_STATE: vendorProperty('animationPlayState'),
    ANIMATION_DURATION: vendorProperty('animationDuration'),
    ANIMATION_NAME: vendorProperty('animationName'),
    ANIMATION_TIMING: vendorProperty('animationTimingFunction'),
    ANIMATION_DIRECTION: vendorProperty('animationDirection')
  };

  // **********************************************************
  // API Methods
  // **********************************************************
  function popIn(element, parentElement, clickElement) {
    var deferred = $q.defer();
    parentElement.append(element);

    var startPos;
    if (clickElement) {
      var clickRect = clickElement[0].getBoundingClientRect();
      startPos = translateString(
        clickRect.left - element[0].offsetWidth,
        clickRect.top - element[0].offsetHeight, 
        0
      ) + ' scale(0.2)';
    } else {
      startPos = 'translate3d(0,100%,0) scale(0.5)';
    }

    element
      .css(self.TRANSFORM, startPos)
      .css('opacity', 0);
    
    $$rAF(function() {
      $$rAF(function() {
        element
          .addClass('md-active')
          .css(self.TRANSFORM, '')
          .css('opacity', '')
          .on(self.TRANSITIONEND_EVENT, finished);
      });
    });

    function finished(ev) {
      //Make sure this transitionend didn't bubble up from a child
      if (ev.target === element[0]) {
        element.off(self.TRANSITIONEND_EVENT, finished);
        deferred.resolve();
      }
    }

    return deferred.promise;
  }

  // **********************************************************
  // Utility Methods
  // **********************************************************


  function translateString(x, y, z) {
    return 'translate3d(' + Math.floor(x) + 'px,' + Math.floor(y) + 'px,' + Math.floor(z) + 'px)';
  }

}

})();

(function() {

angular.module('material.animations')

.directive('inkRipple', [
  '$mdInkRipple',
  InkRippleDirective
])

.factory('$mdInkRipple', [
  '$window',
  '$$rAF',
  '$mdEffects',
  '$timeout',
  '$mdUtil',
  InkRippleService
]);

function InkRippleDirective($mdInkRipple) {
  return function(scope, element, attr) {
    if (attr.inkRipple == 'checkbox') {
      $mdInkRipple.attachCheckboxBehavior(element);
    } else {
      $mdInkRipple.attachButtonBehavior(element);
    }
  };
}

function InkRippleService($window, $$rAF, $mdEffects, $timeout, $mdUtil) {

  return {
    attachButtonBehavior: attachButtonBehavior,
    attachCheckboxBehavior: attachCheckboxBehavior,
    attach: attach
  };

  function attachButtonBehavior(element) {
    return attach(element, {
      mousedown: true,
      center: false,
      animationDuration: 350,
      mousedownPauseTime: 175,
      animationName: 'inkRippleButton',
      animationTimingFunction: 'linear'
    });
  }

  function attachCheckboxBehavior(element) {
    return attach(element, {
      mousedown: true,
      center: true,
      animationDuration: 300,
      mousedownPauseTime: 180,
      animationName: 'inkRippleCheckbox',
      animationTimingFunction: 'linear'
    });
  }

  function attach(element, options) {
    // Parent element with noink attr? Abort.
    if (element.controller('noink')) return angular.noop;
    var contentParent = element.controller('mdContent');

    options = angular.extend({
      mousedown: true,
      hover: true,
      focus: true,
      center: false,
      animationDuration: 300,
      mousedownPauseTime: 150,
      animationName: '',
      animationTimingFunction: 'linear'
    }, options || {});

    var rippleContainer;
    var node = element[0];
    var hammertime = new Hammer(node);

    if (options.mousedown) {
      hammertime.on('hammer.input', onInput);
    }

    // Publish self-detach method if desired...
    return function detach() {
      hammertime.destroy();
      if (rippleContainer) {
        rippleContainer.remove();
      }
    };

    function rippleIsAllowed() {
      return !element[0].hasAttribute('disabled');
    }

    function createRipple(left, top, positionsAreAbsolute) {

      var rippleEl = angular.element('<div class="md-ripple">')
            .css($mdEffects.ANIMATION_DURATION, options.animationDuration + 'ms')
            .css($mdEffects.ANIMATION_NAME, options.animationName)
            .css($mdEffects.ANIMATION_TIMING, options.animationTimingFunction)
            .on($mdEffects.ANIMATIONEND_EVENT, function() {
              rippleEl.remove();
            });

      if (!rippleContainer) {
        rippleContainer = angular.element('<div class="md-ripple-container">');
        element.append(rippleContainer);
      }
      rippleContainer.append(rippleEl);

      var containerWidth = rippleContainer.prop('offsetWidth');

      if (options.center) {
        left = containerWidth / 2;
        top = rippleContainer.prop('offsetHeight') / 2;
      } else if (positionsAreAbsolute) {
        var elementRect = node.getBoundingClientRect();
        left -= elementRect.left;
        top -= elementRect.top;
      }

      if (contentParent) {
        top += contentParent.$element.prop('scrollTop');
      }

      var css = {
        'background-color': $window.getComputedStyle(rippleEl[0]).color || 
          $window.getComputedStyle(node).color,
        'border-radius': (containerWidth / 2) + 'px',

        left: (left - containerWidth / 2) + 'px',
        width: containerWidth + 'px',

        top: (top - containerWidth / 2) + 'px',
        height: containerWidth + 'px'
      };
      css[$mdEffects.ANIMATION_DURATION] = options.fadeoutDuration + 'ms';
      rippleEl.css(css);

      return rippleEl;
    }

    var pauseTimeout;
    var rippleEl;
    function onInput(ev) {
      if (ev.eventType === Hammer.INPUT_START && ev.isFirst && rippleIsAllowed()) {

        rippleEl = createRipple(ev.center.x, ev.center.y, true);
        pauseTimeout = $timeout(function() {
          rippleEl && rippleEl.css($mdEffects.ANIMATION_PLAY_STATE, 'paused');
        }, options.mousedownPauseTime, false);

        rippleEl.on('$destroy', function() {
          rippleEl = null;
        });

      } else if (ev.eventType === Hammer.INPUT_END && ev.isFinal) {
        $timeout.cancel(pauseTimeout);
        rippleEl && rippleEl.css($mdEffects.ANIMATION_PLAY_STATE, '');
      }
    }

  }

}
})();

(function() {
angular.module('material.animations')

/**
 * noink/nobar/nostretch directive: make any element that has one of
 * these attributes be given a controller, so that other directives can 
 * `require:` these and see if there is a `no<xxx>` parent attribute.
 *
 * @usage
 * <hljs lang="html">
 * <parent noink>
 *   <child detect-no>
 *   </child>
 * </parent>
 * </hljs>
 *
 * <hljs lang="js">
 * myApp.directive('detectNo', function() {
 *   return {
 *     require: ['^?noink', ^?nobar'],
 *     link: function(scope, element, attr, ctrls) {
 *       var noinkCtrl = ctrls[0];
 *       var nobarCtrl = ctrls[1];
 *       if (noInkCtrl) {
 *         alert("the noink flag has been specified on an ancestor!");
 *       }
 *       if (nobarCtrl) {
 *         alert("the nobar flag has been specified on an ancestor!");
 *       }
 *     }
 *   };
 * });
 * </hljs>
 */
.directive({
  noink: attrNoDirective(),
  nobar: attrNoDirective(),
  nostretch: attrNoDirective()
});

function attrNoDirective() {
  return function() {
    return {
      controller: angular.noop
    };
  };
}
})();

(function() {
/*
 * @ngdoc module
 * @name material.components.backdrop
 * @description Backdrop
 */

/**
 * @ngdoc directive
 * @name mdBackdrop
 * @module material.components.backdrop
 *
 * @restrict E
 *
 * @description
 * `<md-backdrop>` is a backdrop element used by other coponents, such as dialog and bottom sheet.
 * Apply class `opaque` to make the backdrop use the theme backdrop color.
 *
 */
angular.module('material.components.backdrop', [
  'material.services.theming'
])
.directive('mdBackdrop', [
  '$mdTheming',
  BackdropDirective
]);

function BackdropDirective($mdTheming) {
  return $mdTheming;
}
})();

(function() {
/**
 * @ngdoc module
 * @name material.components.bottomSheet
 * @description
 * BottomSheet
 */
angular.module('material.components.bottomSheet', [
  'material.components.backdrop',
  'material.services.interimElement',
  'material.services.theming'
])
.directive('mdBottomSheet', [
  MdBottomSheetDirective
])
.factory('$mdBottomSheet', [
  '$$interimElement',
  '$animate',
  '$mdEffects',
  '$timeout',
  '$$rAF',
  '$compile',
  '$mdTheming',
  MdBottomSheet
]);

function MdBottomSheetDirective() {
  return {
    restrict: 'E'
  };
}

/**
 * @ngdoc service
 * @name $mdBottomSheet
 * @module material.components.bottomSheet
 *
 * @description
 * `$mdBottomSheet` opens a bottom sheet over the app and provides a simple promise API.
 *
 * ### Restrictions
 * 
 * - The bottom sheet's template must have an outer `<md-bottom-sheet>` element.
 * - Add the `md-grid` class to the bottom sheet for a grid layout.
 * - Add the `md-list` class to the bottom sheet for a list layout.
 *
 * @usage
 * <hljs lang="html">
 * <div ng-controller="MyController">
 *   <md-button ng-click="openBottomSheet()">
 *     Open a Bottom Sheet!
 *   </md-button>
 * </div>
 * </hljs>
 * <hljs lang="js">
 * var app = angular.module('app', ['ngMaterial']);
 * app.controller('MyController', function($scope, $mdBottomSheet) {
 *   $scope.openBottomSheet = function() {
 *     $mdBottomSheet.show({
 *       template: '<md-bottom-sheet>Hello!</md-bottom-sheet>'
 *     });
 *   };
 * });
 * </hljs>
 */

 /**
 * @ngdoc method
 * @name $mdBottomSheet#show
 *
 * @description
 * Show a bottom sheet with the specified options.
 *
 * @param {object} options An options object, with the following properties:
 *
 *   - `templateUrl` - `{string=}`: The url of an html template file that will
 *   be used as the content of the bottom sheet. Restrictions: the template must
 *   have an outer `md-bottom-sheet` element.
 *   - `template` - `{string=}`: Same as templateUrl, except this is an actual
 *   template string.
 *   - `controller` - `{string=}`: The controller to associate with this bottom sheet.
 *   - `locals` - `{string=}`: An object containing key/value pairs. The keys will
 *   be used as names of values to inject into the controller. For example, 
 *   `locals: {three: 3}` would inject `three` into the controller with the value
 *   of 3.
 *   - `targetEvent` - `{DOMClickEvent=}`: A click's event object. When passed in as an option, 
 *   the location of the click will be used as the starting point for the opening animation
 *   of the the dialog.
 *   - `resolve` - `{object=}`: Similar to locals, except it takes promises as values
 *   and the bottom sheet will not open until the promises resolve.
 *   - `controllerAs` - `{string=}`: An alias to assign the controller to on the scope.
 *
 * @returns {promise} A promise that can be resolved with `$mdBottomSheet.hide()` or
 * rejected with `$mdBottomSheet.cancel()`.
 */

/**
 * @ngdoc method
 * @name $mdBottomSheet#hide
 *
 * @description
 * Hide the existing bottom sheet and resolve the promise returned from 
 * `$mdBottomSheet.show()`.
 *
 * @param {*=} response An argument for the resolved promise.
 *
 */

/**
 * @ngdoc method
 * @name $mdBottomSheet#cancel
 *
 * @description
 * Hide the existing bottom sheet and reject the promise returned from 
 * `$mdBottomSheet.show()`.
 *
 * @param {*=} response An argument for the rejected promise.
 *
 */

function MdBottomSheet($$interimElement, $animate, $mdEffects, $timeout, $$rAF, $compile, $mdTheming) {
  var backdrop;

  var $mdBottomSheet;
  return $mdBottomSheet = $$interimElement({
    themable: true,
    targetEvent: null,
    onShow: onShow,
    onRemove: onRemove,
  });

  function onShow(scope, element, options) {
    // Add a backdrop that will close on click
    backdrop = $compile('<md-backdrop class="md-opaque ng-enter">')(scope);
    backdrop.on('click touchstart', function() {
      $timeout($mdBottomSheet.cancel);
    });
    $mdTheming.inherit(backdrop, options.parent);

    $animate.enter(backdrop, options.parent, null);

    var bottomSheet = new BottomSheet(element);
    options.bottomSheet = bottomSheet;

    // Give up focus on calling item
    options.targetEvent && angular.element(options.targetEvent.target).blur();
    $mdTheming.inherit(bottomSheet.element, options.parent);

    return $animate.enter(bottomSheet.element, options.parent);

  }

  function onRemove(scope, element, options) {
    var bottomSheet = options.bottomSheet;
    $animate.leave(backdrop);
    return $animate.leave(bottomSheet.element).then(function() {
      bottomSheet.cleanup();

      // Restore focus
      options.targetEvent && angular.element(options.targetEvent.target).focus();
    });
  }

  /**
   * BottomSheet class to apply bottom-sheet behavior to an element
   */
  function BottomSheet(element) {
    var MAX_OFFSET = 80; // amount past the bottom of the element that we can drag down, this is same as in _bottomSheet.scss
    var WIGGLE_AMOUNT = 20; // point where it starts to get "harder" to drag
    var CLOSING_VELOCITY = 10; // how fast we need to flick down to close the sheet
    var startY, lastY, velocity, transitionDelay, startTarget;

    // coercion incase $mdCompiler returns multiple elements
    element = element.eq(0);

    element.on('touchstart', onTouchStart);
    element.on('touchmove', onTouchMove);
    element.on('touchend', onTouchEnd);

    return {
      element: element,
      cleanup: function cleanup() {
        element.off('touchstart', onTouchStart);
        element.off('touchmove', onTouchMove);
        element.off('touchend', onTouchEnd);
      }
    };

    function onTouchStart(e) {
      e.preventDefault();
      startTarget = e.target;
      startY = getY(e);
      
      // Disable transitions on transform so that it feels fast
      transitionDelay = element.css($mdEffects.TRANSITION_DURATION);
      element.css($mdEffects.TRANSITION_DURATION, '0s');
    }

    function onTouchEnd(e) {
      // Re-enable the transitions on transforms
      element.css($mdEffects.TRANSITION_DURATION, transitionDelay);

      var currentY = getY(e);
      // If we didn't scroll much, and we didn't change targets, assume its a click
      if ( Math.abs(currentY - startY) < 5  && e.target == startTarget) {
        angular.element(e.target).triggerHandler('click');
      } else {
        // If they went fast enough, trigger a close.
        if (velocity > CLOSING_VELOCITY) {
          $timeout($mdBottomSheet.cancel);

        // Otherwise, untransform so that we go back to our normal position
        } else {
          setTransformY(undefined);
        }
      }
    }

    function onTouchMove(e) {
      var currentY = getY(e);
      var delta = currentY - startY;

      velocity = currentY - lastY;
      lastY = currentY;
      
      // Do some conversion on delta to get a friction-like effect
      delta = adjustedDelta(delta);
      setTransformY(delta + MAX_OFFSET);
    }

    /**
     * Helper function to find the Y aspect of various touch events.
     **/
    function getY(e) {
      var touch = e.touches && e.touches.length ? e.touches[0] : e.changedTouches[0];
      return touch.clientY;
    }

    /**
     * Transform the element along the y-axis
     **/
    function setTransformY(amt) {
      if (amt === null || amt === undefined) {
        element.css($mdEffects.TRANSFORM, '');
      } else {
        element.css($mdEffects.TRANSFORM, 'translate3d(0, ' + amt + 'px, 0)');
      }
    }

    // Returns a new value for delta that will never exceed MAX_OFFSET_AMOUNT
    // Will get harder to exceed it as you get closer to it
    function adjustedDelta(delta) {
      if ( delta < 0  && delta < -MAX_OFFSET + WIGGLE_AMOUNT) {
        delta = -delta;
        var base = MAX_OFFSET - WIGGLE_AMOUNT;
        delta = Math.max(-MAX_OFFSET, -Math.min(MAX_OFFSET - 5, base + ( WIGGLE_AMOUNT * (delta - base)) / MAX_OFFSET) - delta / 50);
      }

      return delta;
    }
  }

}
})();

(function() {
/**
 * @ngdoc module
 * @name material.components.button
 * @description
 *
 * Button
 */
angular.module('material.components.button', [
  'material.core',
  'material.animations',
  'material.services.aria',
  'material.services.theming'
])
  .directive('mdButton', [
    'ngHrefDirective',
    '$mdInkRipple',
    '$mdAria',
    '$mdUtil',
    '$mdTheming',
    MdButtonDirective
  ]);

/**
 * @ngdoc directive
 * @name mdButton
 * @module material.components.button
 *
 * @restrict E
 *
 * @description
 * `<md-button>` is a button directive with optional ink ripples (default enabled).
 *
 * @param {boolean=} noink If present, disable ripple ink effects.
 * @param {boolean=} disabled If present, disable tab selection.
 * @param {string=} type Optional attribute to specific button types (useful for forms); such as 'submit', etc.
 * @param {string=} ng-href Optional attribute to support both ARIA and link navigation
 * @param {string=} href Optional attribute to support both ARIA and link navigation
 * @param {string=} ariaLabel Publish the button label used by screen-readers for accessibility. Defaults to the button's text.
 *
 * @usage
 * <hljs lang="html">
 *  <md-button>Button</md-button>
 *  <br/>
 *  <md-button noink class="md-button-colored">
 *    Button (noInk)
 *  </md-button>
 *  <br/>
 *  <md-button disabled class="md-button-colored">
 *    Colored (disabled)
 *  </md-button>
 * </hljs>
 */
function MdButtonDirective(ngHrefDirectives, $mdInkRipple, $mdAria, $mdUtil, $mdTheming ) {
  var ngHrefDirective = ngHrefDirectives[0];

  return {
    restrict: 'E',
    compile: function(element, attr) {
      var innerElement;
      var attributesToCopy;


      // Add an inner anchor if the element has a `href` or `ngHref` attribute,
      // so this element can be clicked like a normal `<a>`.
      if (attr.ngHref || attr.href) {
        innerElement = angular.element('<a>');
        attributesToCopy = ['ng-href', 'href', 'rel', 'target'];
      // Otherwise, just add an inner button element (for form submission etc)
      } else {
        innerElement = angular.element('<button>');
        attributesToCopy = ['type', 'disabled', 'ng-disabled', 'form'];
      }

      angular.forEach(attributesToCopy, function(name) {
        var camelCaseName = $mdUtil.camelCase(name);
        if (attr.hasOwnProperty(camelCaseName)) {
          innerElement.attr(name, attr[camelCaseName]);
        }
      });

      innerElement
        .addClass('md-button-inner')
        .append(element.contents())
        // Since we're always passing focus to the inner element,
        // add a focus class to the outer element so we can still style
        // it with focus.
        .on('focus', function() {
          element.addClass('focus');
        })
        .on('blur', function() {
          element.removeClass('focus');
        });

      element.
        append(innerElement)
        .attr('tabIndex', -1)
        //Always pass focus to innerElement
        .on('focus', function() {
          innerElement.focus();
        });

      return function postLink(scope, element, attr) {
        $mdTheming(element);
        $mdAria.expect(element, 'aria-label', element.text());
        $mdInkRipple.attachButtonBehavior(element);
      };
    }
  };

}
})();

(function() {
/**
 * @ngdoc module
 * @name material.components.card
 *
 * @description
 * Card components.
 */
angular.module('material.components.card', [
])
  .directive('mdCard', [
    mdCardDirective 
  ]);



/**
 * @ngdoc directive
 * @name mdCard
 * @module material.components.card
 *
 * @restrict E
 *
 * @description
 * The `<md-card>` directive is a container element used within `<md-content>` containers.
 *
 * Cards have constant width and variable heights; where the maximum height is limited to what can
 * fit within a single view on a platform, but it can temporarily expand as needed
 *
 * @usage
 * <hljs lang="html">
 * <md-card>
 *  <img src="/img/washedout.png" class="md-card-image">
 *  <h2>Paracosm</h2>
 *  <p>
 *    The titles of Washed Out's breakthrough song and the first single from Paracosm share the * two most important words in Ernest Greene's musical language: feel it. It's a simple request, as well...
 *  </p>
 * </md-card>
 * </hljs>
 *
 */
function mdCardDirective() {
  return {
    restrict: 'E',
    link: function($scope, $element, $attr) {
    }
  };
}
})();

(function() {
/**
 * @ngdoc module
 * @name material.components.checkbox
 * @description Checkbox module!
 */
angular.module('material.components.checkbox', [
  'material.core',
  'material.animations',
  'material.services.theming',
  'material.services.aria'
])
  .directive('mdCheckbox', [ 
    'inputDirective',
    '$mdInkRipple',
    '$mdAria',
    '$mdConstant',
    '$mdTheming',
    MdCheckboxDirective
  ]);

/**
 * @ngdoc directive
 * @name mdCheckbox
 * @module material.components.checkbox
 * @restrict E
 *
 * @description
 * The checkbox directive is used like the normal [angular checkbox](https://docs.angularjs.org/api/ng/input/input%5Bcheckbox%5D).
 *
 * @param {string} ngModel Assignable angular expression to data-bind to.
 * @param {string=} name Property name of the form under which the control is published.
 * @param {expression=} ngTrueValue The value to which the expression should be set when selected.
 * @param {expression=} ngFalseValue The value to which the expression should be set when not selected.
 * @param {string=} ngChange Angular expression to be executed when input changes due to user interaction with the input element.
 * @param {boolean=} noink Use of attribute indicates use of ripple ink effects
 * @param {boolean=} disabled Use of attribute indicates the switch is disabled: no ink effects and not selectable
 * @param {string=} ariaLabel Publish the button label used by screen-readers for accessibility. Defaults to the checkbox's text.
 *
 * @usage
 * <hljs lang="html">
 * <md-checkbox ng-model="isChecked" aria-label="Finished?">
 *   Finished ?
 * </md-checkbox>
 *
 * <md-checkbox noink ng-model="hasInk" aria-label="No Ink Effects">
 *   No Ink Effects
 * </md-checkbox>
 *
 * <md-checkbox disabled ng-model="isDisabled" aria-label="Disabled">
 *   Disabled
 * </md-checkbox>
 *
 * </hljs>
 *
 */
function MdCheckboxDirective(inputDirectives, $mdInkRipple, $mdAria, $mdConstant, $mdTheming) {
  var inputDirective = inputDirectives[0];

  var CHECKED_CSS = 'md-checked';

  return {
    restrict: 'E',
    transclude: true,
    require: '?ngModel',
    template: 
      '<div class="md-container" ink-ripple="checkbox">' +
        '<div class="md-icon"></div>' +
      '</div>' +
      '<div ng-transclude class="md-label"></div>',
    compile: compile
  };

  // **********************************************************
  // Private Methods
  // **********************************************************

  function compile (tElement, tAttrs) {

    tAttrs.type = 'checkbox';
    tAttrs.tabIndex = 0;
    tElement.attr('role', tAttrs.type);

    return function postLink(scope, element, attr, ngModelCtrl) {
      var checked = false;
      $mdTheming(element);

      // Create a mock ngModel if the user doesn't provide one
      ngModelCtrl = ngModelCtrl || {
        $setViewValue: function(value) {
          this.$viewValue = value;
        },
        $parsers: [],
        $formatters: []
      };

      $mdAria.expect(tElement, 'aria-label', true);

      // Reuse the original input[type=checkbox] directive from Angular core.
      // This is a bit hacky as we need our own event listener and own render
      // function.
      inputDirective.link.pre(scope, {
        on: angular.noop,
        0: {}
      }, attr, [ngModelCtrl]);

      element.on('click', listener);
      element.on('keypress', keypressHandler);
      ngModelCtrl.$render = render;

      function keypressHandler(ev) {
        if(ev.which === $mdConstant.KEY_CODE.SPACE) {
          ev.preventDefault();
          listener(ev);
        }
      }
      function listener(ev) {
        if (element[0].hasAttribute('disabled')) return;

        scope.$apply(function() {
          checked = !checked;
          ngModelCtrl.$setViewValue(checked, ev && ev.type);
          ngModelCtrl.$render();
        });
      }

      function render() {
        checked = ngModelCtrl.$viewValue;
        if(checked) {
          element.addClass(CHECKED_CSS);
        } else {
          element.removeClass(CHECKED_CSS);
        }
      }
    };
  }
}


})();

(function() {
/**
 * @ngdoc module
 * @name material.components.content
 *
 * @description
 * Scrollable content
 */
angular.module('material.components.content', [
  'material.services.theming',
  'material.services.registry'
])
  .directive('mdContent', [
    '$mdTheming',
    mdContentDirective
  ]);

/**
 * @ngdoc directive
 * @name mdContent
 * @module material.components.content
 *
 * @restrict E
 *
 * @description
 * The `<md-content>` directive is a container element useful for scrollable content
 *
 * ### Restrictions
 *
 * - Add the `md-padding` class to make the content padded.
 *
 * @usage
 * <hljs lang="html">
 *  <md-content class="md-padding">
 *      Lorem ipsum dolor sit amet, ne quod novum mei.
 *  </md-content>
 * </hljs>
 *
 */
function mdContentDirective($mdTheming) {
  return {
    restrict: 'E',
    controller: ['$scope', '$element', ContentController],
    link: function($scope, $element, $attr) {
      $mdTheming($element);
      $scope.$broadcast('$mdContentLoaded', $element);
    }
  };

  function ContentController($scope, $element) {
    this.$scope = $scope;
    this.$element = $element;
  }
}
})();

(function() {
/**
 * @ngdoc module
 * @name material.components.dialog
 */
angular.module('material.components.dialog', [
  'material.core',
  'material.animations',
  'material.components.backdrop',
  'material.services.compiler',
  'material.services.aria',
  'material.services.interimElement',
  'material.services.theming',
])
  .directive('mdDialog', [
    '$$rAF',
    '$mdTheming',
    MdDialogDirective
  ])
  .factory('$mdDialog', [
    '$timeout',
    '$rootElement',
    '$compile',
    '$mdEffects',
    '$animate',
    '$mdAria',
    '$$interimElement',
    '$mdUtil',
    '$mdConstant',
    '$mdTheming',
    MdDialogService
  ]);

function MdDialogDirective($$rAF, $mdTheming) {
  return {
    restrict: 'E',
    link: function(scope, element, attr) {
      $mdTheming(element);
      $$rAF(function() {
        var content = element[0].querySelector('md-content');
        if (content && content.scrollHeight > content.clientHeight) {
          element.addClass('md-content-overflow');
        }
      });
    }
  };
}

/**
 * @ngdoc service
 * @name $mdDialog
 * @module material.components.dialog
 *
 * @description
 * `$mdDialog` opens a dialog over the app and provides a simple promise API.
 *
 * ### Restrictions
 *
 * - The dialog is always given an isolate scope.
 * - The dialog's template must have an outer `<md-dialog>` element.
 *   Inside, use an `<md-content>` element for the dialog's content, and use
 *   an element with class `md-actions` for the dialog's actions.  
 *
 * @usage
 * <hljs lang="html">
 * <div ng-controller="MyController">
 *   <md-button ng-click="openDialog($event)">
 *     Open a Dialog from this button!
 *   </md-button>
 * </div>
 * </hljs>
 *
 * <hljs lang="js">
 * var app = angular.module('app', ['ngMaterial']);
 * app.controller('MyController', function($scope, $mdDialog) {
 *   $scope.openDialog = function($event) {
 *     $mdDialog.show({
 *       targetEvent: $event,
 *       controller: 'DialogController',
 *       template: 
 *         '<md-dialog>' +
 *         '  <md-content>Hello!</md-content>' +
 *         '  <div class="md-actions">' +
 *         '    <md-button ng-click="closeDialog()">' +
 *         '      Close' +
 *         '    </md-button>' +
 *         '  </div>' +
 *         '</md-dialog>'
 *     });
 *   };
 * });
 * app.controller('DialogController', function($scope, $mdDialog) {
 *   $scope.closeDialog = function() {
 *     $mdDialog.hide();
 *   };
 * });
 * </hljs>
 *
 */

/**
 *
 * @ngdoc method
 * @name $mdDialog#show
 *
 * @description
 * Show a dialog with the specified options.
 *
 * @param {object} options An options object, with the following properties:
 *   - `templateUrl` - `{string=}`: The url of a template that will be used as the content
 *   of the dialog. 
 *   - `template` - `{string=}`: Same as templateUrl, except this is an actual template string.
 *   - `targetEvent` - `{DOMClickEvent=}`: A click's event object. When passed in as an option, 
 *     the location of the click will be used as the starting point for the opening animation
 *     of the the dialog.
 *   - `hasBackdrop` - `{boolean=}`: Whether there should be an opaque backdrop behind the dialog.
 *     Default true.
 *   - `clickOutsideToClose` - `{boolean=}`: Whether the user can click outside the dialog to
 *     close it. Default true.
 *   - `escapeToClose` - `{boolean=}`: Whether the user can press escape to close the dialog.
 *     Default true.
 *   - `controller` - `{string=}`: The controller to associate with the dialog. The controller
 *     will be injected with the local `$hideDialog`, which is a function used to hide the dialog.
 *   - `locals` - `{object=}`: An object containing key/value pairs. The keys will be used as names
 *     of values to inject into the controller. For example, `locals: {three: 3}` would inject
 *     `three` into the controller, with the value 3.
 *   - `resolve` - `{object=}`: Similar to locals, except it takes promises as values, and the
 *     toast will not open until all of the promises resolve.
 *   - `controllerAs` - `{string=}`: An alias to assign the controller to on the scope.
 *   - `parent` - `{element=}`: The element to append the dialog to. Defaults to appending
 *     to the root element of the application.
 *
 * @returns {promise} A promise that can be resolved with `$mdDialog.hide()` or
 * rejected with `mdDialog.cancel()`.
 */

/**
 * @ngdoc method
 * @name $mdDialog#hide
 *
 * @description
 * Hide an existing dialog and resolve the promise returned from `$mdDialog.show()`.
 *
 * @param {*=} response An argument for the resolved promise.
 *
 */

/**
 * @ngdoc method
 * @name $mdDialog#cancel
 *
 * @description
 * Hide an existing dialog and reject the promise returned from `$mdDialog.show()`.
 *
 * @param {*=} response An argument for the rejected promise.
 *
 */

function MdDialogService($timeout, $rootElement, $compile, $mdEffects, $animate, $mdAria, $$interimElement, $mdUtil, $mdConstant, $mdTheming) {

  var $dialogService;
  return $dialogService = $$interimElement({
    hasBackdrop: true,
    isolateScope: true,
    onShow: onShow,
    onRemove: onRemove,
    clickOutsideToClose: true,
    escapeToClose: true,
    targetEvent: null,
    transformTemplate: function(template) {
      return '<div class="md-dialog-container">' + template + '</div>';
    }
  });

  function onShow(scope, element, options) {
    // Incase the user provides a raw dom element, always wrap it in jqLite
    options.parent = angular.element(options.parent);

    options.popInTarget = angular.element((options.targetEvent || {}).target); 
    var closeButton = findCloseButton();

    configureAria(element.find('md-dialog'));

    if (options.hasBackdrop) {
      options.backdrop = $compile('<md-backdrop class="md-opaque ng-enter">')(scope);
      $mdTheming.inherit(options.backdrop, options.parent);
      $animate.enter(options.backdrop, options.parent, null);
    }

    return $mdEffects.popIn(
      element, 
      options.parent, 
      options.popInTarget.length && options.popInTarget
    )
    .then(function() {
      if (options.escapeToClose) {
        options.rootElementKeyupCallback = function(e) {
          if (e.keyCode === $mdConstant.KEY_CODE.ESCAPE) {
            $timeout($dialogService.cancel);
          }
        };

        $rootElement.on('keyup', options.rootElementKeyupCallback);
      }

      if (options.clickOutsideToClose) {
        options.dialogClickOutsideCallback = function(e) {
          // Only close if we click the flex container outside the backdrop
          if (e.target === element[0]) {
            $timeout($dialogService.cancel);
          }
        };

        element.on('click', options.dialogClickOutsideCallback);
      }
      closeButton.focus();
    });


    function findCloseButton() {
      //If no element with class dialog-close, try to find the last
      //button child in md-actions and assume it is a close button
      var closeButton = element[0].querySelector('.dialog-close');
      if (!closeButton) {
        var actionButtons = element[0].querySelectorAll('.md-actions button');
        closeButton = actionButtons[ actionButtons.length - 1 ];
      }
      return angular.element(closeButton);
    }

  }

  function onRemove(scope, element, options) {

    if (options.backdrop) {
      $animate.leave(options.backdrop);
      element.data('backdrop', undefined);
    }
    if (options.escapeToClose) {
      $rootElement.off('keyup', options.rootElementKeyupCallback);
    }
    if (options.clickOutsideToClose) {
      element.off('click', options.dialogClickOutsideCallback);
    }
    return $animate.leave(element).then(function() {
      element.remove();
      options.popInTarget && options.popInTarget.focus();
    });

  }

  /**
   * Inject ARIA-specific attributes appropriate for Dialogs
   */
  function configureAria(element) {
    element.attr({
      'role': 'dialog'
    });

    var dialogContent = element.find('md-content');
    if (dialogContent.length === 0){
      dialogContent = element;
    }
    var defaultText = $mdUtil.stringFromTextBody(dialogContent.text(), 3);
    $mdAria.expect(element, 'aria-label', true, defaultText);
  }
}
})();

(function() {
/**
 * @ngdoc module
 * @name material.components.divider
 * @description Divider module!
 */
angular.module('material.components.divider', [
  'material.animations',
  'material.services.aria',
  'material.services.theming'
])
.directive('mdDivider', [
  '$mdTheming',
  MdDividerDirective
]);

function MdDividerController(){}

/**
 * @ngdoc directive
 * @name mdDivider
 * @module material.components.divider
 * @restrict E
 *
 * @description
 * Dividers group and separate content within lists and page layouts using strong visual and spatial distinctions. This divider is a thin rule, lightweight enough to not distract the user from content.
 *
 * @param {boolean=} inset Add this attribute to activate the inset divider style.
 * @usage
 * <hljs lang="html">
 * <md-divider></md-divider>
 *
 * <md-divider inset></md-divider>
 * </hljs>
 *
 */
function MdDividerDirective($mdTheming) {
  return {
    restrict: 'E',
    link: $mdTheming,
    controller: [MdDividerController]
  };
}
})();

(function() {
/*
 * @ngdoc module
 * @name material.components.icon
 * @description
 * Icon
 */
angular.module('material.components.icon', [])
  .directive('mdIcon', [
    mdIconDirective
  ]);

/*
 * @ngdoc directive
 * @name mdIcon
 * @module material.components.icon
 *
 * @restrict E
 *
 * @description
 * The `<md-icon>` directive is an element useful for SVG icons
 *
 * @usage
 * <hljs lang="html">
 *  <md-icon icon="/img/icons/ic_access_time_24px.svg">
 *  </md-icon>
 * </hljs>
 *
 */
function mdIconDirective() {
  return {
    restrict: 'E',
    template: '<object class="md-icon"></object>',
    compile: function(element, attr) {
      var object = angular.element(element[0].children[0]);
      if(angular.isDefined(attr.icon)) {
        object.attr('data', attr.icon);
      }
    }
  };
}
})();

(function() {
/**
 * @ngdoc module
 * @name material.components.list
 * @description
 * List module
 */
angular.module('material.components.list', [])

.directive('mdList', [
  mdListDirective
])
.directive('mdItem', [
  mdItemDirective
]);

/**
 * @ngdoc directive
 * @name mdList
 * @module material.components.list
 *
 * @restrict E
 *
 * @description
 * The `<md-list>` directive is a list container for 1..n `<md-item>` tags.
 *
 * @usage
 * <hljs lang="html">
 * <md-list>
 *  <md-item ng-repeat="item in todos">
 *    <div class="md-tile-left">
 *      <img ng-src="{{item.face}}" class="face" alt="{{item.who}}">
 *    </div>
 *    <div class="md-tile-content">
 *      <h3>{{item.what}}</h3>
 *      <h4>{{item.who}}</h4>
 *      <p>
 *        {{item.notes}}
 *      </p>
 *    </div>
 *
 *  </md-item>
 * </md-list>
 * </hljs>
 *
 */
function mdListDirective() {
  return {
    restrict: 'E',
    link: function($scope, $element, $attr) {
      $element.attr({
        'role' : 'list'
      });
    }
  };
}

/**
 * @ngdoc directive
 * @name mdItem
 * @module material.components.list
 *
 * @restrict E
 *
 * @description
 * The `<md-item>` directive is a container intended for row items in a `<md-list>` container.
 *
 * @usage
 * <hljs lang="html">
 *  <md-list>
 *    <md-item>
 *            Item content in list
 *    </md-item>
 *  </md-list>
 * </hljs>
 *
 */
function mdItemDirective() {
  return {
    restrict: 'E',
    link: function($scope, $element, $attr) {
      $element.attr({
        'role' : 'listitem'
      });
    }
  };
}
})();

(function() {
/**
 * @ngdoc module
 * @name material.components.progressCircular
 * @description Circular Progress module!
 */
angular.module('material.components.progressCircular', [
  'material.animations',
  'material.services.aria',
  'material.services.theming',
])
  .directive('mdProgressCircular', [
    '$$rAF',
    '$mdEffects',
    '$mdTheming',
    MdProgressCircularDirective
  ]);

/**
 * @ngdoc directive
 * @name mdProgressCircular
 * @module material.components.progressCircular
 * @restrict E
 *
* @description
 * The circular progress directive is used to make loading content in your app as delightful and painless as possible by minimizing the amount of visual change a user sees before they can view and interact with content.
 *
 * For operations where the percentage of the operation completed can be determined, use a determinate indicator. They give users a quick sense of how long an operation will take.
 *
 * For operations where the user is asked to wait a moment while something finishes up, and it’s not necessary to expose what's happening behind the scenes and how long it will take, use an indeterminate indicator.
 *
 * @param {string} mode Select from one of two modes: determinate and indeterminate.
 * @param {number=} value In determinate mode, this number represents the percentage of the circular progress. Default: 0
 * @param {number=} diameter This specifies the diamter of the circular progress. Default: 48
 *
 * @usage
 * <hljs lang="html">
 * <md-progress-circular mode="determinate" value="..."></md-progress-circular>
 *
 * <md-progress-circular mode="determinate" ng-value="..."></md-progress-circular>
 *
 * <md-progress-circular mode="determinate" value="..." diameter="100"></md-progress-circular>
 *
 * <md-progress-circular mode="indeterminate"></md-progress-circular>
 * </hljs>
 */
function MdProgressCircularDirective($$rAF, $mdEffects, $mdTheming) {
  var fillRotations = new Array(101),
    fixRotations = new Array(101);

  for (var i = 0; i < 101; i++) {
    var percent = i / 100;
    var rotation = Math.floor(percent * 180);

    fillRotations[i] = 'rotate(' + rotation.toString() + 'deg)';
    fixRotations[i] = 'rotate(' + (rotation * 2).toString() + 'deg)';
  }

  return {
    restrict: 'E',
    template: 
      '<div class="md-wrapper1"><div class="md-wrapper2"><div class="md-circle">' +
        '<div class="md-mask md-full">' +
          '<div class="md-fill"></div>' +
        '</div>' +
        '<div class="md-mask md-half">' +
          '<div class="md-fill"></div>' +
          '<div class="md-fill md-fix"></div>' +
        '</div>' +
        '<div class="md-shadow"></div>' +
      '</div>' +
      '<div class="md-inset"></div></div></div>',
    compile: compile
  };

  function compile(tElement, tAttrs, transclude) {
    tElement.attr('aria-valuemin', 0);
    tElement.attr('aria-valuemax', 100);
    tElement.attr('role', 'progressbar');

    return postLink;
  }

  function postLink(scope, element, attr) {
    $mdTheming(element);
    var circle = element[0],
      fill = circle.querySelectorAll('.md-fill, .md-mask.md-full'),
      fix = circle.querySelectorAll('.md-fill.md-fix'),
      i, clamped, fillRotation, fixRotation;

    var diameter = attr.diameter || 48;
    var scale = diameter/48;

    circle.style[$mdEffects.TRANSFORM] = 'scale(' + scale.toString() + ')';

    attr.$observe('value', function(value) {
      clamped = clamp(value);
      fillRotation = fillRotations[clamped];
      fixRotation = fixRotations[clamped];

      element.attr('aria-valuenow', clamped);

      for (i = 0; i < fill.length; i++) {
        fill[i].style[$mdEffects.TRANSFORM] = fillRotation;
      }

      for (i = 0; i < fix.length; i++) {
        fix[i].style[$mdEffects.TRANSFORM] = fixRotation;
      }
    });
  }

  function clamp(value) {
    if (value > 100) {
      return 100;
    }

    if (value < 0) {
      return 0;
    }

    return Math.ceil(value || 0);
  }
}
})();

(function() {
/**
 * @ngdoc module
 * @name material.components.progressLinear
 * @description Linear Progress module!
 */
angular.module('material.components.progressLinear', [
  'material.animations',
  'material.services.theming',
  'material.services.aria'
])
.directive('mdProgressLinear', [
  '$$rAF', 
  '$mdEffects',
  '$mdTheming',
  MdProgressLinearDirective
]);

/**
 * @ngdoc directive
 * @name mdProgressLinear
 * @module material.components.progressLinear
 * @restrict E
 *
 * @description
 * The linear progress directive is used to make loading content in your app as delightful and painless as possible by minimizing the amount of visual change a user sees before they can view and interact with content. Each operation should only be represented by one activity indicator—for example, one refresh operation should not display both a refresh bar and an activity circle.
 *
 * For operations where the percentage of the operation completed can be determined, use a determinate indicator. They give users a quick sense of how long an operation will take.
 *
 * For operations where the user is asked to wait a moment while something finishes up, and it’s not necessary to expose what's happening behind the scenes and how long it will take, use an indeterminate indicator.
 *
 * @param {string} mode Select from one of four modes: determinate, indeterminate, buffer or query.
 * @param {number=} value In determinate and buffer modes, this number represents the percentage of the primary progress bar. Default: 0
 * @param {number=} secondaryValue In the buffer mode, this number represents the precentage of the secondary progress bar. Default: 0
 *
 * @usage
 * <hljs lang="html">
 * <md-progress-linear mode="determinate" value="..."></md-progress-linear>
 *
 * <md-progress-linear mode="determinate" ng-value="..."></md-progress-linear>
 *
 * <md-progress-linear mode="indeterminate"></md-progress-linear>
 *
 * <md-progress-linear mode="buffer" value="..." secondaryValue="..."></md-progress-linear>
 *
 * <md-progress-linear mode="query"></md-progress-linear>
 * </hljs>
 */
function MdProgressLinearDirective($$rAF, $mdEffects, $mdTheming) {

  return {
    restrict: 'E',
    template: '<div class="md-container">' +
      '<div class="md-dashed"></div>' +
      '<div class="md-bar md-bar1"></div>' +
      '<div class="md-bar md-bar2"></div>' +
      '</div>',
    compile: compile
  };
  
  function compile(tElement, tAttrs, transclude) {
    tElement.attr('aria-valuemin', 0);
    tElement.attr('aria-valuemax', 100);
    tElement.attr('role', 'progressbar');

    return postLink;
  }
  function postLink(scope, element, attr) {
    $mdTheming(element);
    var bar1Style = element[0].querySelector('.md-bar1').style,
      bar2Style = element[0].querySelector('.md-bar2').style,
      container = angular.element(element[0].querySelector('.md-container'));

    attr.$observe('value', function(value) {
      if (attr.mode == 'query') {
        return;
      }

      var clamped = clamp(value);
      element.attr('aria-valuenow', clamped);
      bar2Style[$mdEffects.TRANSFORM] = progressLinearTransforms[clamped];
    });

    attr.$observe('secondaryvalue', function(value) {
      bar1Style[$mdEffects.TRANSFORM] = progressLinearTransforms[clamp(value)];
    });

    $$rAF(function() {
      container.addClass('md-ready');
    });
  }

  function clamp(value) {
    if (value > 100) {
      return 100;
    }

    if (value < 0) {
      return 0;
    }

    return Math.ceil(value || 0);
  }
}


// **********************************************************
// Private Methods
// **********************************************************
var progressLinearTransforms = (function() {
  var values = new Array(101);
  for(var i = 0; i < 101; i++){
    values[i] = makeTransform(i);
  }

  return values;

  function makeTransform(value){
    var scale = value/100;
    var translateX = (value-100)/2;
    return 'translateX(' + translateX.toString() + '%) scale(' + scale.toString() + ', 1)';
  }
})();
})();

(function() {

/**
 * @ngdoc module
 * @name material.components.radioButton
 * @description radioButton module!
 */
angular.module('material.components.radioButton', [
  'material.core',
  'material.animations',
  'material.services.aria',
  'material.services.theming'
])
  .directive('mdRadioGroup', [
    '$mdUtil',
    '$mdConstant',
    '$mdTheming',
    mdRadioGroupDirective
  ])
  .directive('mdRadioButton', [
    '$mdAria',
    '$mdUtil',
    '$mdTheming',
    mdRadioButtonDirective
  ]);

/**
 * @ngdoc directive
 * @module material.components.radioButton
 * @name mdRadioGroup
 *
 * @restrict E
 *
 * @description
 * The `<md-radio-group>` directive identifies a grouping
 * container for the 1..n grouped radio buttons; specified using nested
 * `<md-radio-button>` tags.
 *
 * @param {string} ngModel Assignable angular expression to data-bind to.
 * @param {boolean=} noink Use of attribute indicates flag to disable ink ripple effects.
 *
 * @usage
 * <hljs lang="html">
 * <md-radio-group ng-model="selected">
 *
 *   <md-radio-button
 *        ng-repeat="d in colorOptions"
 *        ng-value="d.value" aria-label="{{ d.label }}">
 *
 *          {{ d.label }}
 *
 *   </md-radio-button>
 *
 * </md-radio-group>
 * </hljs>
 *
 */
function mdRadioGroupDirective($mdUtil, $mdConstant, $mdTheming) {
  RadioGroupController.prototype = createRadioGroupControllerProto();

  return {
    restrict: 'E',
    controller: ['$element', RadioGroupController],
    require: ['mdRadioGroup', '?ngModel'],
    link: link
  };

  function link(scope, element, attr, ctrls) {
    $mdTheming(element);
    var rgCtrl = ctrls[0],
      ngModelCtrl = ctrls[1] || {
        $setViewValue: angular.noop
      };

    function keydownListener(ev) {
      if (ev.which === $mdConstant.KEY_CODE.LEFT_ARROW) {
        ev.preventDefault();
        rgCtrl.selectPrevious();
      }
      else if (ev.which === $mdConstant.KEY_CODE.RIGHT_ARROW) {
        ev.preventDefault();
        rgCtrl.selectNext();
      }
    }

    rgCtrl.init(ngModelCtrl);

    element.attr({
      'role': 'radiogroup',
      'tabIndex': '0'
    })
    .on('keydown', keydownListener);
  }

  function RadioGroupController($element) {
    this._radioButtonRenderFns = [];
    this.$element = $element;
  }

  function createRadioGroupControllerProto() {
    return {
      init: function(ngModelCtrl) {
        this._ngModelCtrl = ngModelCtrl;
        this._ngModelCtrl.$render = angular.bind(this, this.render);
      },
      add: function(rbRender) {
        this._radioButtonRenderFns.push(rbRender);
      },
      remove: function(rbRender) {
        var index = this._radioButtonRenderFns.indexOf(rbRender);
        if (index !== -1) {
          this._radioButtonRenderFns.splice(index, 1);
        }
      },
      render: function() {
        this._radioButtonRenderFns.forEach(function(rbRender) {
          rbRender();
        });
      },
      setViewValue: function(value, eventType) {
        this._ngModelCtrl.$setViewValue(value, eventType);
        // update the other radio buttons as well
        this.render();
      },
      getViewValue: function() {
        return this._ngModelCtrl.$viewValue;
      },
      selectNext: function() {
        return changeSelectedButton(this.$element, 1);
      },
      selectPrevious : function() {
        return changeSelectedButton(this.$element, -1);
      },
      setActiveDescendant: function (radioId) {
        this.$element.attr('aria-activedescendant', radioId);
      }
    };
  }
  /**
   * Change the radio group's selected button by a given increment.
   * If no button is selected, select the first button.
   */
  function changeSelectedButton(parent, increment) {
    // Coerce all child radio buttons into an array, then wrap then in an iterator
    var buttons = $mdUtil.iterator(
      Array.prototype.slice.call(parent[0].querySelectorAll('md-radio-button')),
      true
    );

    if (buttons.count()) {
      var selected = parent[0].querySelector('md-radio-button.md-checked');
      var target = buttons[increment < 0 ? 'previous' : 'next'](selected) || buttons.first();
      // Activate radioButton's click listener (triggerHandler won't create a real click event)
      angular.element(target).triggerHandler('click');
    }
  }

}

/**
 * @ngdoc directive
 * @module material.components.radioButton
 * @name mdRadioButton
 *
 * @restrict E
 *
 * @description
 * The `<md-radio-button>`directive is the child directive required to be used within `<md-radioo-group>` elements.
 *
 * While similar to the `<input type="radio" ng-model="" value="">` directive,
 * the `<md-radio-button>` directive provides ink effects, ARIA support, and
 * supports use within named radio groups.
 *
 * @param {string} ngModel Assignable angular expression to data-bind to.
 * @param {string=} ngChange Angular expression to be executed when input changes due to user
 *    interaction with the input element.
 * @param {string} ngValue Angular expression which sets the value to which the expression should
 *    be set when selected.*
 * @param {string} value The value to which the expression should be set when selected.
 * @param {string=} name Property name of the form under which the control is published.
 * @param {string=} ariaLabel Publish the button label used by screen-readers for accessibility. Defaults to the radio button's text.
 *
 * @usage
 * <hljs lang="html">
 *
 * <md-radio-button value="1" aria-label="Label 1">
 *   Label 1
 * </md-radio-button>
 *
 * <md-radio-button ng-model="color" ng-value="specialValue" aria-label="Green">
 *   Green
 * </md-radio-button>
 *
 * </hljs>
 *
 */
function mdRadioButtonDirective($mdAria, $mdUtil, $mdTheming) {

  var CHECKED_CSS = 'md-checked';

  return {
    restrict: 'E',
    require: '^mdRadioGroup',
    transclude: true,
    template: '<div class="md-container" ink-ripple="checkbox">' +
                '<div class="md-off"></div>' +
                '<div class="md-on"></div>' +
              '</div>' +
              '<div ng-transclude class="md-label"></div>',
    link: link
  };

  function link(scope, element, attr, rgCtrl) {
    var lastChecked;

    $mdTheming(element);
    configureAria(element, scope);

    rgCtrl.add(render);
    attr.$observe('value', render);

    element
      .on('click', listener)
      .on('$destroy', function() {
        rgCtrl.remove(render);
      });

    function listener(ev) {
      if (element[0].hasAttribute('disabled')) return;

      scope.$apply(function() {
        rgCtrl.setViewValue(attr.value, ev && ev.type);
      });
    }

    function render() {
      var checked = (rgCtrl.getViewValue() === attr.value);
      if (checked === lastChecked) {
        return;
      }
      lastChecked = checked;
      element.attr('aria-checked', checked);
      if (checked) {
        element.addClass(CHECKED_CSS);
        rgCtrl.setActiveDescendant(element.attr('id'));
      } else {
        element.removeClass(CHECKED_CSS);
      }
    }
    /**
     * Inject ARIA-specific attributes appropriate for each radio button
     */
    function configureAria( element, scope ){
      scope.ariaId = buildAriaID();

      element.attr({
        'id' :  scope.ariaId,
        'role' : 'radio',
        'aria-checked' : 'false'
      });

      $mdAria.expect(element, 'aria-label', true);

      /**
       * Build a unique ID for each radio button that will be used with aria-activedescendant.
       * Preserve existing ID if already specified.
       * @returns {*|string}
       */
      function buildAriaID() {
        return attr.id || ( 'radio' + "_" + $mdUtil.nextUid() );
      }
    }
  }
}


})();

(function() {
/**
 * @ngdoc module
 * @name material.components.sidenav
 *
 * @description
 * A Sidenav QP component.
 */
angular.module('material.components.sidenav', [
  'material.core',
  'material.services.registry',
  'material.services.media',
  'material.components.backdrop',
  'material.services.theming',
  'material.animations'
])
  .factory('$mdSidenav', [
    '$mdComponentRegistry', 
    mdSidenavService 
  ])
  .directive('mdSidenav', [
    '$timeout',
    '$animate',
    '$parse',
    '$mdMedia',
    '$mdConstant',
    '$compile',
    '$mdTheming',
    mdSidenavDirective 
  ])
  .controller('$mdSidenavController', [
    '$scope',
    '$element',
    '$attrs',
    '$timeout',
    '$mdSidenav',
    '$mdComponentRegistry',
    mdSidenavController 
  ]);
  
/*
 * @private
 * @ngdoc object
 * @name mdSidenavController
 * @module material.components.sidenav
 *
 * @description
 * The controller for mdSidenav components.
 */
function mdSidenavController($scope, $element, $attrs, $timeout, $mdSidenav, $mdComponentRegistry) {

  var self = this;

  $mdComponentRegistry.register(this, $attrs.componentId);

  this.isOpen = function() {
    return !!$scope.isOpen;
  };
  this.toggle = function() {
    $scope.isOpen = !$scope.isOpen;
  };
  this.open = function() {
    $scope.isOpen = true;
  };
  this.close = function() {
    $scope.isOpen = false;
  };
}

/*
 * @private
 * @ngdoc service
 * @name $mdSidenav
 * @module material.components.sidenav
 *
 * @description
 * $mdSidenav makes it easy to interact with multiple sidenavs
 * in an app.
 *
 * @usage
 *
 * ```javascript
 * // Toggle the given sidenav
 * $mdSidenav(componentId).toggle();
 *
 * // Open the given sidenav
 * $mdSidenav(componentId).open();
 *
 * // Close the given sidenav
 * $mdSidenav(componentId).close();
 * ```
 */
function mdSidenavService($mdComponentRegistry) {
  return function(handle) {
    var instance = $mdComponentRegistry.get(handle);
    if(!instance) {
      $mdComponentRegistry.notFoundError(handle);
    }

    return {
      isOpen: function() {
        return instance && instance.isOpen();
      },
      toggle: function() {
        instance && instance.toggle();
      },
      open: function() {
        instance && instance.open();
      },
      close: function() {
        instance && instance.close();
      }
    };
  };
}

/**
 * @ngdoc directive
 * @name mdSidenav
 * @module material.components.sidenav
 * @restrict E
 *
 * @description
 *
 * A Sidenav component that can be opened and closed programatically.
 *
 * By default, upon opening it will slide out on top of the main content area.
 *
 * @usage
 * <hljs lang="html">
 * <div layout="horizontal" ng-controller="MyController">
 *   <md-sidenav component-id="left" class="md-sidenav-left">
 *     Left Nav!
 *   </md-sidenav>
 *
 *   <md-content>
 *     Center Content
 *     <md-button ng-click="openLeftMenu()">
 *       Open Left Menu
 *     </md-button>
 *   </md-content>
 *
 *   <md-sidenav component-id="right" 
 *     is-locked-open="$media('min-width: 333px')"
 *     class="md-sidenav-right">
 *     Right Nav!
 *   </md-sidenav>
 * </div>
 * </hljs>
 *
 * <hljs lang="js">
 * var app = angular.module('myApp', ['ngMaterial']);
 * app.controller('MyController', function($scope, $mdSidenav) {
 *   $scope.openLeftMenu = function() {
 *     $mdSidenav('left').toggle();
 *   };
 * });
 * </hljs>
 *
 * @param {expression=} is-open A model bound to whether the sidenav is opened.
 * @param {string=} component-id componentId to use with $mdSidenav service.
 * @param {expression=} is-locked-open When this expression evalutes to true,
 * the sidenav 'locks open': it falls into the content's flow instead
 * of appearing over it. This overrides the `is-open` attribute.
 *
 * A $media() function is exposed to the is-locked-open attribute, which
 * can be given a media query or one of the `sm`, `md` or `lg` presets.
 * Examples:
 *
 *   - `<md-sidenav is-locked-open="shouldLockOpen"></md-sidenav>`
 *   - `<md-sidenav is-locked-open="$media('min-width: 1000px')"></md-sidenav>`
 *   - `<md-sidenav is-locked-open="$media('sm')"></md-sidenav>` <!-- locks open on small screens !-->
 */
function mdSidenavDirective($timeout, $animate, $parse, $mdMedia, $mdConstant, $compile, $mdTheming) {
  return {
    restrict: 'E',
    scope: {
      isOpen: '=?'
    },
    controller: '$mdSidenavController',
    compile: function(element) {
      element.addClass('md-closed');
      element.attr('tabIndex', '-1');
      return postLink;
    }
  };

  function postLink(scope, element, attr, sidenavCtrl) {
    var isLockedOpenParsed = $parse(attr.isLockedOpen);
    var backdrop = $compile(
      '<md-backdrop class="md-sidenav-backdrop md-opaque">'
    )(scope);

    $mdTheming.inherit(backdrop, element);

    scope.$watch('isOpen', setOpen);
    scope.$watch(function() {
      return isLockedOpenParsed(scope.$parent, {
        $media: $mdMedia
      });
    }, function(isLocked) {
      element.toggleClass('md-locked-open', !!isLocked);
      backdrop.toggleClass('md-locked-open', !!isLocked);
    });

    /**
     * Toggle the SideNav view and attach/detach listeners
     * @param isOpen
     */
    function setOpen(isOpen) {
      var parent = element.parent();

      parent[isOpen ? 'on' : 'off']('keydown', onKeyDown);
      $animate[isOpen ? 'enter' : 'leave'](backdrop, parent);
      backdrop[isOpen ? 'on' : 'off']('click', close);

      $animate[isOpen ? 'removeClass' : 'addClass'](element, 'md-closed').then(function() {
        // If we opened, and haven't closed again before the animation finished
        if (scope.isOpen) {
          element.focus();
        }
      });
    }

    /**
     * Auto-close sideNav when the `escape` key is pressed.
     * @param evt
     */
    function onKeyDown(ev) {
      if (ev.which === $mdConstant.KEY_CODE.ESCAPE) {
        close();
        ev.preventDefault();
        ev.stopPropagation();
      }
    }

    /**
     * With backdrop `clicks` or `escape` key-press, immediately
     * apply the CSS close transition... Then notify the controller
     * to close() and perform its own actions.
     */
    function close() {
      $timeout(function(){
        sidenavCtrl.close();
      });
    }

  }

}
})();

(function() {
/**
 * @ngdoc module
 * @name material.components.slider
 */
angular.module('material.components.slider', [
  'material.core',
  'material.animations',
  'material.services.aria',
  'material.services.theming'
])
.directive('mdSlider', [
  '$mdTheming',
  SliderDirective
]);

/**
 * @ngdoc directive
 * @name mdSlider
 * @module material.components.slider
 * @restrict E
 * @description
 * The `<md-slider>` component allows the user to choose from a range of
 * values.
 *
 * It has two modes: 'normal' mode, where the user slides between a wide range
 * of values, and 'discrete' mode, where the user slides between only a few
 * select values.
 *
 * To enable discrete mode, add the `discrete` attribute to a slider,
 * and use the `step` attribute to change the distance between
 * values the user is allowed to pick.
 *
 * @usage
 * <h4>Normal Mode</h4>
 * <hljs lang="html">
 * <md-slider ng-model="myValue" min="5" max="500">
 * </md-slider>
 * </hljs>
 * <h4>Discrete Mode</h4>
 * <hljs lang="html">
 * <md-slider discrete ng-model="myDiscreteValue" step="10" min="10" max="130">
 * </md-slider>
 * </hljs>
 *
 * @param {boolean=} discrete Whether to enable discrete mode.
 * @param {number=} step The distance between values the user is allowed to pick. Default 1.
 * @param {number=} min The minimum value the user is allowed to pick. Default 0.
 * @param {number=} max The maximum value the user is allowed to pick. Default 100.
 */
function SliderDirective($mdTheming) {
  return {
    scope: {},
    require: ['?ngModel', 'mdSlider'],
    controller: [
      '$scope',
      '$element',
      '$attrs',
      '$$rAF',
      '$window',
      '$mdEffects',
      '$mdAria',
      '$mdUtil',
      '$mdConstant',
      SliderController
    ],
    template:
      '<div class="md-track-container">' +
        '<div class="md-track"></div>' +
        '<div class="md-track md-track-fill"></div>' +
        '<div class="md-track-ticks"></div>' +
      '</div>' +
      '<div class="md-thumb-container">' +
        '<div class="md-thumb"></div>' +
        '<div class="md-focus-thumb"></div>' +
        '<div class="md-focus-ring"></div>' +
        '<div class="md-sign">' +
          '<span class="md-thumb-text" ng-bind="modelValue"></span>' +
        '</div>' +
        '<div class="md-disabled-thumb"></div>' +
      '</div>',
    link: postLink
  };

  function postLink(scope, element, attr, ctrls) {
    $mdTheming(element);
    var ngModelCtrl = ctrls[0] || {
      // Mock ngModelController if it doesn't exist to give us
      // the minimum functionality needed
      $setViewValue: function(val) {
        this.$viewValue = val;
        this.$viewChangeListeners.forEach(function(cb) { cb(); });
      },
      $parsers: [],
      $formatters: [],
      $viewChangeListeners: []
    };

    var sliderCtrl = ctrls[1];
    sliderCtrl.init(ngModelCtrl);
  }
}

/**
 * We use a controller for all the logic so that we can expose a few
 * things to unit tests
 */
function SliderController(scope, element, attr, $$rAF, $window, $mdEffects, $mdAria, $mdUtil, $mdConstant) {

  this.init = function init(ngModelCtrl) {
    var thumb = angular.element(element[0].querySelector('.md-thumb'));
    var thumbContainer = thumb.parent();
    var trackContainer = angular.element(element[0].querySelector('.md-track-container'));
    var activeTrack = angular.element(element[0].querySelector('.md-track-fill'));
    var tickContainer = angular.element(element[0].querySelector('.md-track-ticks'));
    var throttledRefreshDimensions = $mdUtil.throttle(refreshSliderDimensions, 5000);

    // Default values, overridable by attrs
    attr.min ? attr.$observe('min', updateMin) : updateMin(0);
    attr.max ? attr.$observe('max', updateMax) : updateMax(100);
    attr.step ? attr.$observe('step', updateStep) : updateStep(1);

    // We have to manually stop the $watch on ngDisabled because it exists
    // on the parent scope, and won't be automatically destroyed when
    // the component is destroyed.
    var stopDisabledWatch = angular.noop;
    if (attr.ngDisabled) {
      stopDisabledWatch = scope.$parent.$watch(attr.ngDisabled, updateAriaDisabled);
    } else {
      updateAriaDisabled(!!attr.disabled);
    }

    $mdAria.expect(element, 'aria-label', false);

    element.attr('tabIndex', 0);
    element.attr('role', 'slider');
    element.on('keydown', keydownListener);

    var hammertime = new Hammer(element[0], {
      recognizers: [
        [Hammer.Pan, { direction: Hammer.DIRECTION_HORIZONTAL }]
      ]
    });
    hammertime.on('hammer.input', onInput);
    hammertime.on('panstart', onPanStart);
    hammertime.on('pan', onPan);
    hammertime.on('panend', onPanEnd);

    // On resize, recalculate the slider's dimensions and re-render
    function updateAll() {
      refreshSliderDimensions();
      ngModelRender();
      redrawTicks();
    }
    setTimeout(updateAll);

    var debouncedUpdateAll = $$rAF.debounce(updateAll);
    angular.element($window).on('resize', debouncedUpdateAll);

    scope.$on('$destroy', function() {
      angular.element($window).off('resize', debouncedUpdateAll);
      hammertime.destroy();
      stopDisabledWatch();
    });

    ngModelCtrl.$render = ngModelRender;
    ngModelCtrl.$viewChangeListeners.push(ngModelRender);
    ngModelCtrl.$formatters.push(minMaxValidator);
    ngModelCtrl.$formatters.push(stepValidator);

    /**
     * Attributes
     */
    var min;
    var max;
    var step;
    function updateMin(value) {
      min = parseFloat(value);
      element.attr('aria-valuemin', value);
    }
    function updateMax(value) {
      max = parseFloat(value);
      element.attr('aria-valuemax', value);
    }
    function updateStep(value) {
      step = parseFloat(value);
      redrawTicks();
    }
    function updateAriaDisabled(isDisabled) {
      element.attr('aria-disabled', !!isDisabled);
    }

    // Draw the ticks with canvas.
    // The alternative to drawing ticks with canvas is to draw one element for each tick,
    // which could quickly become a performance bottleneck.
    var tickCanvas, tickCtx;
    function redrawTicks() {
      if (!angular.isDefined(attr.discrete)) return;

      var numSteps = Math.floor( (max - min) / step );
      if (!tickCanvas) {
        tickCanvas = angular.element('<canvas style="position:absolute;">');
        tickCtx = tickCanvas[0].getContext('2d');
        tickCtx.fillStyle = 'black';
        tickContainer.append(tickCanvas);
      }
      var dimensions = getSliderDimensions();
      tickCanvas[0].width = dimensions.width;
      tickCanvas[0].height = dimensions.height;

      var distance;
      for (var i = 0; i <= numSteps; i++) {
        distance = Math.floor(dimensions.width * (i / numSteps));
        tickCtx.fillRect(distance - 1, 0, 2, dimensions.height);
      }
    }


    /**
     * Refreshing Dimensions
     */
    var sliderDimensions = {};
    refreshSliderDimensions();
    function refreshSliderDimensions() {
      sliderDimensions = trackContainer[0].getBoundingClientRect();
    }
    function getSliderDimensions() {
      throttledRefreshDimensions();
      return sliderDimensions;
    }

    /**
     * left/right arrow listener
     */
    function keydownListener(ev) {
      if(element[0].hasAttribute('disabled')) {
        return;
      }

      var changeAmount;
      if (ev.which === $mdConstant.KEY_CODE.LEFT_ARROW) {
        changeAmount = -step;
      } else if (ev.which === $mdConstant.KEY_CODE.RIGHT_ARROW) {
        changeAmount = step;
      }
      if (changeAmount) {
        if (ev.metaKey || ev.ctrlKey || ev.altKey) {
          changeAmount *= 4;
        }
        ev.preventDefault();
        ev.stopPropagation();
        scope.$evalAsync(function() {
          setModelValue(ngModelCtrl.$viewValue + changeAmount);
        });
      }
    }

    /**
     * ngModel setters and validators
     */
    function setModelValue(value) {
      ngModelCtrl.$setViewValue( minMaxValidator(stepValidator(value)) );
    }
    function ngModelRender() {

      if (isNaN(ngModelCtrl.$viewValue)) {
        ngModelCtrl.$viewValue = ngModelCtrl.$modelValue;
      }

      var percent = (ngModelCtrl.$viewValue - min) / (max - min);
      scope.modelValue = ngModelCtrl.$viewValue;
      element.attr('aria-valuenow', ngModelCtrl.$viewValue);
      setSliderPercent(percent);
    }

    function minMaxValidator(value) {
      if (angular.isNumber(value)) {
        return Math.max(min, Math.min(max, value));
      }
    }
    function stepValidator(value) {
      if (angular.isNumber(value)) {
        return Math.round(value / step) * step;
      }
    }

    /**
     * @param percent 0-1
     */
    function setSliderPercent(percent) {
      activeTrack.css('width', (percent * 100) + '%');
      thumbContainer.css(
        $mdEffects.TRANSFORM,
        'translate3d(' + getSliderDimensions().width * percent + 'px,0,0)'
      );
      element.toggleClass('md-min', percent === 0);
    }


    /**
     * Slide listeners
     */
    var isSliding = false;
    var isDiscrete = angular.isDefined(attr.discrete);

    function onInput(ev) {
      if (!isSliding && ev.eventType === Hammer.INPUT_START &&
          !element[0].hasAttribute('disabled')) {

        isSliding = true;

        element.addClass('active');
        element[0].focus();
        refreshSliderDimensions();

        onPan(ev);

        ev.srcEvent.stopPropagation();

      } else if (isSliding && ev.eventType === Hammer.INPUT_END) {

        if ( isSliding && isDiscrete ) onPanEnd(ev);
        isSliding = false;

        element.removeClass('panning active');
      }
    }
    function onPanStart() {
      if (!isSliding) return;
      element.addClass('panning');
    }
    function onPan(ev) {
      if (!isSliding) return;

      // While panning discrete, update only the
      // visual positioning but not the model value.

      if ( isDiscrete ) adjustThumbPosition( ev.center.x );
      else              doSlide( ev.center.x );

      ev.preventDefault();
      ev.srcEvent.stopPropagation();
    }

    function onPanEnd(ev) {
      if ( isDiscrete && !element[0].hasAttribute('disabled') ) {
        // Convert exact to closest discrete value.
        // Slide animate the thumb... and then update the model value.

        var exactVal = percentToValue( positionToPercent( ev.center.x ));
        var closestVal = minMaxValidator( stepValidator(exactVal) );

        setSliderPercent( valueToPercent(closestVal));
        $$rAF(function(){
          setModelValue( closestVal );
        });

        ev.preventDefault();
        ev.srcEvent.stopPropagation();
      }
    }

    /**
     * Expose for testing
     */
    this._onInput = onInput;
    this._onPanStart = onPanStart;
    this._onPan = onPan;

    /**
     * Slide the UI by changing the model value
     * @param x
     */
    function doSlide( x ) {
      scope.$evalAsync( function() {
        setModelValue( percentToValue( positionToPercent(x) ));
      });
    }

    /**
     * Slide the UI without changing the model (while dragging/panning)
     * @param x
     */
    function adjustThumbPosition( x ) {
      setSliderPercent( positionToPercent(x) );
    }

    /**
     * Convert horizontal position on slider to percentage value of offset from beginning...
     * @param x
     * @returns {number}
     */
    function positionToPercent( x ) {
      return Math.max(0, Math.min(1, (x - sliderDimensions.left) / (sliderDimensions.width)));
    }

    /**
     * Convert percentage offset on slide to equivalent model value
     * @param percent
     * @returns {*}
     */
    function percentToValue( percent ) {
      return (min + percent * (max - min));
    }

    function valueToPercent( val ) {
      return (val - min)/(max - min);
    }

  };
}
})();

(function() {
/*
 * @ngdoc module
 * @name material.components.sticky
 * @description
 *
 * Sticky effects for md
 */

angular.module('material.components.sticky', [
  'material.core',
  'material.components.content',
  'material.decorators',
  'material.animations'
])
.factory('$mdSticky', [
  '$document',
  '$mdEffects',
  '$compile',
  '$$rAF',
  '$mdUtil',
  MdSticky
]);

/*
 * @ngdoc service
 * @name $mdSticky
 * @module material.components.sticky
 *
 * @description
 * The `$mdSticky`service provides a mixin to make elements sticky.
 *
 * @returns A `$mdSticky` function that takes three arguments:
 *   - `scope`
 *   - `element`: The element that will be 'sticky'
 *   - `elementClone`: A clone of the element, that will be shown
 *     when the user starts scrolling past the original element.
 *     If not provided, it will use the result of `element.clone()`.
 */

function MdSticky($document, $mdEffects, $compile, $$rAF, $mdUtil) {

  var browserStickySupport = checkStickySupport();

  /**
   * Registers an element as sticky, used internally by directives to register themselves
   */
  return function registerStickyElement(scope, element, stickyClone) {
    var contentCtrl = element.controller('mdContent');
    if (!contentCtrl) return;

    if (browserStickySupport) {
      element.css({
        position: browserStickySupport,
        top: 0,
        'z-index': 2
      });
    } else {
      var $$sticky = contentCtrl.$element.data('$$sticky');
      if (!$$sticky) {
        $$sticky = setupSticky(contentCtrl);
        contentCtrl.$element.data('$$sticky', $$sticky);
      }

      var deregister = $$sticky.add(element, stickyClone || element.clone());
      scope.$on('$destroy', deregister);
    }
  };

  function setupSticky(contentCtrl) {
    var contentEl = contentCtrl.$element;

    // Refresh elements is very expensive, so we use the debounced
    // version when possible.
    var debouncedRefreshElements = $$rAF.debounce(refreshElements);

    // setupAugmentedScrollEvents gives us `$scrollstart` and `$scroll`,
    // more reliable than `scroll` on android.
    setupAugmentedScrollEvents(contentEl);
    contentEl.on('$scrollstart', debouncedRefreshElements);
    contentEl.on('$scroll', onScroll);

    var self;
    return self = {
      prev: null,
      current: null, //the currently stickied item
      next: null,
      items: [],
      add: add,
      refreshElements: refreshElements
    };

    /***************
     * Public
     ***************/
    // Add an element and its sticky clone to this content's sticky collection
    function add(element, stickyClone) {
      stickyClone.addClass('md-sticky-clone');

      var item = {
        element: element,
        clone: stickyClone
      };
      self.items.push(item);

      contentEl.parent().prepend(item.clone);

      debouncedRefreshElements();

      return function remove() {
        self.items.forEach(function(item, index) {
          if (item.element[0] === element[0]) {
            self.items.splice(index, 1);
            item.clone.remove();
          }
        });
        debouncedRefreshElements();
      };
    }

    function refreshElements() {
      var contentRect = contentEl[0].getBoundingClientRect();


      // Sort our collection of elements by their current position in the DOM.
      // We need to do this because our elements' order of being added may not
      // be the same as their order of display.
      self.items.forEach(refreshPosition);
      self.items = self.items.sort(function(a, b) {
        return a.top < b.top ? -1 : 1;
      });

      // Find which item in the list should be active, 
      // based upon the content's current scroll position
      var item;
      var currentScrollTop = contentEl.prop('scrollTop');
      for (var i = self.items.length - 1; i >= 0; i--) {
        if (currentScrollTop > self.items[i].top) {
          item = self.items[i];
          break;
        }
      }
      setCurrentItem(item);
    }


    /***************
     * Private
     ***************/

    // Find the `top` of an item relative to the content element,
    // and also the height.
    function refreshPosition(item) {
      // Find the top of an item by adding to the offsetHeight until we reach the 
      // content element.
      var current = item.element[0];
      item.top = 0;
      item.left = 0;
      while (current && current !== contentEl[0]) {
        item.top += current.offsetTop;
        item.left += current.offsetLeft;
        current = current.offsetParent;
      }
      item.height = item.element.prop('offsetHeight');
      item.clone.css('margin-left', item.left + 'px');
    }


    // As we scroll, push in and select the correct sticky element.
    function onScroll() {
      var scrollTop = contentEl.prop('scrollTop');
      var isScrollingDown = scrollTop > (onScroll.prevScrollTop || 0);
      onScroll.prevScrollTop = scrollTop;

      // At the top?
      if (scrollTop === 0) {
        setCurrentItem(null);

      // Going to next item?
      } else if (isScrollingDown && self.next) {
        if (self.next.top - scrollTop <= 0) {
          // Sticky the next item if we've scrolled past its position.
          setCurrentItem(self.next);
        } else if (self.current) {
          // Push the current item up when we're almost at the next item.
          if (self.next.top - scrollTop <= self.next.height) {
            translate(self.current, self.next.top - self.next.height - scrollTop);
          } else {
            translate(self.current, null);
          }
        }
        
      // Scrolling up with a current sticky item?
      } else if (!isScrollingDown && self.current) {
        if (scrollTop < self.current.top) {
          // Sticky the previous item if we've scrolled up past
          // the original position of the currently stickied item.
          setCurrentItem(self.prev);
        }
        // Scrolling up, and just bumping into the item above (just set to current)?
        // If we have a next item bumping into the current item, translate
        // the current item up from the top as it scrolls into view.
        if (self.current && self.next) {
          if (scrollTop >= self.next.top - self.current.height) {
            translate(self.current, self.next.top - scrollTop - self.current.height);
          } else {
            translate(self.current, null);
          }
        }
      }
    }
     
   function setCurrentItem(item) {
     if (self.current === item) return;
     // Deactivate currently active item
     if (self.current) {
       translate(self.current, null);
       setStickyState(self.current, null);
     }

     // Activate new item if given
     if (item) {
       setStickyState(item, 'active');
     }

     self.current = item;
     var index = self.items.indexOf(item);
     // If index === -1, index + 1 = 0. It works out.
     self.next = self.items[index + 1];
     self.prev = self.items[index - 1];
     setStickyState(self.next, 'next');
     setStickyState(self.prev, 'prev');
   }

   function setStickyState(item, state) {
     if (!item || item.state === state) return;
     if (item.state) {
       item.clone.attr('sticky-prev-state', item.state);
       item.element.attr('sticky-prev-state', item.state);
     }
     item.clone.attr('sticky-state', state);
     item.element.attr('sticky-state', state);
     item.state = state;
   }

   function translate(item, amount) {
     if (!item) return;
     if (amount === null || amount === undefined) {
       if (item.translateY) {
         item.translateY = null;
         item.clone.css($mdEffects.TRANSFORM, '');
       }
     } else {
       item.translateY = amount;
       item.clone.css(
         $mdEffects.TRANSFORM, 
         'translate3d(' + item.left + 'px,' + amount + 'px,0)'
       );
     }
   }
  }

  // Function to check for browser sticky support
  function checkStickySupport($el) {
    var stickyProp;
    var testEl = angular.element('<div>');
    $document[0].body.appendChild(testEl[0]);

    var stickyProps = ['sticky', '-webkit-sticky'];
    for (var i = 0; i < stickyProps.length; ++i) {
      testEl.css({position: stickyProps[i], top: 0, 'z-index': 2});
      if (testEl.css('position') == stickyProps[i]) {
        stickyProp = stickyProps[i];
        break;
      }
    }
    testEl.remove();
    return stickyProp;
  }

  // Android 4.4 don't accurately give scroll events.
  // To fix this problem, we setup a fake scroll event. We say:
  // > If a scroll or touchmove event has happened in the last DELAY milliseconds, 
  //   then send a `$scroll` event every animationFrame.
  // Additionally, we add $scrollstart and $scrollend events.
  function setupAugmentedScrollEvents(element) {
    var SCROLL_END_DELAY = 200;
    var isScrolling;
    var lastScrollTime;
    element.on('scroll touchmove', function() {
      if (!isScrolling) {
        isScrolling = true;
        $$rAF(loopScrollEvent);
        element.triggerHandler('$scrollstart');
      }
      element.triggerHandler('$scroll');
      lastScrollTime = +$mdUtil.now();
    });

    function loopScrollEvent() {
      if (+$mdUtil.now() - lastScrollTime > SCROLL_END_DELAY) {
        isScrolling = false;
        element.triggerHandler('$scrollend');
      } else {
        element.triggerHandler('$scroll');
        $$rAF(loopScrollEvent);
      }
    }
  }

}
})();

(function() {
/**
 * @ngdoc module
 * @name material.components.subheader
 * @description
 * SubHeader module
 */
angular.module('material.components.subheader', [
  'material.components.sticky',
  'material.services.theming'
])
.directive('mdSubheader', [
  '$mdSticky',
  '$compile',
  '$mdTheming',
  MdSubheaderDirective
]);

/**
 * @ngdoc directive
 * @name mdSubheader
 * @module material.components.subheader
 *
 * @restrict E
 *
 * @description
 * The `<md-subheader>` directive is a subheader for a section
 *
 * @usage
 * <hljs lang="html">
 * <md-subheader>Online Friends</md-subheader>
 * </hljs>
 */

function MdSubheaderDirective($mdSticky, $compile, $mdTheming) {
  return {
    restrict: 'E',
    replace: true,
    transclude: true,
    template: 
      '<h2 class="md-subheader">' +
        '<span class="md-subheader-content"></span>' +
      '</h2>',
    compile: function(element, attr, transclude) {
      var outerHTML = element[0].outerHTML;
      return function postLink(scope, element, attr) {
        $mdTheming(element);
        function getContent(el) {
          return angular.element(el[0].querySelector('.md-subheader-content'));
        }

        // Transclude the user-given contents of the subheader
        // the conventional way.
        transclude(scope, function(clone) {
          getContent(element).append(clone);
        });

        // Create another clone, that uses the outer and inner contents
        // of the element, that will be 'stickied' as the user scrolls.
        transclude(scope, function(clone) {
          var stickyClone = $compile(angular.element(outerHTML))(scope);
          $mdTheming(stickyClone);
          getContent(stickyClone).append(clone);
          $mdSticky(scope, element, stickyClone);
        });
      };
    }
  };
}
})();

(function() {
(function() {

  /**
   * @ngdoc module
   * @name material.components.swipe
   * @description Swipe module!
   */
  angular.module('material.components.swipe',['ng'])

    /*
     * @ngdoc service
     * @module material.components.swipe
     * @name $mdSwipe
     * @description
     * This service allows directives to easily attach swipe and pan listeners to
     * the specified element.
     */
    .factory("$mdSwipe", function() {

      // match expected API functionality
      var attachNoop = function(){ return angular.noop; };

      /**
       * SwipeService constructor pre-captures scope and customized event types
       *
       * @param scope
       * @param eventTypes
       * @returns {*}
       * @constructor
       */
      return function SwipeService(scope, eventTypes) {
        if ( !eventTypes ) eventTypes = "swipeleft swiperight";

        // publish configureFor() method for specific element instance
        return function configureFor(element, onSwipeCallback, attachLater ) {
          var hammertime = new Hammer(element[0], {
            recognizers : addRecognizers([], eventTypes )
          });

          // Attach swipe listeners now
          if ( !attachLater ) attachSwipe();

          // auto-disconnect during destroy
          scope.$on('$destroy', function() {
            hammertime.destroy();
          });

          return attachSwipe;

          // **********************
          // Internal methods
          // **********************

          /**
           * Delegate swipe event to callback function
           * and ensure $digest is triggered.
           *
           * @param ev HammerEvent
           */
          function swipeHandler(ev) {

            // Prevent triggering parent hammer listeners
            ev.srcEvent.stopPropagation();

            if ( angular.isFunction(onSwipeCallback) ) {
              scope.$apply(function() {
                onSwipeCallback(ev);
              });
            }
          }

          /**
           * Enable listeners and return detach() fn
           */
          function attachSwipe() {
            hammertime.on(eventTypes, swipeHandler );

            return function detachSwipe() {
              hammertime.off( eventTypes );
            };
          }

          /**
           * Add optional recognizers such as panleft, panright
           */
          function addRecognizers(list, events) {
            var hasPanning = (events.indexOf("pan") > -1);
            var hasSwipe   = (events.indexOf("swipe") > -1);

            if (hasPanning) {
              list.push([ Hammer.Pan, { direction: Hammer.DIRECTION_HORIZONTAL } ]);
            }
            if (hasSwipe) {
              list.push([ Hammer.Swipe, { direction: Hammer.DIRECTION_HORIZONTAL } ]);
            }

            return list;
          }

        };
      };
    })

    /**
     * @ngdoc directive
     * @module material.components.swipe
     * @name mdSwipeLeft
     *
     * @restrict A
     *
     * @description
     * The `<div  md-swipe-left="expression">` directive identifies an element on which
     * HammerJS horizontal swipe left and pan left support will be active. The swipe/pan action
     * can result in custom activity trigger by evaluating `expression`.
     *
     * @param {boolean=} noPan Use of attribute indicates flag to disable detection of `panleft` activity
     *
     * @usage
     * <hljs lang="html">
     *
     * <div class="animate-switch-container"
     *      ng-switch on="data.selectedIndex"
     *      md-swipe-left="data.selectedIndex+=1;"
     *      md-swipe-right="data.selectedIndex-=1;" >
     *
     * </div>
     * </hljs>
     *
     */
    .directive("mdSwipeLeft", ['$parse', '$mdSwipe',
      function MdSwipeLeft($parse, $mdSwipe) {
        return {
          restrict: 'A',
          link :  swipePostLink( $parse, $mdSwipe, "SwipeLeft" )
        };
      }])

    /**
     * @ngdoc directive
     * @module material.components.swipe
     * @name mdSwipeRight
     *
     * @restrict A
     *
     * @description
     * The `<div  md-swipe-right="expression">` directive identifies functionality
     * that attaches HammerJS horizontal swipe right and pan right support to an element. The swipe/pan action
     * can result in activity trigger by evaluating `expression`
     *
     * @param {boolean=} noPan Use of attribute indicates flag to disable detection of `panright` activity
     *
     * @usage
     * <hljs lang="html">
     *
     * <div class="animate-switch-container"
     *      ng-switch on="data.selectedIndex"
     *      md-swipe-left="data.selectedIndex+=1;"
     *      md-swipe-right="data.selectedIndex-=1;" >
     *
     * </div>
     * </hljs>
     *
     */
    .directive( "mdSwipeRight", ['$parse', '$mdSwipe',
      function MdSwipeRight($parse, $mdSwipe) {
        return {
          restrict: 'A',
          link: swipePostLink( $parse, $mdSwipe, "SwipeRight" )
        };
      }
    ]);

    /**
     * Factory to build PostLink function specific to Swipe or Pan direction
     *
     * @param $parse
     * @param $mdSwipe
     * @param name
     * @returns {Function}
     */
    function swipePostLink($parse, $mdSwipe, name ) {

      return function(scope, element, attrs) {
        var direction = name.toLowerCase();
        var directiveName= "md" + name;

        var parentGetter = $parse(attrs[directiveName]) || angular.noop;
        var configureSwipe = $mdSwipe(scope, direction);
        var requestSwipe = function(locals) {
          // build function to request scope-specific swipe response
          parentGetter(scope, locals);
        };

        configureSwipe( element, function onHandleSwipe(ev) {
          if ( ev.type == direction ) {
            requestSwipe();
          }
        });

      }
    }

})();



})();

(function() {
/**
 * @private
 * @ngdoc module
 * @name material.components.switch
 */

angular.module('material.components.switch', [
  'material.components.checkbox',
  'material.components.radioButton',
  'material.services.theming'
])

.directive('mdSwitch', [
  'mdCheckboxDirective',
  'mdRadioButtonDirective',
  '$mdTheming',
  MdSwitch
]);

/**
 * @private
 * @ngdoc directive
 * @module material.components.switch
 * @name mdSwitch
 * @restrict E
 *
 * The switch directive is used very much like the normal [angular checkbox](https://docs.angularjs.org/api/ng/input/input%5Bcheckbox%5D).
 *
 * @param {string} ngModel Assignable angular expression to data-bind to.
 * @param {string=} name Property name of the form under which the control is published.
 * @param {expression=} ngTrueValue The value to which the expression should be set when selected.
 * @param {expression=} ngFalseValue The value to which the expression should be set when not selected.
 * @param {string=} ngChange Angular expression to be executed when input changes due to user interaction with the input element.
 * @param {boolean=} noink Use of attribute indicates use of ripple ink effects.
 * @param {boolean=} disabled Use of attribute indicates the switch is disabled: no ink effects and not selectable
 * @param {string=} ariaLabel Publish the button label used by screen-readers for accessibility. Defaults to the switch's text.
 *
 * @usage
 * <hljs lang="html">
 * <md-switch ng-model="isActive" aria-label="Finished?">
 *   Finished ?
 * </md-switch>
 *
 * <md-switch noink ng-model="hasInk" aria-label="No Ink Effects">
 *   No Ink Effects
 * </md-switch>
 *
 * <md-switch disabled ng-model="isDisabled" aria-label="Disabled">
 *   Disabled
 * </md-switch>
 *
 * </hljs>
 */
function MdSwitch(checkboxDirectives, radioButtonDirectives, $mdTheming) {
  var checkboxDirective = checkboxDirectives[0];
  var radioButtonDirective = radioButtonDirectives[0];

  return {
    restrict: 'E',
    transclude: true,
    template:
      '<div class="md-switch-bar"></div>' +
      '<div class="md-switch-thumb">' +
        radioButtonDirective.template +
      '</div>',
    require: '?ngModel',
    compile: compile
  };

  function compile(element, attr) {
    
    var thumb = angular.element(element[0].querySelector('.md-switch-thumb'));
    //Copy down disabled attributes for checkboxDirective to use
    thumb.attr('disabled', attr.disabled);
    thumb.attr('ngDisabled', attr.ngDisabled);

    var link = checkboxDirective.compile(thumb, attr);

    return function (scope, element, attr, ngModelCtrl) {
      $mdTheming(element);
      var thumb = angular.element(element[0].querySelector('.md-switch-thumb'));
      return link(scope, thumb, attr, ngModelCtrl);
    };
  }
}
})();

(function() {
/**
 * @ngdoc module
 * @name material.components.tabs
 * @description
 *
 * Tabs
 */
angular.module('material.components.tabs', [
  'material.core',
  'material.animations',
  'material.components.swipe',
  'material.services.theming'
]);
})();

(function() {
/**
 * @ngdoc module
 * @name material.components.textField
 * @description
 * Form
 */
angular.module('material.components.textField', ['material.core', 'material.services.theming'])
       .directive('mdInputGroup', [ mdInputGroupDirective ])
       .directive('mdInput', ['$mdUtil', mdInputDirective ])
       .directive('mdTextFloat', [ '$mdTheming', '$mdUtil', mdTextFloatDirective ]);



/**
 * @ngdoc directive
 * @name mdTextFloat
 * @module material.components.textField
 *
 * @restrict E
 *
 * @description
 * Use the `<md-text-float>` directive to quickly construct `Floating Label` text fields
 *
 * @param {string} fid Attribute used for accessibility link pairing between the Label and Input elements
 * @param {string=} type Optional value to define the type of input field. Defaults to string.
 * @param {string} label Attribute to specify the input text field hint.
 * @param {string=} ng-model Optional value to assign as existing input text string
 *
 * @usage
 * <hljs lang="html">
 * <md-text-float label="LastName" ng-model="user.lastName" > </md-text-float>
 *
 * <!-- Specify a read-only input field by using the `disabled` attribute -->
 * <md-text-float label="Company"  ng-model="user.company"    disabled > </md-text-float>
 *
 * <!-- Specify an input type if desired. -->
 * <md-text-float label="eMail"    ng-model="user.email" type="email" ></md-text-float>
 * </hljs>
 */
function mdTextFloatDirective($mdTheming, $mdUtil) {
  return {
    restrict: 'E',
    replace: true,
    scope : {
      fid : '@?',
      label : '@?',
      value : '=ngModel'
    },
    compile : function(element, attr) {

      if ( angular.isUndefined(attr.fid) ) {
        attr.fid = $mdUtil.nextUid();
      }

      return {
        pre : function(scope, element, attrs) {
          // transpose `disabled` flag
          if ( angular.isDefined(attrs.disabled) ) {
            element.attr('disabled', true);
            scope.isDisabled = true;
          }

          scope.inputType = attrs.type || "text";
          element.removeAttr('type');

          // transpose optional `class` settings
          element.attr('class', attrs.class );

        },
        post: $mdTheming
      };
    },
    template:
    '<md-input-group ng-disabled="isDisabled" tabindex="-1">' +
    ' <label for="{{fid}}" >{{label}}</label>' +
    ' <md-input id="{{fid}}" ng-model="value" type="{{inputType}}"></md-input>' +
    '</md-input-group>'
  };
}

/*
 * @private
 *
 * @ngdoc directive
 * @name mdInputGroup
 * @module material.components.textField
 * @restrict E
 * @description
 * Use the `<md-input-group>` directive as the grouping parent of a `<md-input>` element.
 *
 * @usage 
 * <hljs lang="html">
 * <md-input-group ng-disabled="isDisabled">
 *   <label for="{{fid}}">{{someLabel}}</label>
 *   <md-input id="{{fid}}" type="text" ng-model="someText"></md-input>
 * </md-input-group>
 * </hljs>
 */
function mdInputGroupDirective() {
  return {
    restrict: 'CE',
    controller: ['$element', function($element) {
      this.setFocused = function(isFocused) {
        $element.toggleClass('md-input-focused', !!isFocused);
      };
      this.setHasValue = function(hasValue) {
        $element.toggleClass('md-input-has-value', hasValue );
      };
    }]
  };

}

/*
 * @private
 *
 * @ngdoc directive
 * @name mdInput
 * @module material.components.textField
 *
 * @restrict E
 *
 * @description
 * Use the `<md-input>` directive as elements within a `<md-input-group>` container
 *
 * @usage
 * <hljs lang="html">
 * <md-input-group ng-disabled="user.isLocked">
 *   <label for="i1">FirstName</label>
 *   <md-input id="i1" ng-model="user.firstName"></md-input>
 * </md-input-group>
 * </hljs>
 */
function mdInputDirective($mdUtil) {
  return {
    restrict: 'E',
    replace: true,
    template: '<input >',
    require: ['^?mdInputGroup', '?ngModel'],
    link: function(scope, element, attr, ctrls) {
      var inputGroupCtrl = ctrls[0];
      var ngModelCtrl = ctrls[1];
      if (!inputGroupCtrl) {
        return;
      }

      // scan for disabled and transpose the `type` value to the <input> element
      var isDisabled = $mdUtil.isParentDisabled(element);

      element.attr('tabindex', isDisabled ? -1 : 0 );
      element.attr('aria-disabled', isDisabled ? 'true' : 'false');
      element.attr('type', attr.type || element.parent().attr('type') || "text" );

      // When the input value changes, check if it "has" a value, and
      // set the appropriate class on the input group
      if (ngModelCtrl) {
        //Add a $formatter so we don't use up the render function
        ngModelCtrl.$formatters.push(function(value) {
          inputGroupCtrl.setHasValue( isNotEmpty(value) );
          return value;
        });
      }

      element.on('input', function() {
        inputGroupCtrl.setHasValue( isNotEmpty() );
      });

      // When the input focuses, add the focused class to the group
      element.on('focus', function(e) {
        inputGroupCtrl.setFocused(true);
      });
      // When the input blurs, remove the focused class from the group
      element.on('blur', function(e) {
        inputGroupCtrl.setFocused(false);
        inputGroupCtrl.setHasValue( isNotEmpty() );
      });

      scope.$on('$destroy', function() {
        inputGroupCtrl.setFocused(false);
        inputGroupCtrl.setHasValue(false);
      });


      function isNotEmpty(value) {
        value = angular.isUndefined(value) ? element.val() : value;
        return (angular.isDefined(value) && (value!==null) &&
               (value.toString().trim() != ""));
      }
    }
  };
}




})();

(function() {
/**
 * @ngdoc module
 * @name material.components.toast
 * @description
 * Toast
 */
angular.module('material.components.toast', [
  'material.services.interimElement',
  'material.components.swipe'
])
  .directive('mdToast', [
    MdToastDirective
  ])
  .factory('$mdToast', [
    '$timeout',
    '$$interimElement',
    '$animate',
    '$mdSwipe',
    MdToastService
  ]);

function MdToastDirective() {
  return {
    restrict: 'E'
  };
}

/**
 * @ngdoc service
 * @name $mdToast
 * @module material.components.toast
 *
 * @description
 * `$mdToast` opens a toast nofication on any position on the screen with an optional
 * duration, and provides a simple promise API.
 *
 *
 * ### Restrictions
 * - The toast's template must have an outer `<md-toast>` element.
 * - For a toast action, use element with class `md-action`.
 * - Add the class `md-capsule` for curved corners.
 *
 * @usage
 * <hljs lang="html">
 * <div ng-controller="MyController">
 *   <md-button ng-click="openToast()">
 *     Open a Toast!
 *   </md-button>
 * </div>
 * </hljs>
 *
 * <hljs lang="js">
 * var app = angular.module('app', ['ngMaterial']);
 * app.controller('MyController', function($scope, $mdToast) {
 *   $scope.openToast = function($event) {
 *     $mdToast.show({
 *       template: '<md-toast>Hello!</md-toast>',
 *       hideDelay: 3000
 *     });
 *   };
 * });
 * </hljs>
 */

 /**
 * @ngdoc method
 * @name $mdToast#show
 *
 * @description
 * Show a toast dialog with the specified options.
 *
 * @param {object} options An options object, with the following properties:
 *
 *   - `templateUrl` - `{string=}`: The url of an html template file that will
 *     be used as the content of the toast. Restrictions: the template must
 *     have an outer `md-toast` element.
 *   - `template` - `{string=}`: Same as templateUrl, except this is an actual
 *     template string.
 *   - `hideDelay` - `{number=}`: How many milliseconds the toast should stay
 *     active before automatically closing.  Set to 0 or false to have the toast stay open until 
 *     closed manually. Default: 3000.
 *   - `position` - `{string=}`: Where to place the toast. Available: any combination
 *     of 'bottom', 'left', 'top', 'right', 'fit'. Default: 'bottom left'.
 *   - `controller` - `{string=}`: The controller to associate with this toast.
 *     The controller will be injected the local `$hideToast`, which is a function
 *     used to hide the toast.
 *   - `locals` - `{string=}`: An object containing key/value pairs. The keys will
 *     be used as names of values to inject into the controller. For example,
 *     `locals: {three: 3}` would inject `three` into the controller with the value
 *     of 3.
 *   - `resolve` - `{object=}`: Similar to locals, except it takes promises as values
 *     and the toast will not open until the promises resolve.
 *   - `controllerAs` - `{string=}`: An alias to assign the controller to on the scope.
 *
 * @returns {promise} A promise that can be resolved with `$mdToast.hide()` or
 * rejected with `$mdBottomSheet.cancel()`.
 */

/**
 * @ngdoc method
 * @name $mdToast#hide
 *
 * @description
 * Hide the existing toast and resolve the promise returned from `$mdToast.show()`.
 *
 * @param {*=} response An argument for the resolved promise.
 *
 */

/**
 * @ngdoc method
 * @name $mdToast#cancel
 *
 * @description
 * Hide the existing toast and reject the promise returned from 
 * `$mdToast.show()`.
 *
 * @param {*=} response An argument for the rejected promise.
 *
 */

function MdToastService($timeout, $$interimElement, $animate, $mdSwipe, $mdTheming) {

  var factoryDef = {
    onShow: onShow,
    onRemove: onRemove,
    position: 'bottom left',
    themable: true,
    hideDelay: 3000,
  };

  var $mdToast = $$interimElement(factoryDef);
  return $mdToast;

  function onShow(scope, element, options) {
    // 'top left' -> 'md-top md-left'
    element.addClass(options.position.split(' ').map(function(pos) {
      return 'md-' + pos;
    }).join(' '));
    options.parent.addClass(toastOpenClass(options.position));

    var configureSwipe = $mdSwipe(scope, 'swipeleft swiperight');
    options.detachSwipe = configureSwipe(element, function(ev) {
      //Add swipeleft/swiperight class to element so it can animate correctly
      element.addClass('md-' + ev.type);
      $timeout($mdToast.hide);
    });

    return $animate.enter(element, options.parent);
  }

  function onRemove(scope, element, options) {
    options.detachSwipe();
    options.parent.removeClass(toastOpenClass(options.position));
    return $animate.leave(element);
  }

  function toastOpenClass(position) {
    return 'md-toast-open-' +
      (position.indexOf('top') > -1 ? 'top' : 'bottom');
  }
}
})();

(function() {
/**
 * @ngdoc module
 * @name material.components.toolbar
 */
angular.module('material.components.toolbar', [
  'material.core',
  'material.components.content',
  'material.services.theming',
  'material.animations'
])
  .directive('mdToolbar', [
    '$$rAF',
    '$mdEffects',
    '$mdUtil',
    '$mdTheming',
    mdToolbarDirective
  ]);

/**
 * @ngdoc directive
 * @name mdToolbar
 * @module material.components.toolbar
 * @restrict E
 * @description
 * `md-toolbar` is used to place a toolbar in your app.
 *
 * Toolbars are usually used above a content area to display the title of the
 * current page, and show relevant action buttons for that page.
 *
 * You can change the height of the toolbar by adding either the
 * `md-medium-tall` or `md-tall` class to the toolbar.
 *
 * @usage
 * <hljs lang="html">
 * <div layout="vertical" layout-fill>
 *   <md-toolbar>
 *
 *     <div class="md-toolbar-tools">
 *       <span>My App's Title</span>
 *
 *       <!-- fill up the space between left and right area -->
 *       <span flex></span>
 *
 *       <md-button>
 *         Right Bar Button
 *       </md-button>
 *     </div>
 *
 *   </md-toolbar>
 *   <md-content>
 *     Hello!
 *   </md-content>
 * </div>
 * </hljs>
 *
 * @param {boolean=} scrollShrink Whether the header should shrink away as 
 * the user scrolls down, and reveal itself as the user scrolls up. 
 * Note: for scrollShrink to work, the toolbar must be a sibling of a 
 * `md-content` element, placed before it. See the scroll shrink demo.
 *
 *
 * @param {number=} shrinkSpeedFactor How much to change the speed of the toolbar's
 * shrinking by. For example, if 0.25 is given then the toolbar will shrink
 * at one fourth the rate at which the user scrolls down. Default 0.5.
 */ 
function mdToolbarDirective($$rAF, $mdEffects, $mdUtil, $mdTheming) {

  return {
    restrict: 'E',
    controller: angular.noop,
    link: function(scope, element, attr) {
      $mdTheming(element);

      if (angular.isDefined(attr.scrollShrink)) {
        setupScrollShrink();
      }

      function setupScrollShrink() {
        // Current "y" position of scroll
        var y = 0;
        // Store the last scroll top position
        var prevScrollTop = 0;

        var shrinkSpeedFactor = attr.shrinkSpeedFactor || 0.5;

        var toolbarHeight;
        var contentElement;

        var debouncedContentScroll = $$rAF.debounce(onContentScroll);
        var debouncedUpdateHeight = $mdUtil.debounce(updateToolbarHeight, 5 * 1000);

        // Wait for $mdContentLoaded event from mdContent directive.
        // If the mdContent element is a sibling of our toolbar, hook it up
        // to scroll events.
        scope.$on('$mdContentLoaded', onMdContentLoad);

        function onMdContentLoad($event, newContentEl) {
          if ($mdUtil.elementIsSibling(element, newContentEl)) {
            // unhook old content event listener if exists
            if (contentElement) {
              contentElement.off('scroll', debouncedContentScroll);
            }

            newContentEl.on('scroll', debouncedContentScroll);
            newContentEl.attr('scroll-shrink', 'true');

            contentElement = newContentEl;
            $$rAF(updateToolbarHeight);
          }
        }

        function updateToolbarHeight() {
          toolbarHeight = element.prop('offsetHeight');
          // Add a negative margin-top the size of the toolbar to the content el.
          // The content will start transformed down the toolbarHeight amount,
          // so everything looks normal.
          //
          // As the user scrolls down, the content will be transformed up slowly
          // to put the content underneath where the toolbar was.
          contentElement.css(
            'margin-top', 
            (-toolbarHeight * shrinkSpeedFactor) + 'px'
          );
          onContentScroll();
        }

        function onContentScroll(e) {
          var scrollTop = e ? e.target.scrollTop : prevScrollTop;

          debouncedUpdateHeight();

          y = Math.min(
            toolbarHeight / shrinkSpeedFactor, 
            Math.max(0, y + scrollTop - prevScrollTop)
          );

          element.css(
            $mdEffects.TRANSFORM, 
            'translate3d(0,' + (-y * shrinkSpeedFactor) + 'px,0)'
          );
          contentElement.css(
            $mdEffects.TRANSFORM, 
            'translate3d(0,' + ((toolbarHeight - y) * shrinkSpeedFactor) + 'px,0)'
          );

          prevScrollTop = scrollTop;
        }

      }

    }
  };

}
})();

(function() {
/**
 * @ngdoc module
 * @name material.components.tooltip
 */
angular.module('material.components.tooltip', [
  'material.core',
  'material.services.theming'
])

.directive('mdTooltip', [
  '$timeout',
  '$window',
  '$$rAF',
  '$document',
  '$mdUtil',
  '$mdTheming',
  MdTooltipDirective
]);

/**
 * @ngdoc directive
 * @name mdTooltip
 * @module material.components.tooltip
 * @description
 * Tooltips are used to describe elements that are interactive and primarily graphical (not textual).
 *
 * Place a `<md-tooltip>` as a child of the element it describes.
 *
 * A tooltip will activate when the user focuses, hovers over, or touches the parent.
 *
 * @usage
 * <hljs lang="html">
 * <md-icon icon="/img/icons/ic_play_arrow_24px.svg">
 *   <md-tooltip>
 *     Play Music
 *   </md-tooltip>
 * </md-icon>
 * </hljs>
 *
 * @param {expression=} visible Boolean bound to whether the tooltip is 
 * currently visible.
 */
function MdTooltipDirective($timeout, $window, $$rAF, $document, $mdUtil, $mdTheming) {

  var TOOLTIP_SHOW_DELAY = 400;
  var TOOLTIP_WINDOW_EDGE_SPACE = 8;
  // We have to append tooltips to the body, because we use
  // getBoundingClientRect() to find where to append the tooltip.
  var tooltipParent = angular.element(document.body);

  return {
    restrict: 'E',
    transclude: true,
    require: '^?mdContent',
    template: 
      '<div class="md-background"></div>' +
      '<div class="md-content" ng-transclude></div>',
    scope: {
      visible: '=?'
    },
    link: postLink
  };

  function postLink(scope, element, attr, contentCtrl) {
    $mdTheming(element);
    var parent = element.parent();

    // We will re-attach tooltip when visible
    element.detach();
    element.attr('role', 'tooltip');
    element.attr('id', attr.id || ('tooltip_' + $mdUtil.nextUid()));

    parent.on('focus mouseenter touchstart', function() {
      setVisible(true);
    });
    parent.on('blur mouseleave touchend touchcancel', function() {
      // Don't hide the tooltip if the parent is still focused.
      if ($document.activeElement === parent[0]) return;
      setVisible(false);
    });

    scope.$watch('visible', function(isVisible) {
      if (isVisible) showTooltip();
      else hideTooltip();
    });
    
    var debouncedOnResize = $$rAF.debounce(onWindowResize);
    angular.element($window).on('resize', debouncedOnResize);
    function onWindowResize() {
      // Reposition on resize
      if (scope.visible) positionTooltip();
    }

    // Be sure to completely cleanup the element on destroy
    scope.$on('$destroy', function() {
      scope.visible = false;
      element.remove();
      angular.element($window).off('resize', debouncedOnResize);
    });

    // *******
    // Methods
    // *******

    // If setting visible to true, debounce to TOOLTIP_SHOW_DELAY ms
    // If setting visible to false and no timeout is active, instantly hide the tooltip.
    function setVisible(value) {
      setVisible.value = !!value;

      if (!setVisible.queued) {
        if (value) {
          setVisible.queued = true;
          $timeout(function() {
            scope.visible = setVisible.value;
            setVisible.queued = false;
          }, TOOLTIP_SHOW_DELAY);

        } else {
          $timeout(function() { scope.visible = false; });
        }
      }
    }

    function showTooltip() {
      // Insert the element before positioning it, so we can get position
      // (tooltip is hidden by default)
      element.removeClass('md-hide');
      parent.attr('aria-describedby', element.attr('id'));
      tooltipParent.append(element);

      // Wait until the element has been in the dom for two frames before 
      // fading it in.
      // Additionally, we position the tooltip twice to avoid positioning bugs
      //positionTooltip();
      $$rAF(function() {

        $$rAF(function() {
          positionTooltip();
          if (!scope.visible) return;
          element.addClass('md-show');
        });

      });
    }

    function hideTooltip() {
      element.removeClass('md-show').addClass('md-hide');
      parent.removeAttr('aria-describedby');
      $timeout(function() {
        if (scope.visible) return;
        element.detach();
      }, 200, false);
    }

    function positionTooltip(rerun) {
      var tipRect = element[0].getBoundingClientRect();
      var parentRect = parent[0].getBoundingClientRect();

      if (contentCtrl) {
        parentRect.top += contentCtrl.$element.prop('scrollTop');
        parentRect.left += contentCtrl.$element.prop('scrollLeft');
      }

      // Default to bottom position if possible
      var tipDirection = 'bottom';
      var newPosition = {
        left: parentRect.left + parentRect.width / 2 - tipRect.width / 2,
        top: parentRect.top + parentRect.height
      };

      // If element bleeds over left/right of the window, place it on the edge of the window.
      newPosition.left = Math.min(
        newPosition.left, 
        $window.innerWidth - tipRect.width - TOOLTIP_WINDOW_EDGE_SPACE
      );
      newPosition.left = Math.max(newPosition.left, TOOLTIP_WINDOW_EDGE_SPACE);

      // If element bleeds over the bottom of the window, place it above the parent.
      if (newPosition.top + tipRect.height > $window.innerHeight) {
        newPosition.top = parentRect.top - tipRect.height;
        tipDirection = 'top';
      }

      element.css({top: newPosition.top + 'px', left: newPosition.left + 'px'});
      // Tell the CSS the size of this tooltip, as a multiple of 32.
      element.attr('width-32', Math.ceil(tipRect.width / 32));
      element.attr('md-direction', tipDirection);
    }

  }

}
})();

(function() {
/**
 * @ngdoc module
 * @name material.components.whiteframe
 */
angular.module('material.components.whiteframe', []);
})();

(function() {
angular.module('material.services.aria', [])

.service('$mdAria', [
  '$$rAF',
  '$log',
  AriaService
]);

function AriaService($$rAF, $log) {
  var messageTemplate = 'ARIA: Attribute "%s", required for accessibility, is missing on "%s"';
  var defaultValueTemplate = 'Default value was set: %s="%s".';

  return {
    expect : expectAttribute,
  };

  /**
   * Check if expected ARIA has been specified on the target element
   * @param element
   * @param attrName
   * @param copyElementText
   * @param {optional} defaultValue
   */
  function expectAttribute(element, attrName, copyElementText, defaultValue) {

    $$rAF(function(){

      var node = element[0];
      if (!node.hasAttribute(attrName)) {

        var hasDefault;
        if(copyElementText === true){
          if(!defaultValue) defaultValue = element.text().trim();
          hasDefault = angular.isDefined(defaultValue) && defaultValue.length;
        }

        if (hasDefault) {
          defaultValue = String(defaultValue).trim();
          element.attr(attrName, defaultValue);
        } else {
          $log.warn(messageTemplate, attrName, node);
          $log.warn(node);
        }
      }
    });
  }


  /**
   * Gets the tag definition from a node's outerHTML
   * @example getTagString(
   *   '<md-button foo="bar">Hello</md-button>'
   * ) // => '<md-button foo="bar">'
   */
  function getTagString(node) {
    var html = node.outerHTML;
    var closingIndex = html.indexOf('>');
    return html.substring(0, closingIndex + 1);
  }
}
})();

(function() {
angular.module('material.services.attrBind', [
])
  .factory('$attrBind', [
    '$parse', 
    '$interpolate', 
    MdAttrBind 
  ]);

/**
 *  This service allows directives to easily databind attributes to private scope properties.
 *
 * @private
 */
function MdAttrBind($parse, $interpolate) {
  var LOCAL_REGEXP = /^\s*([@=&])(\??)\s*(\w*)\s*$/;

  return function (scope, attrs, bindDefinition, bindDefaults) {
    angular.forEach(bindDefinition || {}, function (definition, scopeName) {
      //Adapted from angular.js $compile
      var match = definition.match(LOCAL_REGEXP) || [],
        attrName = match[3] || scopeName,
        mode = match[1], // @, =, or &
        parentGet,
        unWatchFn;

      switch (mode) {
        case '@':   // One-way binding from attribute into scope

          attrs.$observe(attrName, function (value) {
            scope[scopeName] = value;
          });
          attrs.$$observers[attrName].$$scope = scope;

          if (!bypassWithDefaults(attrName, scopeName)) {
            // we trigger an interpolation to ensure
            // the value is there for use immediately
            scope[scopeName] = $interpolate(attrs[attrName])(scope);
          }
          break;

        case '=':   // Two-way binding...

          if (!bypassWithDefaults(attrName, scopeName)) {
            // Immediate evaluation
            scope[scopeName] = (attrs[attrName] === "") ? true : scope.$eval(attrs[attrName]);

            // Data-bind attribute to scope (incoming) and
            // auto-release watcher when scope is destroyed

            unWatchFn = scope.$watch(attrs[attrName], function (value) {
              scope[scopeName] = value;
            });
            scope.$on('$destroy', unWatchFn);
          }

          break;

        case '&':   // execute an attribute-defined expression in the context of the parent scope

          if (!bypassWithDefaults(attrName, scopeName, angular.noop)) {
            /* jshint -W044 */
            if (attrs[attrName] && attrs[attrName].match(RegExp(scopeName + '\(.*?\)'))) {
              throw new Error('& expression binding "' + scopeName + '" looks like it will recursively call "' +
                attrs[attrName] + '" and cause a stack overflow! Please choose a different scopeName.');
            }

            parentGet = $parse(attrs[attrName]);
            scope[scopeName] = function (locals) {
              return parentGet(scope, locals);
            };
          }

          break;
      }
    });

    /**
     * Optional fallback value if attribute is not specified on element
     * @param scopeName
     */
    function bypassWithDefaults(attrName, scopeName, defaultVal) {
      if (!angular.isDefined(attrs[attrName])) {
        var hasDefault = bindDefaults && bindDefaults.hasOwnProperty(scopeName);
        scope[scopeName] = hasDefault ? bindDefaults[scopeName] : defaultVal;
        return true;
      }
      return false;
    }

  };
}
})();

(function() {
/*
 * @ngdoc module
 * @name material.services.compiler
 * @description compiler service
 */
angular.module('material.services.compiler', [
])
  .service('$mdCompiler', [
    '$q',
    '$http',
    '$injector',
    '$compile',
    '$controller',
    '$templateCache',
    mdCompilerService
  ]);

function mdCompilerService($q, $http, $injector, $compile, $controller, $templateCache) {

  /*
   * @ngdoc service
   * @name $mdCompiler
   * @module material.services.compiler
   * @description
   * The $mdCompiler service is an abstraction of angular's compiler, that allows the developer
   * to easily compile an element with a templateUrl, controller, and locals.
   *
   * @usage
   * <hljs lang="js">
   * $mdCompiler.compile({
   *   templateUrl: 'modal.html',
   *   controller: 'ModalCtrl',
   *   locals: {
   *     modal: myModalInstance;
   *   }
   * }).then(function(compileData) {
   *   compileData.element; // modal.html's template in an element
   *   compileData.link(myScope); //attach controller & scope to element
   * });
   * </hljs>
   */

   /*
    * @ngdoc method
    * @name $mdCompiler#compile
    * @description A helper to compile an HTML template/templateUrl with a given controller,
    * locals, and scope.
    * @param {object} options An options object, with the following properties:
    *
    *    - `controller` - `{(string=|function()=}` Controller fn that should be associated with
    *      newly created scope or the name of a registered controller if passed as a string.
    *    - `controllerAs` - `{string=}` A controller alias name. If present the controller will be
    *      published to scope under the `controllerAs` name.
    *    - `template` - `{string=}` An html template as a string.
    *    - `templateUrl` - `{string=}` A path to an html template.
    *    - `transformTemplate` - `{function(template)=}` A function which transforms the template after
    *      it is loaded. It will be given the template string as a parameter, and should
    *      return a a new string representing the transformed template.
    *    - `resolve` - `{Object.<string, function>=}` - An optional map of dependencies which should
    *      be injected into the controller. If any of these dependencies are promises, the compiler
    *      will wait for them all to be resolved, or if one is rejected before the controller is
    *      instantiated `compile()` will fail..
    *      * `key` - `{string}`: a name of a dependency to be injected into the controller.
    *      * `factory` - `{string|function}`: If `string` then it is an alias for a service.
    *        Otherwise if function, then it is injected and the return value is treated as the
    *        dependency. If the result is a promise, it is resolved before its value is 
    *        injected into the controller.
    *
    * @returns {object=} promise A promise, which will be resolved with a `compileData` object.
    * `compileData` has the following properties: 
    *
    *   - `element` - `{element}`: an uncompiled element matching the provided template.
    *   - `link` - `{function(scope)}`: A link function, which, when called, will compile
    *     the element and instantiate the provided controller (if given).
    *   - `locals` - `{object}`: The locals which will be passed into the controller once `link` is
    *     called.
    */
  this.compile = function(options) {
    var templateUrl = options.templateUrl;
    var template = options.template || '';
    var controller = options.controller;
    var controllerAs = options.controllerAs;
    var resolve = options.resolve || {};
    var locals = options.locals || {};
    var transformTemplate = options.transformTemplate || angular.identity;

    // Take resolve values and invoke them.  
    // Resolves can either be a string (value: 'MyRegisteredAngularConst'),
    // or an invokable 'factory' of sorts: (value: function ValueGetter($dependency) {})
    angular.forEach(resolve, function(value, key) {
      if (angular.isString(value)) {
        resolve[key] = $injector.get(value);
      } else {
        resolve[key] = $injector.invoke(value);
      }
    });
    //Add the locals, which are just straight values to inject
    //eg locals: { three: 3 }, will inject three into the controller
    angular.extend(resolve, locals);

    if (templateUrl) {
      resolve.$template = $http.get(templateUrl, {cache: $templateCache})
        .then(function(response) {
          return response.data;
        });
    } else {
      resolve.$template = $q.when(template);
    }

    // Wait for all the resolves to finish if they are promises
    return $q.all(resolve).then(function(locals) {

      var template = transformTemplate(locals.$template);
      var element = angular.element('<div>').html(template).contents();
      var linkFn = $compile(element);

      //Return a linking function that can be used later when the element is ready
      return {
        locals: locals,
        element: element,
        link: function link(scope) {
          locals.$scope = scope;

          //Instantiate controller if it exists, because we have scope
          if (controller) {
            var ctrl = $controller(controller, locals);
            //See angular-route source for this logic
            element.data('$ngControllerController', ctrl);
            element.children().data('$ngControllerController', ctrl);

            if (controllerAs) {
              scope[controllerAs] = ctrl;
            }
          }

          return linkFn(scope);
        }
      };
    });

  };
}
})();

(function() {
/*
 * @ngdoc module
 * @name material.services.interimElement
 * @description InterimElement
 */

angular.module('material.services.interimElement', [
  'material.services.compiler',
  'material.services.theming'
])
.factory('$$interimElement', [
  '$q',
  '$rootScope',
  '$timeout',
  '$rootElement',
  '$animate',
  '$mdCompiler',
  '$mdTheming',
  InterimElementFactory
]);

/*
 * @ngdoc service
 * @name $$interimElement
 *
 * @description
 *
 * Factory that contructs `$$interimElement.$service` services. 
 * Used internally in material design for elements that appear on screen temporarily.
 * The service provides a promise-like API for interacting with the temporary
 * elements.
 *
 * ```js
 * app.service('$mdToast', function($$interimElement) {
 *   var $mdToast = $$interimElement(toastDefaultOptions);
 *   return $mdToast;
 * });
 * ```
 * @param {object=} defaultOptions Options used by default for the `show` method on the service.
 *
 * @returns {$$interimElement.$service}
 *
 */

function InterimElementFactory($q, $rootScope, $timeout, $rootElement, $animate, $mdCompiler, $mdTheming) {

  return function createInterimElementService(defaults) {

    /*
     * @ngdoc service
     * @name $$interimElement.$service
     *
     * @description
     * A service used to control inserting and removing an element into the DOM.
     *
     */


    var stack = [];

    defaults = angular.extend({
      onShow: function(scope, $el, options) {
        return $animate.enter($el, options.parent);
      },
      onRemove: function(scope, $el, options) {
        return $animate.leave($el);
      },
    }, defaults || {});

    var service;
    return service = {
      show: show,
      hide: hide,
      cancel: cancel
    };

    /*
     * @ngdoc method
     * @name $$interimElement.$service#show
     * @kind function
     *
     * @description
     * Compiles and inserts an element into the DOM.
     *
     * @param {Object} options Options object to compile with.
     *
     * @returns {Promise} Promise that will resolve when the service
     * has `#close()` or `#cancel()` called.
     *
     */
    function show(options) {
      if (stack.length) {
        service.hide();
      }

      var interimElement = new InterimElement(options);
      stack.push(interimElement);
      return interimElement.show().then(function() {
        return interimElement.deferred.promise;
      });
    }

    /*
     * @ngdoc method
     * @name $$interimElement.$service#hide
     * @kind function
     *
     * @description
     * Removes the `$interimElement` from the DOM and resolves the promise returned from `show`
     *
     * @param {*} resolveParam Data to resolve the promise with
     *
     * @returns undefined data that resolves after the element has been removed.
     *
     */
    function hide(success) {
      var interimElement = stack.shift();
      interimElement && interimElement.remove().then(function() {
        interimElement.deferred.resolve(success);
      });
    }

    /*
     * @ngdoc method
     * @name $$interimElement.$service#cancel
     * @kind function
     *
     * @description
     * Removes the `$interimElement` from the DOM and rejects the promise returned from `show`
     *
     * @param {*} reason Data to reject the promise with
     *
     * @returns undefined
     *
     */
    function cancel(reason) {
      var interimElement = stack.shift();
      interimElement && interimElement.remove().then(function() {
        interimElement.deferred.reject(reason);
      });
    }


    /*
     * Internal Interim Element Object
     * Used internally to manage the DOM element and related data
     */
    function InterimElement(options) {
      var self;
      var hideTimeout, element;

      options = options || {};

      options = angular.extend({
        scope: options.scope || $rootScope.$new(options.isolateScope)
      }, defaults, options);

      return self = {
        options: options,
        deferred: $q.defer(),
        show: function() {
          return $mdCompiler.compile(options).then(function(compiledData) {
            // Search for parent at insertion time, if not specified
            if (!options.parent) {
              options.parent = $rootElement.find('body');
              if (!options.parent.length) options.parent = $rootElement;
            }
            element = compiledData.link(options.scope);
            if (options.themable) $mdTheming(element);
            var ret = options.onShow(options.scope, element, options);
            return $q.when(ret)
              .then(startHideTimeout);

            function startHideTimeout() {
              if (options.hideDelay) {
                hideTimeout = $timeout(service.hide, options.hideDelay) ;
              }
            }
          });
        },
        cancelTimeout: function() {
          if (hideTimeout) {
            $timeout.cancel(hideTimeout);
            hideTimeout = undefined;
          }
        },
        remove: function() {
          self.cancelTimeout();
          var ret = options.onRemove(options.scope, element, options);
          return $q.when(ret).then(function() {
            options.scope.$destroy();
          });
        }
      };
    }
  };
}

})();

(function() {
angular.module('material.services.media', [
  'material.core'
])

.factory('$mdMedia', [
  '$window',
  '$mdUtil',
  '$timeout',
  mdMediaFactory
]);

function mdMediaFactory($window, $mdUtil, $timeout) {
  var cache = $mdUtil.cacheFactory('$mdMedia', { capacity: 15 });
  var presets = {
    sm: '(min-width: 600px)',
    md: '(min-width: 960px)',
    lg: '(min-width: 1200px)'
  };

  angular.element($window).on('resize', updateAll);

  return $mdMedia;

  function $mdMedia(query) {
    query = validate(query);
    var result;
    if ( !angular.isDefined(result = cache.get(query)) ) {
      return add(query);
    }
    return result;
  }

  function validate(query) {
    return presets[query] || (
      query.charAt(0) != '(' ?  ('(' + query + ')') : query
    );
  }

  function add(query) {
    return cache.put(query, !!$window.matchMedia(query).matches);
  }
  
  function updateAll() {
    var keys = cache.keys();
    if (keys.length) {
      for (var i = 0, ii = keys.length; i < ii; i++) {
        cache.put(keys[i], !!$window.matchMedia(keys[i]).matches);
      }
      // trigger an $digest()
      $timeout(angular.noop);
    }
  }

}
})();

(function() {
/*
 * @ngdoc module
 * @name material.services.registry
 *
 * @description
 * A component registry system for accessing various component instances in an app.
 */
angular.module('material.services.registry', [
])
  .factory('$mdComponentRegistry', [
    '$log', 
    mdComponentRegistry 
  ]);

/*
 * @ngdoc service
 * @name $mdComponentRegistry
 * @module material.services.registry
 *
 * @description
 * $mdComponentRegistry enables the user to interact with multiple instances of
 * certain complex components in a running app.
 */
function mdComponentRegistry($log) {
  var instances = [];

  return {
    /**
     * Used to print an error when an instance for a handle isn't found.
     */
    notFoundError: function(handle) {
      $log.error('No instance found for handle', handle);
    },
    /**
     * Return all registered instances as an array.
     */
    getInstances: function() {
      return instances;
    },

    /**
     * Get a registered instance.
     * @param handle the String handle to look up for a registered instance.
     */
    get: function(handle) {
      var i, j, instance;
      for(i = 0, j = instances.length; i < j; i++) {
        instance = instances[i];
        if(instance.$$mdHandle === handle) {
          return instance;
        }
      }
      return null;
    },

    /**
     * Register an instance.
     * @param instance the instance to register
     * @param handle the handle to identify the instance under.
     */
    register: function(instance, handle) {
      instance.$$mdHandle = handle;
      instances.push(instance);

      return function deregister() {
        var index = instances.indexOf(instance);
        if (index !== -1) {
          instances.splice(index, 1);
        }
      };
    }
  }
}

})();

(function() {
/*
 * @ngdoc module
 * @name material.services.theming
 * @description InterimElement
 */

angular.module('material.services.theming', [
])
.directive('mdTheme', [
  '$interpolate',
  ThemingDirective
])
.directive('mdThemable', [
  '$mdTheming',
  ThemableDirective
])
.provider('$mdTheming', [
  Theming
]);

/*
 * @ngdoc provider
 * @name $mdTheming
 *
 * @description
 *
 * Provider that makes an element apply theming related classes to itself.
 *
 * ```js
 * app.directive('myFancyDirective', function($mdTheming) {
 *   return {
 *     restrict: 'e',
 *     link: function(scope, el, attrs) {
 *       $mdTheming(el);
 *     }
 *   };
 * });
 * ```
 * @param {el=} element to apply theming to
 *
 * @returns {$$interimElement.$service}
 *
 */

function Theming($injector) {
  var defaultTheme = 'default';
  return {
    setDefaultTheme: function(theme) {
      defaultTheme = theme;
    },
    $get: ['$rootElement', '$rootScope', ThemingService]
  };

  function ThemingService($rootElement, $rootScope) {
    applyTheme.inherit = function(el, parent) {
      var ctrl = parent.controller('mdTheme');

      if (angular.isDefined(el.attr('md-theme-watch'))) { 
        var deregisterWatch = $rootScope.$watch(function() { 
          return ctrl && ctrl.$mdTheme || defaultTheme; 
        }, changeTheme);
        el.on('$destroy', deregisterWatch);
      } else {
        var theme = ctrl && ctrl.$mdTheme || defaultTheme;
        changeTheme(theme);
      }

      function changeTheme(theme) {
        var oldTheme = el.data('$mdThemeName');
        if (oldTheme) el.removeClass('md-' + oldTheme +'-theme');
        el.addClass('md-' + theme + '-theme');
        el.data('$mdThemeName', theme);
      }
    };

    return applyTheme;

    function applyTheme(scope, el) {
      // Allow us to be invoked via a linking function signature.
      if (el === undefined) { 
        el = scope;
        scope = undefined;
      }
      if (scope === undefined) {
        scope = $rootScope;
      }
      applyTheme.inherit(el, el);
    }
  }
}

function ThemingDirective($interpolate) {
  return {
    priority: 100,
    link: {
      pre: function(scope, el, attrs) {
        var ctrl = {
          $setTheme: function(theme) {
            ctrl.$mdTheme = theme;
          }
        };
        el.data('$mdThemeController', ctrl);
        ctrl.$setTheme($interpolate(attrs.mdTheme)(scope));
        attrs.$observe('mdTheme', ctrl.$setTheme);
      }
    }
  };
}

function ThemableDirective($mdTheming) {
  return $mdTheming;
}
})();

(function() {
/**
 * Conditionally configure ink bar animations when the
 * tab selection changes. If `nobar` then do not show the
 * bar nor animate.
 */
angular.module('material.components.tabs')

.directive('mdTabsInkBar', [
  '$mdEffects',
  '$window',
  '$$rAF',
  '$timeout',
  MdTabInkDirective
]);

function MdTabInkDirective($mdEffects, $window, $$rAF, $timeout) {

  return {
    restrict: 'E',
    require: ['^?nobar', '^mdTabs'],
    link: postLink
  };

  function postLink(scope, element, attr, ctrls) {
    var nobar = ctrls[0];
    var tabsCtrl = ctrls[1];

    if (nobar) return;

    var debouncedUpdateBar = $$rAF.debounce(updateBar);

    scope.$watch(tabsCtrl.selected, updateBar);
    scope.$on('$mdTabsChanged', debouncedUpdateBar);
    scope.$on('$mdTabsPaginationChanged', debouncedUpdateBar);
    angular.element($window).on('resize', onWindowResize);

    function onWindowResize() {
      debouncedUpdateBar();
      $timeout(debouncedUpdateBar, 100, false);
    }

    scope.$on('$destroy', function() {
      angular.element($window).off('resize', onWindowResize);
    });

    function updateBar() {
      var selectedElement = tabsCtrl.selected() && tabsCtrl.selected().element;

      if (!selectedElement || tabsCtrl.count() < 2) {
        element.css({
          display : 'none',
          width : '0px'
        });
      } else {
        var width = selectedElement.prop('offsetWidth');
        var left = selectedElement.prop('offsetLeft') + (tabsCtrl.$$pagingOffset || 0);

        element.css({
          display : width > 0 ? 'block' : 'none',
          width: width + 'px'
        });
        element.css($mdEffects.TRANSFORM, 'translate3d(' + left + 'px,0,0)');
      }
    }

  }

}
})();

(function() {

angular.module('material.components.tabs')

.directive('mdTabsPagination', [
  '$mdEffects',
  '$window',
  '$$rAF',
  '$$q',
  '$timeout',
  TabPaginationDirective
]);

function TabPaginationDirective($mdEffects, $window, $$rAF, $$q, $timeout) {

  // TODO allow configuration of TAB_MIN_WIDTH
  // Must match tab min-width rule in _tabs.scss
  var TAB_MIN_WIDTH = 8 * 12; 
  // Must match (2 * width of paginators) in scss
  var PAGINATORS_WIDTH = (8 * 4) * 2;

  return {
    restrict: 'A',
    require: '^mdTabs',
    link: postLink
  };

  function postLink(scope, element, attr, tabsCtrl) {

    var tabsParent = element.children();
    var state = scope.pagination = {
      page: -1,
      active: false,
      clickNext: function() { userChangePage(+1); },
      clickPrevious: function() { userChangePage(-1); }
    };

    updatePagination();
    var debouncedUpdatePagination = $$rAF.debounce(updatePagination);

    scope.$on('$mdTabsChanged', debouncedUpdatePagination);
    angular.element($window).on('resize', debouncedUpdatePagination);

    // Listen to focus events bubbling up from md-tab elements
    tabsParent.on('focusin', onTabsFocusIn);

    scope.$on('$destroy', function() {
      angular.element($window).off('resize', debouncedUpdatePagination);
      tabsParent.off('focusin', onTabsFocusIn);
    });

    scope.$watch(tabsCtrl.selected, onSelectedTabChange);

    // Allows pagination through focus change.
    function onTabsFocusIn(ev) {
      if (!state.active) return;

      var tab = angular.element(ev.target).controller('mdTab');
      var pageIndex = getPageForTab(tab);
      if (pageIndex !== state.page) {
        // If the focused element is on a new page, don't focus yet.
        tab.element.blur();
        // Go to the new page, wait for the page transition to end, then focus.
        setPage(pageIndex).then(function() {
          tab.element.focus();
        });
      }
    }

    function onSelectedTabChange(selectedTab) {
      if (!selectedTab) return;

      if (state.active) {
        var selectedTabPage = getPageForTab(selectedTab);
        setPage(selectedTabPage);
      } else {
        debouncedUpdatePagination();
      }
    }

    // Called when page is changed by a user action (click)
    function userChangePage(increment) {
      var newPage = state.page + increment;
      var newTab;
      if (!tabsCtrl.selected() || getPageForTab(tabsCtrl.selected()) !== newPage) {
        var startIndex;
        if (increment < 0) {
          // If going backward, select the previous available tab, starting from
          // the first item on the page after newPage.
          startIndex = (newPage + 1) * state.itemsPerPage;
          newTab = tabsCtrl.previous( tabsCtrl.itemAt(startIndex) );
        } else {
          // If going forward, select the next available tab, starting with the
          // last item before newPage.
          startIndex = (newPage * state.itemsPerPage) - 1;
          newTab = tabsCtrl.next( tabsCtrl.itemAt(startIndex) );
        }
      }
      setPage(newPage).then(function() {
        newTab && newTab.element.focus();
      });
      newTab && tabsCtrl.select(newTab);
    }

    function updatePagination() {
      var tabs = element.find('md-tab');
      var tabsWidth = element.parent().prop('clientWidth') - PAGINATORS_WIDTH;

      var needPagination = tabsWidth && TAB_MIN_WIDTH * tabsCtrl.count() > tabsWidth;
      var paginationToggled = needPagination !== state.active;

      state.active = needPagination;

      if (needPagination) {

        state.pagesCount = Math.ceil((TAB_MIN_WIDTH * tabsCtrl.count()) / tabsWidth);
        state.itemsPerPage = Math.max(1, Math.floor(tabsCtrl.count() / state.pagesCount));
        state.tabWidth = tabsWidth / state.itemsPerPage;
        
        tabsParent.css('width', state.tabWidth * tabsCtrl.count() + 'px');
        tabs.css('width', state.tabWidth + 'px');

        var selectedTabPage = getPageForTab(tabsCtrl.selected());
        setPage(selectedTabPage);

      } else {

        if (paginationToggled) {
          $timeout(function() {
            tabsParent.css('width', '');
            tabs.css('width', '');
            slideTabButtons(0);
            state.page = -1;
          });
        }

      }
    }

    function slideTabButtons(x) {
      if (tabsCtrl.pagingOffset === x) {
        // Resolve instantly if no change
        return $$q.when();
      }

      var deferred = $$q.defer();

      tabsCtrl.$$pagingOffset = x;
      tabsParent.css($mdEffects.TRANSFORM, 'translate3d(' + x + 'px,0,0)');
      tabsParent.on($mdEffects.TRANSITIONEND_EVENT, onTabsParentTransitionEnd);

      return deferred.promise;

      function onTabsParentTransitionEnd(ev) {
        // Make sure this event didn't bubble up from an animation in a child element.
        if (ev.target === tabsParent[0]) {
          tabsParent.off($mdEffects.TRANSITIONEND_EVENT, onTabsParentTransitionEnd);
          deferred.resolve();
        }
      }
    }

    function getPageForTab(tab) {
      var tabIndex = tabsCtrl.indexOf(tab);
      if (tabIndex === -1) return 0;

      return Math.floor(tabIndex / state.itemsPerPage);
    }

    function setPage(page) {
      if (page === state.page) return;

      var lastPage = state.pagesCount;

      if (page < 0) page = 0;
      if (page > lastPage) page = lastPage;

      state.hasPrev = page > 0;
      state.hasNext = ((page + 1) * state.itemsPerPage) < tabsCtrl.count();

      state.page = page;

      $timeout(function() {
        scope.$broadcast('$mdTabsPaginationChanged');
      });

      return slideTabButtons(-page * state.itemsPerPage * state.tabWidth);
    }
  }

}
})();

(function() {

angular.module('material.components.tabs')

.controller('$mdTab', [
  '$scope',
  '$element',
  '$compile',
  '$animate',
  '$mdSwipe',
  '$mdUtil',
  TabItemController
]);

function TabItemController(scope, element, $compile, $animate, $mdSwipe, $mdUtil) {
  var self = this;

  var detachSwipe = angular.noop;
  var attachSwipe = function() { return detachSwipe; };
  var eventTypes = "swipeleft swiperight" ;
  var configureSwipe = $mdSwipe( scope, eventTypes );

  // special callback assigned by TabsController
  self.$$onSwipe = angular.noop;

  // Properties
  self.contentContainer = angular.element('<div class="md-tab-content ng-hide">');
  self.element = element;

  // Methods
  self.isDisabled = isDisabled;
  self.onAdd = onAdd;
  self.onRemove = onRemove;
  self.onSelect = onSelect;
  self.onDeselect = onDeselect;


  function isDisabled() {
    return element[0].hasAttribute('disabled');
  }
  
  /**
   * Add the tab's content to the DOM container area in the tabs,
   * @param contentArea the contentArea to add the content of the tab to
   */
  function onAdd(contentArea) {
    if (self.content.length) {

      self.contentContainer.append(self.content);
      self.contentScope = scope.$parent.$new();
      contentArea.append(self.contentContainer);

      $compile(self.contentContainer)(self.contentScope);

      $mdUtil.disconnectScope(self.contentScope);

      // For internal tab views we only use the `$mdSwipe`
      // so we can easily attach()/detach() when the tab view is active/inactive

      attachSwipe = configureSwipe( self.contentContainer, function(ev) {
        self.$$onSwipe(ev.type);
      }, true );
    }
  }


  /**
   * Usually called when a Tab is programmatically removed; such
   * as in an ng-repeat
   */
  function onRemove() {
    $animate.leave(self.contentContainer).then(function() {
      self.contentScope && self.contentScope.$destroy();
      self.contentScope = null;
    });
  }

  function onSelect() {
    // Resume watchers and events firing when tab is selected
    $mdUtil.reconnectScope(self.contentScope);
    detachSwipe = attachSwipe();

    element.addClass('active');
    element.attr('aria-selected', true);
    element.attr('tabIndex', 0);
    $animate.removeClass(self.contentContainer, 'ng-hide');

    scope.onSelect();
  }

  function onDeselect() {
    // Stop watchers & events from firing while tab is deselected
    $mdUtil.disconnectScope(self.contentScope);
    detachSwipe();

    element.removeClass('active');
    element.attr('aria-selected', false);
    // Only allow tabbing to the active tab
    element.attr('tabIndex', -1);
    $animate.addClass(self.contentContainer, 'ng-hide');

    scope.onDeselect();
  }

}

})();

(function() {
angular.module('material.components.tabs')

.directive('mdTab', [
  '$mdInkRipple',
  '$compile',
  '$mdAria',
  '$mdUtil',
  '$mdConstant',
  MdTabDirective
]);

/**
 * @ngdoc directive
 * @name mdTab
 * @module material.components.tabs
 *
 * @restrict E
 *
 * @description
 * `<md-tab>` is the nested directive used [within `<md-tabs>`] to specify each tab with a **label** and optional *view content*.
 *
 * If the `label` attribute is not specified, then an optional `<md-tab-label>` tag can be used to specified more
 * complex tab header markup. If neither the **label** nor the **md-tab-label** are specified, then the nested
 * markup of the `<md-tab>` is used as the tab header markup.
 *
 * If a tab **label** has been identified, then any **non-**`<md-tab-label>` markup
 * will be considered tab content and will be transcluded to the internal `<div class="md-tabs-content">` container.
 *
 * This container is used by the TabsController to show/hide the active tab's content view. This synchronization is
 * automatically managed by the internal TabsController whenever the tab selection changes. Selection changes can
 * be initiated via data binding changes, programmatic invocation, or user gestures.
 *
 * @param {string=} label Optional attribute to specify a simple string as the tab label
 * @param {boolean=} active When evaluteing to true, selects the tab.
 * @param {boolean=} disabled If present, disabled tab selection.
 * @param {expression=} deselected Expression to be evaluated after the tab has been de-selected.
 * @param {expression=} selected Expression to be evaluated after the tab has been selected.
 *
 *
 * @usage
 *
 * <hljs lang="html">
 * <md-tab label="" disabled="" selected="" deselected="" >
 *   <h3>My Tab content</h3>
 * </md-tab>
 *
 * <md-tab >
 *   <md-tab-label>
 *     <h3>My Tab content</h3>
 *   </md-tab-label>
 *   <p>
 *     Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium,
 *     totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae
 *     dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit,
 *     sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt.
 *   </p>
 * </md-tab>
 * </hljs>
 *
 */
function MdTabDirective($mdInkRipple, $compile, $mdAria, $mdUtil, $mdConstant) {
  return {
    restrict: 'E',
    require: ['mdTab', '^mdTabs'],
    controller: '$mdTab',
    scope: {
      onSelect: '&',
      onDeselect: '&',
      label: '@'
    },
    compile: compile
  };

  function compile(element, attr) {
    var tabLabel = element.find('md-tab-label');

    if (tabLabel.length) {

      // If a tab label element is found, remove it for later re-use.
      tabLabel.remove();

    } else if (angular.isDefined(attr.label)) {

      // Otherwise, try to use attr.label as the label
      tabLabel = angular.element('<md-tab-label>').html(attr.label);

    } else {

      // If nothing is found, use the tab's content as the label
      tabLabel = angular.element('<md-tab-label>')
                        .append(element.contents().remove());
    }

    // Everything that's left as a child is the tab's content.
    var tabContent = element.contents().remove();

    return function postLink(scope, element, attr, ctrls) {

      var tabItemCtrl = ctrls[0]; // Controller for THIS tabItemCtrl
      var tabsCtrl = ctrls[1]; // Controller for ALL tabs

      transcludeTabContent();

      var detachRippleFn = $mdInkRipple.attachButtonBehavior(element);
      tabsCtrl.add(tabItemCtrl);
      scope.$on('$destroy', function() {
        detachRippleFn();
        tabsCtrl.remove(tabItemCtrl);
      });

      if (!angular.isDefined(attr.ngClick)) element.on('click', defaultClickListener);
      element.on('keydown', keydownListener);

      if (angular.isNumber(scope.$parent.$index)) watchNgRepeatIndex();
      if (angular.isDefined(attr.active)) watchActiveAttribute();
      watchDisabled();

      configureAria();

      function transcludeTabContent() {
        // Clone the label we found earlier, and $compile and append it
        var label = tabLabel.clone();
        element.append(label);
        $compile(label)(scope.$parent);

        // Clone the content we found earlier, and mark it for later placement into
        // the proper content area.
        tabItemCtrl.content = tabContent.clone();
      }

      //defaultClickListener isn't applied if the user provides an ngClick expression.
      function defaultClickListener() {
        scope.$apply(function() {
          tabsCtrl.select(tabItemCtrl);
          tabItemCtrl.element.focus();
        });
      }
      function keydownListener(ev) {
        if (ev.which == $mdConstant.KEY_CODE.SPACE ) {
          // Fire the click handler to do normal selection if space is pressed
          element.triggerHandler('click');
          ev.preventDefault();

        } else if (ev.which === $mdConstant.KEY_CODE.LEFT_ARROW) {
          var previous = tabsCtrl.previous(tabItemCtrl);
          previous && previous.element.focus();

        } else if (ev.which === $mdConstant.KEY_CODE.RIGHT_ARROW) {
          var next = tabsCtrl.next(tabItemCtrl);
          next && next.element.focus();
        }
      }

      // If tabItemCtrl is part of an ngRepeat, move the tabItemCtrl in our internal array
      // when its $index changes
      function watchNgRepeatIndex() {
        // The tabItemCtrl has an isolate scope, so we watch the $index on the parent.
        scope.$watch('$parent.$index', function $indexWatchAction(newIndex) {
          tabsCtrl.move(tabItemCtrl, newIndex);
        });
      }

      function watchActiveAttribute() {
        var unwatch = scope.$parent.$watch('!!(' + attr.active + ')', activeWatchAction);
        scope.$on('$destroy', unwatch);
        
        function activeWatchAction(isActive) {
          var isSelected = tabsCtrl.selected() === tabItemCtrl;

          if (isActive && !isSelected) {
            tabsCtrl.select(tabItemCtrl);
          } else if (!isActive && isSelected) {
            tabsCtrl.deselect(tabItemCtrl);
          }
        }
      }

      function watchDisabled() {
        scope.$watch(tabItemCtrl.isDisabled, disabledWatchAction);
        
        function disabledWatchAction(isDisabled) {
          element.attr('aria-disabled', isDisabled);

          // Auto select `next` tab when disabled
          var isSelected = (tabsCtrl.selected() === tabItemCtrl);
          if (isSelected && isDisabled) {
            tabsCtrl.select(tabsCtrl.next() || tabsCtrl.previous());
          }

        }
      }

      function configureAria() {
        // Link together the content area and tabItemCtrl with an id
        var tabId = attr.id || $mdUtil.nextUid();
        var tabContentId = 'content_' + tabId;
        element.attr({
          id: tabId,
          role: 'tabItemCtrl',
          tabIndex: '-1', //this is also set on select/deselect in tabItemCtrl
          'aria-controls': tabContentId
        });
        tabItemCtrl.contentContainer.attr({
          id: tabContentId,
          role: 'tabpanel',
          'aria-labelledby': tabId
        });

        $mdAria.expect(element, 'aria-label', true);
      }

    };

  }

}

})();

(function() {
angular.module('material.components.tabs')

.controller('$mdTabs', [
  '$scope', 
  '$element',
  '$mdUtil',
  MdTabsController
]);

function MdTabsController(scope, element, $mdUtil) {

  var tabsList = $mdUtil.iterator([], false);
  var self = this;

  // Properties
  self.element = element;
  // The section containing the tab content elements
  self.contentArea = angular.element(element[0].querySelector('.md-tabs-content'));

  // Methods from iterator
  self.inRange = tabsList.inRange;
  self.indexOf = tabsList.indexOf;
  self.itemAt = tabsList.itemAt;
  self.count = tabsList.count;
  
  self.selected = selected;
  self.add = add;
  self.remove = remove;
  self.move = move;
  self.select = select;
  self.deselect = deselect;

  self.next = next;
  self.previous = previous;

  self.swipe = swipe;

  scope.$on('$destroy', function() {
    self.deselect(self.selected());
    for (var i = tabsList.count() - 1; i >= 0; i--) {
      self.remove(tabsList[i], true);
    }
  });

  // Get the selected tab
  function selected() {
    return self.itemAt(scope.selectedIndex);
  }

  // Add a new tab.
  // Returns a method to remove the tab from the list.
  function add(tab, index) {

    tabsList.add(tab, index);
    tab.onAdd(self.contentArea);

    // Register swipe feature
    tab.$$onSwipe = swipe;

    // Select the new tab if we don't have a selectedIndex, or if the
    // selectedIndex we've been waiting for is this tab
    if (scope.selectedIndex === -1 || scope.selectedIndex === self.indexOf(tab)) {
      self.select(tab);
    }
    scope.$broadcast('$mdTabsChanged');
  }

  function remove(tab, noReselect) {
    if (!tabsList.contains(tab)) return;

    if (noReselect) {

    } else if (self.selected() === tab) {
      if (tabsList.count() > 1) {
        self.select(self.previous() || self.next());
      } else {
        self.deselect(tab);
      }
    }

    tabsList.remove(tab);
    tab.onRemove();

    scope.$broadcast('$mdTabsChanged');
  }

  // Move a tab (used when ng-repeat order changes)
  function move(tab, toIndex) {
    var isSelected = self.selected() === tab;

    tabsList.remove(tab);
    tabsList.add(tab, toIndex);
    if (isSelected) self.select(tab);

    scope.$broadcast('$mdTabsChanged');
  }

  function select(tab) {
    if (!tab || tab.isSelected || tab.isDisabled()) return;
    if (!tabsList.contains(tab)) return;

    self.deselect(self.selected());

    scope.selectedIndex = self.indexOf(tab);
    tab.isSelected = true;
    tab.onSelect();
  }
  function deselect(tab) {
    if (!tab || !tab.isSelected) return;
    if (!tabsList.contains(tab)) return;

    scope.selectedIndex = -1;
    tab.isSelected = false;
    tab.onDeselect();
  }

  function next(tab, filterFn) {
    return tabsList.next(tab || self.selected(), filterFn || isTabEnabled);
  }
  function previous(tab, filterFn) {
    return tabsList.previous(tab || self.selected(), filterFn || isTabEnabled);
  }

  function isTabEnabled(tab) {
    return tab && !tab.isDisabled();
  }

  /*
   * attach a swipe listen
   * if it's not selected, abort
   * check the direction
   *   if it is right
   *   it pan right
   *     Now select
   */

  function swipe(direction) {
    if ( !self.selected() ) return;

    // check the direction
    switch(direction) {

      case "swiperight":  // if it is right
      case "panright"  :  // it pan right
        // Now do this...
        self.select( self.previous() );
        break;

      case "swipeleft":
      case "panleft"  :
        self.select( self.next() );
        break;
    }

  }

}
})();

(function() {
angular.module('material.components.tabs')

/**
 * @ngdoc directive
 * @name mdTabs
 * @module material.components.tabs
 *
 * @restrict E
 *
 * @description
 * The `<md-tabs>` directive serves as the container for 1..n `<md-tab>` child directives to produces a Tabs components.
 * In turn, the nested `<md-tab>` directive is used to specify a tab label for the **header button** and a [optional] tab view
 * content that will be associated with each tab button.
 *
 * Below is the markup for its simplest usage:
 *
 *  <hljs lang="html">
 *  <md-tabs>
 *    <md-tab label="Tab #1"></md-tab>
 *    <md-tab label="Tab #2"></md-tab>
 *    <md-tab label="Tab #3"></md-tab>
 *  <md-tabs>
 *  </hljs>
 *
 * Tabs supports three (3) usage scenarios:
 *
 *  1. Tabs (buttons only)
 *  2. Tabs with internal view content
 *  3. Tabs with external view content
 *
 * **Tab-only** support is useful when tab buttons are used for custom navigation regardless of any other components, content, or views.
 * **Tabs with internal views** are the traditional usages where each tab has associated view content and the view switching is managed internally by the Tabs component.
 * **Tabs with external view content** is often useful when content associated with each tab is independently managed and data-binding notifications announce tab selection changes.
 *
 * > As a performance bonus, if the tab content is managed internally then the non-active (non-visible) tab contents are temporarily disconnected from the `$scope.$digest()` processes; which restricts and optimizes DOM updates to only the currently active tab.
 *
 * Additional features also include:
 *
 * *  Content can include any markup.
 * *  If a tab is disabled while active/selected, then the next tab will be auto-selected.
 * *  If the currently active tab is the last tab, then next() action will select the first tab.
 * *  Any markup (other than **`<md-tab>`** tags) will be transcluded into the tab header area BEFORE the tab buttons.
 *
 * @param {integer=} selected Index of the active/selected tab
 * @param {boolean=} noink If present, disables ink ripple effects.
 * @param {boolean=} nobar If present, disables the selection ink bar.
 * @param {string=}  align-tabs Attribute to indicate position of tab buttons: bottom or top; default is `top`
 *
 * @usage
 * <hljs lang="html">
 * <md-tabs selected="selectedIndex" >
 *   <img ng-src="/img/angular.png" class="centered">
 *
 *   <md-tab
 *      ng-repeat="tab in tabs | orderBy:predicate:reversed"
 *      on-select="onTabSelected(tab)"
 *      on-deselect="announceDeselected(tab)"
 *      disabled="tab.disabled" >
 *
 *       <md-tab-label>
 *           {{tab.title}}
 *           <img src="/img/removeTab.png"
 *                ng-click="removeTab(tab)"
 *                class="delete" >
 *       </md-tab-label>
 *
 *       {{tab.content}}
 *
 *   </md-tab>
 *
 * </md-tabs>
 * </hljs>
 *
 */
.directive('mdTabs', [
  '$parse',
  '$mdTheming',
  TabsDirective
]);

function TabsDirective($parse, $mdTheming) {
  return {
    restrict: 'E',
    controller: '$mdTabs',
    require: 'mdTabs',
    transclude: true,
    scope: {
      selectedIndex: '=?selected'
    },
    template:
      '<section class="md-header" ' +
        'ng-class="{\'md-paginating\': pagination.active}">' +

        '<div class="md-paginator md-prev" ' +
          'ng-if="pagination.active && pagination.hasPrev" ' +
          'ng-click="pagination.clickPrevious()">' +
        '</div>' +

        // overflow: hidden container when paginating
        '<div class="md-header-items-container" md-tabs-pagination>' +
          // flex container for <md-tab> elements
          '<div class="md-header-items" ng-transclude></div>' +
          '<md-tabs-ink-bar></md-tabs-ink-bar>' +
        '</div>' +

        '<div class="md-paginator md-next" ' +
          'ng-if="pagination.active && pagination.hasNext" ' +
          'ng-click="pagination.clickNext()">' +
        '</div>' +

      '</section>' +
      '<section class="md-tabs-content"></section>',
    link: postLink
  };

  function postLink(scope, element, attr, tabsCtrl) {
    $mdTheming(element);
    configureAria();
    watchSelected();

    function configureAria() {
      element.attr({
        role: 'tablist'
      });
    }

    function watchSelected() {
      scope.$watch('selectedIndex', function watchSelectedIndex(newIndex, oldIndex) {
        // Note: if the user provides an invalid newIndex, all tabs will be deselected
        // and the associated view will be hidden.
        tabsCtrl.deselect( tabsCtrl.itemAt(oldIndex) );

        if (tabsCtrl.inRange(newIndex)) {
          var newTab = tabsCtrl.itemAt(newIndex);

          // If the newTab is disabled, find an enabled one to go to.
          if (newTab && newTab.isDisabled()) {
            newTab = newIndex > oldIndex ?
              tabsCtrl.next(newTab) :
              tabsCtrl.previous(newTab);
          }
          tabsCtrl.select(newTab);

        }
      });
    }

  }
}
})();
