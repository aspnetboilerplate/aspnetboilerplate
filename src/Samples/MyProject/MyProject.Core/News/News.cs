using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace MyProject.News
{
    public class News : Entity<long>
    {
        /// <summary>
        /// 标题
        /// </summary>
        [Required]
        public virtual string Title { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        [Required]
        public virtual string Intro { get; set; }

        /// <summary>
        /// 详细内容
        /// </summary>
        [Required]
        public virtual string Content { get; set; }

        public News()
        {
            
        }

    }
}
