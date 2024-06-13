using Abp.NHibernate.EntityMappings;
using Abp.Zero.SampleApp.TPH.NHibernate;

namespace Abp.Zero.SampleApp.NHibernate.Mappings
{
    public class StudentMap :EntityMap<NhStudent>
    {
        public StudentMap() : base("People")
        {
            Map(x => x.Grade);
            Map(x => x.Name);
            Map(x => x.IdCard);
            Map(x => x.Address);
            References(x => x.CitizenshipInformation);
            HasMany(x => x.LectureNotes);
        }
    }
    
    public class CitizenshipInformationMap : EntityMap<NhCitizenshipInformation>
    {
        public CitizenshipInformationMap() : base("Citizenship")
        {
            Map(x => x.CitizenShipId);
            References(x => x.Student);
        }
    }
    
    public class StudentLectureNoteMap : EntityMap<NhStudentLectureNote>
    {
        public StudentLectureNoteMap() : base("StudentLectureNotes")
        {
            Map(x => x.StudentId);
            References(x => x.Student);
            Map(x => x.Note);
            Map(x => x.CourseName);
            Map(x => x.Information);
        }
    }
}