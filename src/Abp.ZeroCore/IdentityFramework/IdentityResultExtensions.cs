using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Collections.Extensions;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.Text;
using Abp.UI;
using Abp.Zero;
using Microsoft.AspNetCore.Identity;

namespace Abp.IdentityFramework
{
    public static class IdentityResultExtensions
    {
        private static readonly Dictionary<string, string> IdentityLocalizations
            = new Dictionary<string, string>
              {
                  {"Optimistic concurrency failure, object has been modified.", "Identity.ConcurrencyFailure"},
                  {"An unknown failure has occurred.", "Identity.DefaultError"},
                  {"Email '{0}' is already taken.", "Identity.DuplicateEmail"},
                  {"Role name '{0}' is already taken.", "Identity.DuplicateRoleName"},
                  {"User name '{0}' is already taken.", "Identity.DuplicateUserName"},
                  {"Email '{0}' is invalid.", "Identity.InvalidEmail"},
                  {"The provided PasswordHasherCompatibilityMode is invalid.", "Identity.InvalidPasswordHasherCompatibilityMode"},
                  {"The iteration count must be a positive integer.", "Identity.InvalidPasswordHasherIterationCount"},
                  {"Role name '{0}' is invalid.", "Identity.InvalidRoleName"},
                  {"Invalid token.", "Identity.InvalidToken"},
                  {"User name '{0}' is invalid, can only contain letters or digits.", "Identity.InvalidUserName"},
                  {"A user with this login already exists.", "Identity.LoginAlreadyAssociated"},
                  {"Incorrect password.", "Identity.PasswordMismatch"},
                  {"Passwords must have at least one digit ('0'-'9').", "Identity.PasswordRequireDigit"},
                  {"Passwords must have at least one lowercase ('a'-'z').", "Identity.PasswordRequireLower"},
                  {"Passwords must have at least one non alphanumeric character.", "Identity.PasswordRequireNonAlphanumeric"},
                  {"Passwords must have at least one uppercase ('A'-'Z').", "Identity.PasswordRequireUpper"},
                  {"Passwords must be at least {0} characters.", "Identity.PasswordTooShort"},
                  {"Role {0} does not exist.", "Identity.RoleNotFound"},
                  {"User already has a password set.", "Identity.UserAlreadyHasPassword"},
                  {"User already in role '{0}'.", "Identity.UserAlreadyInRole"},
                  {"User is locked out.", "Identity.UserLockedOut"},
                  {"Lockout is not enabled for this user.", "Identity.UserLockoutNotEnabled"},
                  {"User {0} does not exist.", "Identity.UserNameNotFound"},
                  {"User is not in role '{0}'.", "Identity.UserNotInRole"}
              };

        /// <summary>
        /// Checks errors of given <see cref="IdentityResult"/> and throws <see cref="UserFriendlyException"/> if it's not succeeded.
        /// </summary>
        /// <param name="identityResult">Identity result to check</param>
        public static void CheckErrors(this IdentityResult identityResult)
        {
            if (identityResult.Succeeded)
            {
                return;
            }

            throw new UserFriendlyException(identityResult.Errors.Select(err => err.Description).JoinAsString(", "));
        }

        /// <summary>
        /// Checks errors of given <see cref="IdentityResult"/> and throws <see cref="UserFriendlyException"/> if it's not succeeded.
        /// </summary>
        /// <param name="identityResult">Identity result to check</param>
        /// <param name="localizationManager">Localization manager to localize error messages</param>
        public static void CheckErrors(this IdentityResult identityResult, ILocalizationManager localizationManager)
        {
            if (identityResult.Succeeded)
            {
                return;
            }

            throw new UserFriendlyException(identityResult.LocalizeErrors(localizationManager));
        }

        public static string LocalizeErrors(this IdentityResult identityResult, ILocalizationManager localizationManager)
        {
            return LocalizeErrors(identityResult, localizationManager, AbpZeroConsts.LocalizationSourceName);
        }

        public static string LocalizeErrors(this IdentityResult identityResult, ILocalizationManager localizationManager, string localizationSourceName)
        {
            if (identityResult.Succeeded)
            {
                throw new ArgumentException("identityResult.Succeeded should be false in order to localize errors.");
            }

            if (identityResult.Errors == null)
            {
                throw new ArgumentException("identityResult.Errors should not be null.");
            }

            var localizationSource = localizationManager.GetSource(localizationSourceName);
            return identityResult.Errors.Select(err => LocalizeErrorMessage(err.Description, localizationSource)).JoinAsString(" ");
        }

        private static string LocalizeErrorMessage(string identityErrorMessage, ILocalizationSource localizationSource)
        {
            foreach (var identityLocalization in IdentityLocalizations)
            {
                string[] values;
                if (FormattedStringValueExtracter.IsMatch(identityErrorMessage, identityLocalization.Key, out values))
                {
                    return localizationSource.GetString(identityLocalization.Value, values.Cast<object>().ToArray());
                }
            }

            return localizationSource.GetString("Identity.DefaultError");
        }
    }
}