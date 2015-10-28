using System;
using Abp.Application.Services.Dto;

namespace MyAbpZeroProject.Users.Dto
{

    public class GetAllUsersOutput : IOutputDto
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
