angular.module('app')
    .filter('fromNow', function() {
        return function(input) {
            return moment(input).fromNow();
        };
    });