using System.ComponentModel.DataAnnotations;
using Adorable.Application.Services.Dto;
using Adorable.AutoMapper;

namespace Adorable.TestBase.SampleApplication.People.Dto
{
    [AutoMapTo(typeof(Person))]
    public class CreatePersonInput : IInputDto
    {
        [Range(1, int.MaxValue)]
        public int ContactListId { get; set; }

        [Required]
        [MaxLength(Person.MaxNameLength)]
        public string Name { get; set; }
    }
}