declare namespace abp {

    let appPath: string;

    let pageLoadTime: Date;

    function toAbsAppPath(path: string): string;

    namespace multiTenancy {

        enum sides {

            TENANT = 1,

            HOST = 2

        }

        let isEnabled: boolean;

        let ignoreFeatureCheckForHostUsers: boolean;

        let tenantIdCookieName: string;

        function setTenantIdCookie(tenantId?: number): void;

        function getTenantIdCookie(): number;

    }

    interface IAbpSession {

        readonly userId?: number;

        readonly tenantId?: number;

        readonly impersonatorUserId?: number;

        readonly impersonatorTenantId?: number;

        readonly multiTenancySide: multiTenancy.sides;

    }

    let session: IAbpSession;

    namespace localization {

        interface ILanguageInfo {

            name: string;

            displayName: string;

            icon: string;

            isDefault: boolean;

            isDisabled: boolean;

        }

        interface ILocalizationSource {

            name: string;

            type: string;

        }

        let languages: ILanguageInfo[];

        let currentLanguage: ILanguageInfo;

        let sources: ILocalizationSource[];

        let defaultSourceName: string;

        let values: { [key: string]: string };

        let abpWeb: (key: string) => string;

        function localize(key: string, sourceName: string): string;

        function getSource(sourceName: string): (key: string) => string;

        function isCurrentCulture(name: string): boolean;
    }

    namespace auth {

        let allPermissions: { [name: string]: boolean };

        let grantedPermissions: { [name: string]: boolean };

        function isGranted(permissionName: string): boolean;

        function isAnyGranted(...args: string[]): boolean;

        function areAllGranted(...args: string[]): boolean;

        let tokenCookieName: string;

        /**
         * Saves auth token.
         * @param authToken The token to be saved.
         * @param expireDate Optional expire date. If not specified, token will be deleted at end of the session.
         */
        function setToken(authToken: string, expireDate?: Date): void;

        function getToken(): string;

        function clearToken(): void;
    }

    namespace features {

        interface IFeature {

            value: string;

        }

        let allFeatures: { [name: string]: IFeature };

        function get(name: string): IFeature;

        function getValue(name: string): string;

        function isEnabled(name: string): boolean;

    }

    namespace setting {

        let values: { [name: string]: string };

        function get(name: string): string;

        function getBoolean(name: string): boolean;

        function getInt(name: string): number;

        enum settingScopes {

            Application = 1,

            Tenant = 2,

            User = 4
        }
    }

    namespace nav {

        interface IMenu {

            name: string;

            displayName?: string;

            customData?: any;

            items: IMenuItem[];

        }

        interface IMenuItem {

            name: string;

            order: number;

            displayName?: string;

            icon?: string;

            url?: string;

            customData?: any;

            items: IMenuItem[];

        }

        let menus: { [name: string]: IMenu };

    }

    namespace notifications {

        enum severity {
            INFO,
            SUCCESS,
            WARN,
            ERROR,
            FATAL
        }

        enum userNotificationState {
            UNREAD,
            READ
        }

        //TODO: We can extend this interface to define built-in notification types, like ILocalizableMessageNotificationData 
        interface INotificationData {

            type: string;

            properties: any;
        }

        interface INotification {

            id: string;

            notificationName: string;

            severity: severity;

            entityType?: any;

            entityTypeName?: string;

            entityId?: any;

            data: INotificationData;

            creationTime: Date;

        }

        interface IUserNotification {

            id: string;

            userId: number;

            state: userNotificationState;

            notification: INotification;
        }

        let messageFormatters: any;

        function getUserNotificationStateAsString(userNotificationState: userNotificationState): string;

        function getUiNotifyFuncBySeverity(severity: severity): (message: string, title?: string, options?: any) => void;

        function getFormattedMessageFromUserNotification(userNotification: IUserNotification): string;

        function showUiNotifyForUserNotification(userNotification: IUserNotification, options?: any): void;

    }

    namespace log {

