We love to show some fancy auto-disappearing notifications when
something happens, like when an item is saved or a problem occured.
ASP.NET Boilerplate defines standard APIs for that.

    abp.notify.success('a message text', 'optional title');
    abp.notify.info('a message text', 'optional title');
    abp.notify.warn('a message text', 'optional title');
    abp.notify.error('a message text', 'optional title');

It also can get a 3rd argument (object) as **custom options** of the
notification library.

Notification API is implemented using
[toastr](http://codeseven.github.io/toastr/demo.html) library by
default. To make toastr work, you should include toastr's css &
javascript files, then include **abp.toastr.js** to your page as
adapter. A toastr success notification is shown below:

<img src="../images/success_notification.png" alt="Success notification using toastr.js" class="img-thumbnail" />

You can also implement notification in your favourite notification
library. Just override all functions in a custom javascript file and
include it in to your page instead of abp.toastr.js (You can check this
file to see implementation, it's pretty simple).
