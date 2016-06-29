using System;
using Abp.Collections.Extensions;
using JetBrains.Annotations;

namespace Abp.Application.Services
{
    internal static class AbpCrossCuttingConcerns
    {
        public const string Auditing = "AbpAuditing";
        public const string Validation = "AbpValidation";
        public const string UnitOfWork = "AbpUnitOfWork";
        public const string Authorization = "AbpAuthorization";

        public static void AddApplied(object obj, params string[] appliedConcerns)
        {
            if (appliedConcerns.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(appliedConcerns), $"{nameof(appliedConcerns)} should be provided!");
            }

            (obj as IAvoidDuplicateCrossCuttingConcerns)?.AppliedCrossCuttingConcerns.AddRange(appliedConcerns);
        }

        public static bool IsApplied([NotNull] object obj, [NotNull] string concern)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (concern == null)
            {
                throw new ArgumentNullException(nameof(concern));
            }

            return (obj as IAvoidDuplicateCrossCuttingConcerns)?.AppliedCrossCuttingConcerns.Contains(concern) ?? false;
        }
    }
}