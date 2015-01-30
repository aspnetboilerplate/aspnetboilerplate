using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Abp.TestBase.SampleApplication.People.Dto
{
    [AutoMapTo(typeof(Person))]
    public class CreatePersonInput : IInputDto
    {
        [Required]
        [MaxLength(Person.MaxNameLength)]
        public string Name { get; set; }
    }
}