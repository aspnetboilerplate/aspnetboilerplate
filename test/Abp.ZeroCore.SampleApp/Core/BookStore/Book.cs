using System;
using Abp.Domain.Entities;

namespace Abp.ZeroCore.SampleApp.Core.BookStore
{
    public class Book : Entity<Guid>
    {
        public string Name { get; set; }
    }
}
