using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace Reation.CMS.Articles.Dtos
{
  public  class GetArticlesOutput : IOutputDto
    {
        public List<ArticleDto> News { get; set; }
    }
}
