using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace MyProject.News.Dtos
{
   public class CreateNewsInput : IInputDto
    {
        /// <summary>
        /// 标题
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// 简历
        /// </summary>
        [Required]
        public string Intro { get; set; }

        /// <summary>
        /// 详细内容
        /// </summary>
        [Required]
        public string Content { get; set; }
    }
}
