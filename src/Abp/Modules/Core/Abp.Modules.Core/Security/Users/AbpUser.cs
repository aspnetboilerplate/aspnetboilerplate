using System;
using System.Threading;
using Abp.Domain.Entities;
using Abp.Utils.Helpers;
using Microsoft.AspNet.Identity;

namespace Abp.Security.Users
{
    /// <summary>
    /// Represents a user.
    /// </summary>
    public class AbpUser : Entity<long>, IUser<long>
    {
        /// <summary>
        /// Tenant of this user.
        /// </summary>
        public virtual int? TenantId { get; set; }

        /// <summary>
        /// User name.
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// Name of the user.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Surname of the user.
        /// </summary>
        public virtual string Surname { get; set; }

        /// <summary>
        /// Email address of the user.
        /// </summary>
        public virtual string EmailAddress { get; set; }

        /// <summary>
        /// Is the <see cref="EmailAddress"/> confirmed.
        /// </summary>
        public virtual bool IsEmailConfirmed { get; set; }

        /// <summary>
        /// Confirmation code for email.
        /// </summary>
        public virtual string EmailConfirmationCode { get; set; }

        /// <summary>
        /// Password of the user.
        /// </summary>
        public virtual string Password { get; set; }

        /// <summary>
        /// Reset code for password.
        /// It's not valid if it's null.
        /// It's for one usage and must be set to null after reset.
        /// </summary>
        public virtual string PasswordResetCode { get; set; }

        /// <summary>
        /// Gets Name and Surname joined by space.
        /// </summary>
        public virtual string NameAndSurname { get { return Name + " " + Surname; } } //TODO@Halil: Remove this?

        /// <summary>
        /// Gets current user id.
        /// </summary>
        public static long? CurrentUserId
        {
            get
            {
                var userId = Thread.CurrentPrincipal.Identity.GetUserId();
                if (userId == null)
                {
                    return null;
                }

                return Convert.ToInt32(userId);
            } 
        }

        public virtual void GenerateEmailConfirmationCode()
        {
            EmailConfirmationCode = RandomCodeGenerator.Generate(16);
        }

        public virtual void GeneratePasswordResetCode()
        {
            PasswordResetCode = RandomCodeGenerator.Generate(32);
        }

        public virtual void ConfirmEmail(string confirmationCode)
        {
            if (IsEmailConfirmed)
            {
                return;
            }

            if (EmailConfirmationCode != confirmationCode)
            {
                throw new ApplicationException("Wrong email confirmation code!");
            }

            IsEmailConfirmed = true;
        }
    }
}