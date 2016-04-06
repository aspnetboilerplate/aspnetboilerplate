using System;
using Abp.Timing;
using System.Security.Cryptography;

namespace Abp
{
    /// <summary>
    /// Implements <see cref="IGuidGenerator"/> by creating sequential Guids.
    /// This code is taken from jhtodd/SequentialGuid
    /// https://github.com/jhtodd/SequentialGuid/blob/master/SequentialGuid/Classes/SequentialGuid.cs
    /// </summary>
    public class SequentialGuidGenerator : IGuidGenerator
    {
        /// <summary>
        /// Gets the singleton <see cref="SequentialGuidGenerator"/> instance.
        /// </summary>
        public static SequentialGuidGenerator Instance { get { return _instance; } }
        private static readonly SequentialGuidGenerator _instance = new SequentialGuidGenerator();
        private static readonly RNGCryptoServiceProvider _rng = new RNGCryptoServiceProvider();
        public SequentialGuidDatabase SequentialGuidDatabase { get; set; }
        public Guid Create()
        {
           return NewGuid(SequentialGuidDatabase);
        }

        public  Guid NewGuid(SequentialGuidType guidType)
        {
            // We start with 16 bytes of cryptographically strong random data.
            byte[] randomBytes = new byte[10];
            _rng.GetBytes(randomBytes);

            // An alternate method: use a normally-created GUID to get our initial
            // random data:
            // byte[] randomBytes = Guid.NewGuid().ToByteArray();
            // This is faster than using RNGCryptoServiceProvider, but I don't
            // recommend it because the .NET Framework makes no guarantee of the
            // randomness of GUID data, and future versions (or different
            // implementations like Mono) might use a different method.

            // Now we have the random basis for our GUID.  Next, we need to
            // create the six-byte block which will be our timestamp.

            // We start with the number of milliseconds that have elapsed since
            // DateTime.MinValue.  This will form the timestamp.  There's no use
            // being more specific than milliseconds, since DateTime.Now has
            // limited resolution.

            // Using millisecond resolution for our 48-bit timestamp gives us
            // about 5900 years before the timestamp overflows and cycles.
            // Hopefully this should be sufficient for most purposes. :)
            long timestamp = DateTime.UtcNow.Ticks / 10000L;

            // Then get the bytes
            byte[] timestampBytes = BitConverter.GetBytes(timestamp);

            // Since we're converting from an Int64, we have to reverse on
            // little-endian systems.
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(timestampBytes);
            }

            byte[] guidBytes = new byte[16];

            switch (guidType)
            {
                case SequentialGuidType.SequentialAsString:
                case SequentialGuidType.SequentialAsBinary:

                    // For string and byte-array version, we copy the timestamp first, followed
                    // by the random data.
                    Buffer.BlockCopy(timestampBytes, 2, guidBytes, 0, 6);
                    Buffer.BlockCopy(randomBytes, 0, guidBytes, 6, 10);

                    // If formatting as a string, we have to compensate for the fact
                    // that .NET regards the Data1 and Data2 block as an Int32 and an Int16,
                    // respectively.  That means that it switches the order on little-endian
                    // systems.  So again, we have to reverse.
                    if (guidType == SequentialGuidType.SequentialAsString && BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(guidBytes, 0, 4);
                        Array.Reverse(guidBytes, 4, 2);
                    }

                    break;

                case SequentialGuidType.SequentialAtEnd:

                    // For sequential-at-the-end versions, we copy the random data first,
                    // followed by the timestamp.
                    Buffer.BlockCopy(randomBytes, 0, guidBytes, 0, 10);
                    Buffer.BlockCopy(timestampBytes, 2, guidBytes, 10, 6);
                    break;
            }

            return new Guid(guidBytes);
        }

        public  Guid NewGuid(SequentialGuidDatabase database)
        {
            switch (database)
            {
                case SequentialGuidDatabase.SqlServer: return NewGuid(SequentialGuidType.SequentialAtEnd);
                case SequentialGuidDatabase.Oracle: return NewGuid(SequentialGuidType.SequentialAsBinary);
                case SequentialGuidDatabase.MySql: return NewGuid(SequentialGuidType.SequentialAsString);
                case SequentialGuidDatabase.PostgreSql: return NewGuid(SequentialGuidType.SequentialAsString);
                default: throw new InvalidOperationException();
            }
        }
    }


    public enum SequentialGuidDatabase
    {
        SqlServer,
        Oracle,
        MySql,
        PostgreSql,
    }
    /// <summary>
    /// Describes the type of a sequential GUID value.
    /// </summary>
    public enum SequentialGuidType
    {
        /// <summary>
        /// The GUID should be sequential when formatted using the
        /// <see cref="Guid.ToString()" /> method.
        /// </summary>
        SequentialAsString,

        /// <summary>
        /// The GUID should be sequential when formatted using the
        /// <see cref="Guid.ToByteArray" /> method.
        /// </summary>
        SequentialAsBinary,

        /// <summary>
        /// The sequential portion of the GUID should be located at the end
        /// of the Data4 block.
        /// </summary>
        SequentialAtEnd
    }
}