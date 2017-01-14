var abp = abp || {};
(function () {
    if (!moment || !moment.tz) {
        return;
    }

    /* DEFAULTS *************************************************/

    abp.timing = abp.timing || {};

    /* FUNCTIONS **************************************************/

    abp.timing.convertToUserTimezone = function (date) {
        var momentDate = moment(date);
        var targetDate = momentDate.clone().tz(abp.timing.timeZoneInfo.iana.timeZoneId);
        return targetDate;
    };

})();