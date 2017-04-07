using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace Abp.TestBase.SampleApplication.People.Dto
{
    [AutoMapTo(typeof(Person))]
    public class CreatePersonInput
    {
        [Range(1, int.MaxValue)]
        public int ContactListId { get; set; }

        [Required]
        [MaxLength(Person.MaxNameLength)]
        public string Name { get; set; }
    }
}