using System;
using System.Globalization;
using System.Net;

namespace Abp.Auditing
{
    internal class IPEndPointHelper
    {
        // Backport IPEndPoint Parsing methods back to .net 4.x
        // Reference https://github.com/dotnet/runtime/blob/master/src/libraries/System.Net.Primitives/src/System/Net/IPEndPoint.cs
        internal static bool TryParse(string s, out IPEndPoint result)
        {
            return TryParse(s.AsSpan(), out result);
        }

        internal static bool TryParse(ReadOnlySpan<char> s, out IPEndPoint result)
        {
            int addressLength = s.Length;  // If there's no port then send the entire string to the address parser
            int lastColonPos = s.LastIndexOf(':');

            // Look to see if this is an IPv6 address with a port.
            if (lastColonPos > 0)
            {
                if (s[lastColonPos - 1] == ']')
                {
                    addressLength = lastColonPos;
                }
                // Look to see if this is IPv4 with a port (IPv6 will have another colon)
                else if (s.Slice(0, lastColonPos).LastIndexOf(':') == -1)
                {
                    addressLength = lastColonPos;
                }
            }

            if (IPAddress.TryParse(s.Slice(0, addressLength).ToString(), out IPAddress address))
            {
                uint port = 0;
                if (addressLength == s.Length ||
                    (uint.TryParse(s.Slice(addressLength + 1).ToString(), NumberStyles.None, CultureInfo.InvariantCulture, out port) && port <= IPEndPoint.MaxPort))

                {
                    result = new IPEndPoint(address, (int)port);
                    return true;
                }
            }

            result = null;
            return false;
        }

        internal static IPEndPoint Parse(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            return Parse(s.AsSpan());
        }

        internal static IPEndPoint Parse(ReadOnlySpan<char> s)
        {
            if (TryParse(s, out IPEndPoint result))
            {
                return result;
            }

            throw new FormatException("An invalid IPEndPoint was specified.");
        }
    }
}
