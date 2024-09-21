using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;

namespace Abp.Zero.SampleApp.TPH.NHibernate
{
    public class NhStudentLectureNote : Entity
    {
        [Required]
        public virtual NhStudent Student { get; set; }

        public virtual int StudentId { get; set; }

        public virtual string CourseName { get; set; }

        public virtual double Note { get; set; }

        public virtual string Information { get; set; }
    }
}
