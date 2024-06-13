using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;

namespace Abp.Zero.SampleApp.TPH
{
    public class StudentLectureNote : Entity
    {
        [Required]
        public virtual Student Student { get; set; }

        public virtual int StudentId { get; set; }

        public virtual string CourseName { get; set; }

        public virtual double Note { get; set; }

        public virtual string Information { get; set; }
    }
}
