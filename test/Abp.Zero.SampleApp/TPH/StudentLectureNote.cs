using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;

namespace Abp.Zero.SampleApp.TPH
{
    public class StudentLectureNote : Entity
    {
        [Required]
        public Student Student { get; set; }

        public int StudentId { get; set; }

        public string CourseName { get; set; }

        public double Note { get; set; }

        public string Information { get; set; }
    }
}
