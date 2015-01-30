using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace Abp.TestBase.SampleApplication.People.Dto
{
    public class CreatePersonInput : IInputDto
    {
        [Required]
        [MaxLength(Person.MaxNameLength)]
        public string Name { get; set; }
    }
}