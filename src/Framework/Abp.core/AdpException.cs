using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp
{
    public class AbpException: Exception
    {
        public AbpException()
        {

        }

        public AbpException(string message)
            : base(message)
        {

        }

        public AbpException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
