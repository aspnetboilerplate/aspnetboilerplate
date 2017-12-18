ASP.NET Boilerplate provides useful APIs to make all or some part of the
page blocked or/and busy (with a busy indicator).

### UI Block API

This API is used to block whole page or an element on the page with an
transparent overlay. Thus, user can not click it. It's pretty useful
when saving a form or loading an area (a div or even complete page).
Examples:

    abp.ui.block(); //Block all page
    abp.ui.block($('#MyDivElement')); //You can use any jQuery selection..
    abp.ui.block('#MyDivElement'); //..or directly selector
    abp.ui.unblock(); //Unblock all page
    abp.ui.unblock('#MyDivElement'); //Unblock specific element

UI Block API is implemented using
[blockUI](http://malsup.com/jquery/block/) jQuery plug-in as default. To
make it work, you should include it's javascript file, then include
**abp.blockUI.js** to your page as adapter (See this javascript file for
simple implementation and defaults).

### UI Busy API

This API is used to make some page/element busy. For example, you may
want to block a form and show a busy indicator while submitting the form
to the server. Examples:

    abp.ui.setBusy('#MyLoginForm');
    abp.ui.clearBusy('#MyLoginForm');

Example screenshot:

<img src="../images/ui_busy_sample.png" alt="A busy div with spin.js" class="img-thumbnail" />

The parameter should be a jQuery selector (like '\#MyLoginForm') or
jQuery selection (like $('\#MyLoginForm')). To make busy the whole page,
you can pass null (or 'body') as selector.

setBusy function can take a promise (as second parameter) and
automatically clear busy when this promise completed. Example:

    abp.ui.setBusy(
        $('#MyLoginForm'), 
        abp.ajax({ ... })   
    );

Since [abp.ajax](/Pages/Documents/Javascript-API/AJAX) returns promise,
we can directly pass it as promise. To learn more about promises, see
jQuery's [Deferred](http://api.jquery.com/category/deferred-object/).
setBusy also supports Q (and Angular's $http service).

UI Busy API is implemented using
[spin.js](http://fgnass.github.io/spin.js/). To make it work, you should
include it's javascript file, then include **abp.spin.js** to your page
as adapter (See this javascript file for simple implementation and
defaults).
