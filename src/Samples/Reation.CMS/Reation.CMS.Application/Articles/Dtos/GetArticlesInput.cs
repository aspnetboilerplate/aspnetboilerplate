using Abp.Application.Services.Dto;

namespace Reation.CMS.Articles.Dtos
{
  public  class GetArticlesInput : IInputDto
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
    }
}
