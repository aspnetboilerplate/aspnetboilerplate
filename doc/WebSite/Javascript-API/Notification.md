We all love to show some fancy auto-disappearing notifications when
something happens, like when an item is saved or a problem has occurred.
ASP.NET Boilerplate defines some standard APIs for that.

    abp.notify.success('a message text', 'optional title');
    abp.notify.info('a message text', 'optional title');
    abp.notify.warn('a message text', 'optional title');
    abp.notify.error('a message text', 'optional title');

It can also get a 3rd argument (object) as the **custom options** of the
notification library.

The Notification API is implemented using the
[toastr](http://codeseven.github.io/toastr/demo.html) library by
default. To make toastr work, you must include toastr's CSS &
JavaScript files, then include **abp.toastr.js** to your page.

A toastr success notification is shown below:

<img src="../images/success_notification.png" alt="Success notification using toastr.js" class="img-thumbnail" />

You can also implement a notification in your favourite notification
library. Just override all functions in a custom JavaScript file and
include it in to your page instead of abp.toastr.js (You can check this
file to see the implementation, it's pretty simple).
