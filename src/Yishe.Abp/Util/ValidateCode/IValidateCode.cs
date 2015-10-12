using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yishe.Abp.ValidateCode
{
    public interface IValidateCode : ITransientDependency
    {
        string CreateValidateCode(int length);
        byte[] CreateValidateGraphic(string validateCode);

    }
}
