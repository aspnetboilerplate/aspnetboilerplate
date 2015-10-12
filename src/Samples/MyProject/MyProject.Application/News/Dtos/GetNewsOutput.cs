using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace MyProject.News.Dtos
{
  public  class GetNewsOutput : IOutputDto
    {
        public List<NewsDto> News { get; set; }
    }
}
