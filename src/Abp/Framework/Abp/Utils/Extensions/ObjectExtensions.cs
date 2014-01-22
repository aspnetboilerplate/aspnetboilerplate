using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Utils.Extensions
{
    public static class ObjectExtensions
    {
        public static T As<T>(this object obj)
        {
            return (T)obj;
        }
    }
}
