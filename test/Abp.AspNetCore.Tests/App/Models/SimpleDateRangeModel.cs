using System;
using Abp.Timing;

namespace Abp.AspNetCore.App.Models
{
    public class SimpleDateModel
    {
        public DateTime Date { get; set; }
    }

    public class SimpleDateModel2
    {
        [DisableDateTimeNormalization]
        public DateTime Date { get; set; }
    }
}