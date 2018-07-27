using System;
using Abp.Domain.Entities;

namespace Abp.Zero.SampleApp.BookStore
{
    public class Book : Entity<Guid>
    {
        public string Name { get; set; }
    }
}
