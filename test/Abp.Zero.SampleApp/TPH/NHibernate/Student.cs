using System.Collections.Generic;

namespace Abp.Zero.SampleApp.TPH.NHibernate
{
    public class NhStudent : NhPersonWithIdCardAndAddress
    {
        public virtual int Grade { get; set; }

        public virtual NhCitizenshipInformation CitizenshipInformation { get; set; }

        public virtual ICollection<NhStudentLectureNote> LectureNotes { get; set; }
    }
}
