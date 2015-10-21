using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reation.CMS.Articles.Dtos
{
    public class ArticleDto : EntityDto<long>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 简历
        /// </summary>
        public string Intro { get; set; }

        /// <summary>
        /// 详细内容
        /// </summary>
        public string Content { get; set; }
    }
}
