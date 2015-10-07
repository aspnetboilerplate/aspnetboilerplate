using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace MyProject.News
{
    public class News : Entity<long>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// 简历
        /// </summary>
        public virtual string Intro { get; set; }

        /// <summary>
        /// 详细内容
        /// </summary>
        public virtual string Content { get; set; }

        public News()
        {
            
        }

    }
}
