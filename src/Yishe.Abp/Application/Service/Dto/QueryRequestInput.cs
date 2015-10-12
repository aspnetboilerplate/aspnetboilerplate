using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yishe.Abp.Util.Help;

namespace Yishe.Abp.Application.Services.Dto
{
    /// <summary>
    /// 传入获取查询列表的参数
    /// 用于实现<see cref="IApplicationService"/>的方法中，可自动验证输入参数，并可重写AddValidationErrors方法进行自定义验证
    /// </summary>
    [Serializable]
    public class QueryRequestInput : IQueryRequestInput
    {
        private string keywords;
        private int? skipCount;

        /// <summary>
        /// Creates a new <see cref="EntityRequestInput"/> object.
        /// </summary>
        public QueryRequestInput()
        {
            PageIndex = 1;
            PageSize = 10;
        }

        /// <summary>
        /// 每页面记录数
        /// </summary>
        [Range(1, 1000)]
        public virtual int PageSize { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public virtual int PageIndex { get; set; }

        /// <summary>
        /// 排序字段和排序方向
        /// </summary>
        public virtual string Sorting { get; set; }

        /// <summary>
        /// 查询起始时间
        /// </summary>
        public virtual DateTime? StartTime { get; set; }

        /// <summary>
        /// 查询结束时间
        /// </summary>
        public virtual DateTime? EndTime { get; set; }

        /// <summary>
        /// 查询关键词
        /// </summary>
        public virtual string Keywords
        {
            get
            {
                return keywords;
            }
            set
            {
                keywords = StringHelper.CleanSearchString(value);
            }
        }

        /// <summary>
        /// 要跳过的记录数。如果此属性有被赋值，以此为准，否则通过PageIndex和PageSize进行计算获得
        /// </summary>
        public virtual int SkipCount
        {
            get
            {
                if (!skipCount.HasValue)
                    skipCount = (PageIndex - 1) * PageSize;

                return skipCount.Value;
            }
            set
            {
                skipCount = value;
            }
        }

        public int MaxResultCount
        {
            get; set;
        }

        /// <summary>
        /// 添加自定义验证
        /// </summary>
        /// <param name="results"></param>
        public virtual void AddValidationErrors(List<ValidationResult> results)
        {
            //var validSortingValues = new[] { "CreationTime DESC", "VoteCount DESC", "ViewCount DESC", "AnswerCount DESC" };

            //if (!Sorting.IsIn(validSortingValues))
            //{
            //    results.Add(new ValidationResult("Sorting is not valid. Valid values: " + string.Join(", ", validSortingValues)));
            //}
        }

    }
}
