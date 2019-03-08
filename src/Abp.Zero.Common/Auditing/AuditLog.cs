using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Abp.Domain.Entities;
using Abp.Extensions;
using Abp.Runtime.Validation;
using Abp.UI;

namespace Abp.Auditing
{
    /// <summary>
    /// Used to store audit logs.
    /// </summary>
    [Table("AbpAuditLogs")]
    public class AuditLog : Entity<long>, IMayHaveTenant
    {
        /// <summary>
        /// Maximum length of <see cref="ServiceName"/> property.
        /// </summary>
        public static int MaxServiceNameLength = 256;

        /// <summary>
        /// Maximum length of <see cref="MethodName"/> property.
        /// </summary>
        public static int MaxMethodNameLength = 256;

        /// <summary>
        /// Maximum length of <see cref="Parameters"/> property.
        /// </summary>
        public static int MaxParametersLength = 1024;

        /// <summary>
        /// Maximum length of <see cref="ReturnValue"/> property.
        /// </summary>
        public static int MaxReturnValueLength = 1024;

        /// <summary>
        /// Maximum length of <see cref="ClientIpAddress"/> property.
        /// </summary>
        public static int MaxClientIpAddressLength = 64;

        /// <summary>
        /// Maximum length of <see cref="ClientName"/> property.
        /// </summary>
        public static int MaxClientNameLength = 128;

        /// <summary>
        /// Maximum length of <see cref="BrowserInfo"/> property.
        /// </summary>
        public static int MaxBrowserInfoLength = 512;

        /// <summary>
        /// Maximum length of <see cref="Exception"/> property.
        /// </summary>
        public static int MaxExceptionLength = 2000;

        /// <summary>
        /// Maximum length of <see cref="CustomData"/> property.
        /// </summary>
        public static int MaxCustomDataLength = 2000;

        /// <summary>
        /// TenantId.
        /// </summary>
        public virtual int? TenantId { get; set; }

        /// <summary>
        /// UserId.
        /// </summary>
        public virtual long? UserId { get; set; }

        /// <summary>
        /// Service (class/interface) name.
        /// </summary>
        public virtual string ServiceName { get; set; }

        /// <summary>
        /// Executed method name.
        /// </summary>
        public virtual string MethodName { get; set; }

        /// <summary>
        /// Calling parameters.
        /// </summary>
        public virtual string Parameters { get; set; }

        /// <summary>
        /// Return values.
        /// </summary>
        public virtual string ReturnValue { get; set; }

        /// <summary>
        /// Start time of the method execution.
        /// </summary>
        public virtual DateTime ExecutionTime { get; set; }

        /// <summary>
        /// Total duration of the method call as milliseconds.
        /// </summary>
        public virtual int ExecutionDuration { get; set; }

        /// <summary>
        /// IP address of the client.
        /// </summary>
        public virtual string ClientIpAddress { get; set; }

        /// <summary>
        /// Name (generally computer name) of the client.
        /// </summary>
        public virtual string ClientName { get; set; }

        /// <summary>
        /// Browser information if this method is called in a web request.
        /// </summary>
        public virtual string BrowserInfo { get; set; }

        /// <summary>
        /// Exception object, if an exception occured during execution of the method.
        /// </summary>
        public virtual string Exception { get; set; }

        /// <summary>
        /// <see cref="AuditInfo.ImpersonatorUserId"/>.
        /// </summary>
        public virtual long? ImpersonatorUserId { get; set; }

        /// <summary>
        /// <see cref="AuditInfo.ImpersonatorTenantId"/>.
        /// </summary>
        public virtual int? ImpersonatorTenantId { get; set; }

        /// <summary>
        /// <see cref="AuditInfo.CustomData"/>.
        /// </summary>
        public virtual string CustomData { get; set; }

        /// <summary>
        /// Creates a new CreateFromAuditInfo from given <see cref="auditInfo"/>.
        /// </summary>
        /// <param name="auditInfo">Source <see cref="AuditInfo"/> object</param>
        /// <returns>The <see cref="AuditLog"/> object that is created using <see cref="auditInfo"/></returns>
        public static AuditLog CreateFromAuditInfo(AuditInfo auditInfo)
        {
            var exceptionMessage = GetAbpClearException(auditInfo.Exception);
            return new AuditLog
                   {
                       TenantId = auditInfo.TenantId,
                       UserId = auditInfo.UserId,
                       ServiceName = auditInfo.ServiceName.TruncateWithPostfix(MaxServiceNameLength),
                       MethodName = auditInfo.MethodName.TruncateWithPostfix(MaxMethodNameLength),
                       Parameters = auditInfo.Parameters.TruncateWithPostfix(MaxParametersLength),
                       ReturnValue = auditInfo.ReturnValue.TruncateWithPostfix(MaxReturnValueLength),
                       ExecutionTime = auditInfo.ExecutionTime,
                       ExecutionDuration = auditInfo.ExecutionDuration,
                       ClientIpAddress = auditInfo.ClientIpAddress.TruncateWithPostfix(MaxClientIpAddressLength),
                       ClientName = auditInfo.ClientName.TruncateWithPostfix(MaxClientNameLength),
                       BrowserInfo = auditInfo.BrowserInfo.TruncateWithPostfix(MaxBrowserInfoLength),
                       Exception = exceptionMessage.TruncateWithPostfix(MaxExceptionLength),
                       ImpersonatorUserId = auditInfo.ImpersonatorUserId,
                       ImpersonatorTenantId = auditInfo.ImpersonatorTenantId,
                       CustomData = auditInfo.CustomData.TruncateWithPostfix(MaxCustomDataLength)
                   };
        }

        public override string ToString()
        {
            return string.Format(
                "AUDIT LOG: {0}.{1} is executed by user {2} in {3} ms from {4} IP address.",
                ServiceName, MethodName, UserId, ExecutionDuration, ClientIpAddress
                );
        }

        /// <summary>
        /// Make audit exceptions more explicit.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string GetAbpClearException(Exception exception)
        {
            var clearMessage = "";
            switch (exception)
            {
                case null:
                    return null;

                case AbpValidationException abpValidationException:
                    clearMessage = "There are " + abpValidationException.ValidationErrors.Count + " validation errors:";
                    foreach (var validationResult in abpValidationException.ValidationErrors) 
                    {
                        var memberNames = "";
                        if (validationResult.MemberNames != null && validationResult.MemberNames.Any())
                        {
                            memberNames = " (" + string.Join(", ", validationResult.MemberNames) + ")";
                        }

                        clearMessage += "\r\n" + validationResult.ErrorMessage + memberNames;
                    }
                    break;

                case UserFriendlyException userFriendlyException:
                    clearMessage =
                        $"UserFriendlyException.Code:{userFriendlyException.Code}\r\nUserFriendlyException.Details:{userFriendlyException.Details}";
                    break;
            }

            return exception + (clearMessage.IsNullOrWhiteSpace() ? "" : "\r\n\r\n" + clearMessage);
        }
    }
}
