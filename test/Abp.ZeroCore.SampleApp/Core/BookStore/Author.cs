using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Abp.ZeroCore.SampleApp.Core.BookStore
{
    public class Author : Entity<Guid>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override Guid Id { get; set; }

        public string Name { get; set; }
    }
}