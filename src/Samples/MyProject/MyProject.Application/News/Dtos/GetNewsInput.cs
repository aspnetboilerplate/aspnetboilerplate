using Abp.Application.Services.Dto;

namespace MyProject.News.Dtos
{
  public  class GetNewsInput : IInputDto
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
    }
}
