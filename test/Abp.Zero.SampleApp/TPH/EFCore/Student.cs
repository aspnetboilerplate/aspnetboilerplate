using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Abp.Zero.SampleApp.TPH
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
