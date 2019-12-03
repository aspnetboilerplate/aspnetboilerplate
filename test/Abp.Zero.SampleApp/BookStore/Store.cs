using System;
using Abp.Domain.Entities;

namespace Abp.Zero.SampleApp.BookStore
{
    public class Store : Entity<Guid>
    {
        public string Name { get; set; }
    }
}
