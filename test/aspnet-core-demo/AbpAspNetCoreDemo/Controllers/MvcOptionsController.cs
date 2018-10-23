using System;
using Abp.Timing;

namespace AbpAspNetCoreDemo.Controllers
{
    public class MvcOptionsController : DemoControllerBase
    {
        public DateTime FormatDateTest1()
        {
            return Clock.Now;
        }

        public FormatDateTest2Output FormatDateTest2()
        {
            return new FormatDateTest2Output
            {
                Date = Clock.Now
            };
        }

        public class FormatDateTest2Output
        {
            public DateTime Date { get; set; }
        }
    }
}
