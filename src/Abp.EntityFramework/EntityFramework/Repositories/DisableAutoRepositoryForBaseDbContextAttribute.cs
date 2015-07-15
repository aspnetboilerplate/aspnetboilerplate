using System;

namespace Abp.EntityFramework.Repositories
{
    /// <summary>
    /// Add this class to a DbContext to disable auto-repository generation for entities defined in base DbDontext.
    /// This is useful if you inherit same DbContext by more than one DbContext.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DisableAutoRepositoryForBaseDbContextAttribute : Attribute
    {

    }
}