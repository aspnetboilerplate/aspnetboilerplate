using System;
using Abp.Application.Services.Dto;

namespace MyProject.News.Dtos
{
    public class NewsDto : EntityDto<long>
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
