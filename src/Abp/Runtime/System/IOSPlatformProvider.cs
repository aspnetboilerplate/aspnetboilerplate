using System.Runtime.InteropServices;

namespace Abp.Runtime.System
{
    public interface IOSPlatformProvider
    {
        OSPlatform GetCurrentOSPlatform();
    }
}