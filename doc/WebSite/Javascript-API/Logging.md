When you want to write some simple log in the client, you can use
console.log('...') API as you know. But it's not supported by all
browsers and your script may be broken. So, you should check it first if
console is available. Also, you may want to write logs somewhere else.
Even you may want to write logs in some level. ASP.NET Boilerplate
defines safe logging functions:

    abp.log.debug('...');
    abp.log.info('...');
    abp.log.warn('...');
    abp.log.error('...');
    abp.log.fatal('...');

You can change log level by setting **abp.log.level** to one of
abp.log.levels (ex: abp.log.levels.INFO to do not write debug logs).
These functions write logs to browser's console by default. But you can
override/extend this behaviour if you need.
