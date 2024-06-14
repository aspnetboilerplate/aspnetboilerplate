using System.Collections.Generic;

namespace Abp.Zero.SampleApp.TPH.EFCore
{
    public class Student : PersonWithIdCardAndAddress
    {
        public int Grade { get; set; }

        public CitizenshipInformation CitizenshipInformation { get; set; }

        public ICollection<StudentLectureNote> LectureNotes { get; set; }

        public Student()
        {
            LectureNotes = new List<StudentLectureNote>();
        }
    }
}
