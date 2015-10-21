using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;

namespace Reation.CMS.Articles.Dtos
{
  public  class UpdateArticleInput : IInputDto
    {
        [Range(1, long.MaxValue)] //Data annotation attributes work as expected.
        public long Id { get; set; }

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
