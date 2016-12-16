using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Abp.TestBase.SampleApplication.Crm
{
    [AutoMapFrom(typeof(Company))]
    public class CompanyDto : EntityDto
    {
        public string Name { get; set; }

        public DateTime CreationTime { get; set; }

        public Address ShippingAddress { get; set; }

        public Address BillingAddress { get; set; }
    }
}