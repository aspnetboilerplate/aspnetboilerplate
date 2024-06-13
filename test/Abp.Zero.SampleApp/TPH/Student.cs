using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Abp.Zero.SampleApp.TPH
{
    public class Student : PersonWithIdCardAndAddress
    {
        public virtual int Grade { get; set; }

        public virtual CitizenshipInformation CitizenshipInformation { get; set; }

        public virtual ICollection<StudentLectureNote> LectureNotes { get; set; }
    }
}
