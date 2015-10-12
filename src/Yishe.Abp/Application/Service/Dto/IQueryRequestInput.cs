using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yishe.Abp.Application.Services.Dto
{
    public interface IQueryRequestInput : IInputDto, ISortedResultRequest, ICustomValidate,IPagedRequest
    {
        string Keywords { get; set; }
        //string Sorting { get; set; }
        //int PageIndex { get; set; }
     
        //int SkipCount { get; set; }
        //void AddValidationErrors(System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult> results);
    }
}
