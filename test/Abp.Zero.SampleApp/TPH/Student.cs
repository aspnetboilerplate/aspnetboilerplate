using System.Collections.Generic;

namespace Abp.Zero.SampleApp.TPH
{
    public class Student : PersonWithIdCardAndAddress
    {
        public int Grade { get; set; }

        public virtual ICollection<StudentLectureNote> LectureNotes { get; set; }
    }
}