        enum levels {
            DEBUG,
            INFO,
            WARN,
            ERROR,
            FATAL
        }

        let level: levels;

        function log(logObject?: any, logLevel?: levels): void;

        function debug(logObject?: any): void;

        function info(logObject?: any): void;

        function warn(logObject?: any): void;

        function error(logObject?: any): void;

        function fatal(logObject?: any): void;

    }

    namespace notify {

        function info(message: string, title?: string, options?: any): void;

        function success(message: string, title?: string, options?: any): void;

        function warn(message: string, title?: string, options?: any): void;

        function error(message: string, title?: string, options?: any): void;

    }

    namespace message {

        //TODO: these methods return jQuery.Promise instead of any. fix it.

        function info(message: string, title?: string, options?: any): any;

        function success(message: string, title?: string, options?: any): any;

        function warn(message: string, title?: string, options?: any): any;

        function error(message: string, title?: string, options?: any): any;

        function confirm(message: string, title?: string, callback?: (result: boolean) => void, options?: any): any;

    }

    namespace ui {

        function block(elm?: any): void;

        function unblock(elm?: any): void;

        function setBusy(elm?: any, optionsOrPromise?: any): void;

        function clearBusy(elm?: any): void;

    }

    namespace event {

        function on(eventName: string, callback: (...args: any[]) => void): void;

        function off(eventName: string, callback: (...args: any[]) => void): void;

        function trigger(eventName: string, ...args: any[]): void;

    }

    interface INameValue {
        name: string;
        value?: any;
    }

    namespace utils {

        function createNamespace(root: any, ns: string): any;

        function replaceAll(str: string, search: string, replacement: any): string;

        function formatString(str: string, ...args: any[]): string;

        function toPascalCase(str: string): string;

        function toCamelCase(str: string): string;

        function truncateString(str: string, maxLength: number): string;

        function truncateStringWithPostfix(str: string, maxLength: number, postfix?: string): string;

        function isFunction(obj: any): boolean;

        function buildQueryString(parameterInfos: INameValue[], includeQuestionMark?: boolean): string;

        /**
        * Sets a cookie value for given key.
        * This is a simple implementation created to be used by ABP.
        * Please use a complete cookie library if you need.
        * @param {string} key
        * @param {string} value 
        * @param {Date} expireDate (optional). If not specified the cookie will expire at the end of session.
        * @param {string} path (optional)
        */
        function setCookieValue(key: string, value: string, expireDate?: Date, path?: string): void;

        /**
        * Gets a cookie with given key.
        * This is a simple implementation created to be used by ABP.
        * Please use a complete cookie library if you need.
        * @param {string} key
        * @returns {string} Cookie value or null
        */
        function getCookieValue(key: string): string;

        /**
         * Deletes cookie for given key.
         * This is a simple implementation created to be used by ABP.
         * Please use a complete cookie library if you need.
         * @param {string} key
         * @param {string} path (optional)
         */
        function deleteCookie(key: string, path?: string): void;
    }

    namespace timing {

        interface IClockProvider {

            supportsMultipleTimezone: boolean;

            now(): Date;

            normalize(date: Date): Date;

        }

        interface ITimeZoneInfo {

            windows: {

                timeZoneId: string;

                baseUtcOffsetInMilliseconds: number;

                currentUtcOffsetInMilliseconds: number;

                isDaylightSavingTimeNow: boolean;

            },

            iana: {

                timeZoneId: string;

            }

        }

        const utcClockProvider: IClockProvider;

        const localClockProvider: IClockProvider;

        const unspecifiedClockProvider: IClockProvider;

        function convertToUserTimezone(date: Date): Date;

        let timeZoneInfo: ITimeZoneInfo;
    }

    namespace clock {

        let provider: timing.IClockProvider;

        function now(): Date;

        function normalize(date: Date): Date;

    }

    namespace security {

        namespace antiForgery {

            let tokenCookieName: string;

            let tokenHeaderName: string;

            function getToken(): string;
        }

    }

}