When you want to write a simple log in the client, you can use
console.log('...') API as you may already know. However, it's not supported by all
browsers and your script may break as a result. You must first check if
console is available. You may also want to write logs somewhere else.
You may evem want to write logs at some other level. ASP.NET Boilerplate
defines these safe logging functions:

    abp.log.debug('...');
    abp.log.info('...');
    abp.log.warn('...');
    abp.log.error('...');
    abp.log.fatal('...');

You can change the log-level by setting the **abp.log.level** to one of the
abp.log.levels (ex: abp.log.levels.INFO does not write to the debug logs).
These functions write logs to the browser's console by default, but you can
override/extend this behavior if you need to.
