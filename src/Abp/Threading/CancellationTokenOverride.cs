using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Abp.Threading
{
    public class CancellationTokenOverride
    {
        public CancellationToken CancellationToken { get; }

        public CancellationTokenOverride(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
        }
    }
}
