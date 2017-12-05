Message API is used to show a message to the user or to get a
confirmation from user.

Message API is implemented using
[sweetalert](http://t4t5.github.io/sweetalert/) by default. To make
sweetalert work, you should include it's css & javascript files, then
include **abp.sweet-alert.js** to your page as adapter.

### Show message

Examples:

    abp.message.info('some info message', 'some optional title');
    abp.message.success('some success message', 'some optional title');
    abp.message.warn('some warning message', 'some optional title');
    abp.message.error('some error message', 'some optional title');

 A success message is shown below:

<img src="../images/success_message.png" alt="Success message using sweetalert" class="img-thumbnail" />

### Confirmation

Example:

    abp.message.confirm(
        'User admin will be deleted.',
        'Are you sure?',
        function (isConfirmed) {
            if (isConfirmed) {
                //...delete user
            }
        }
    );

Second argument (title) is optional here (so, callback function can be
second argument).

A confirmation message is shown below:

<img src="../images/confirmation_message.png" alt="Confirmation message using sweetalert" class="img-thumbnail" />

ASP.NET Boilerplate internally uses Message API. For example, it calls
abp.message.error if an [AJAX](/Pages/Documents/Javascript-API/AJAX)
call fails.
