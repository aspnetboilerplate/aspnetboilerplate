using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Abp.Zero.SampleApp.BookStore
{
    public class Author : Entity<Guid>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override Guid Id { get; set; }

        public string Name { get; set; }
    }
}