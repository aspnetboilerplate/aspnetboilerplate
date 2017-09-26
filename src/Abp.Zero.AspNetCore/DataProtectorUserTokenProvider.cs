using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Abp.Zero.AspNetCore.Internal;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.DataProtection;

namespace Abp.Zero.AspNetCore
{
    public class DataProtectorUserTokenProvider<TUser> : IUserTokenProvider<TUser, long>
        where TUser : class, IUser<long>
    {
        public IDataProtector Protector { get; }

        public TimeSpan TokenLifespan { get; set; }

        public DataProtectorUserTokenProvider(IDataProtector protector)
        {
            if (protector == null)
            {
                throw new ArgumentNullException(nameof(protector));
            }

            Protector = protector;
            TokenLifespan = TimeSpan.FromDays(1.0);
        }

        public async Task<string> GenerateAsync(string purpose, UserManager<TUser, long> manager, TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            using (var ms = new MemoryStream())
            {
                using (var binaryWriter = ms.CreateWriter())
                {
                    binaryWriter.Write(DateTimeOffset.UtcNow);
                    binaryWriter.Write(Convert.ToString((object)user.Id, (IFormatProvider)CultureInfo.InvariantCulture));
                    binaryWriter.Write(purpose ?? "");

                    if (manager.SupportsUserSecurityStamp)
                    {
                        binaryWriter.Write(await manager.GetSecurityStampAsync(user.Id));
                    }
                    else
                    {
                        binaryWriter.Write("");
                    }
                }

                return Convert.ToBase64String(Protector.Protect(ms.ToArray()));
            }
        }

        public Task<bool> IsValidProviderForUserAsync(UserManager<TUser, long> manager, TUser user)
        {
            return Task.FromResult(true);
        }

        public Task NotifyAsync(string token, UserManager<TUser, long> manager, TUser user)
        {
            return Task.FromResult(0);
        }

        public async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser, long> manager, TUser user)
        {
            try
            {
                var unprotectedData = this.Protector.Unprotect(Convert.FromBase64String(token));
                using (var ms = new MemoryStream(unprotectedData))
                {
                    using (var binaryReader = ms.CreateReader())
                    {
                        var creationTime = binaryReader.ReadDateTimeOffset();
                        var expirationTime = creationTime + TokenLifespan;
                        if (expirationTime < DateTimeOffset.UtcNow)
                        {
                            return false;
                        }

                        var userId = binaryReader.ReadString();
                        if (!string.Equals(userId, Convert.ToString((object) user.Id, CultureInfo.InvariantCulture)))
                        {
                            return false;
                        }

                        var purp = binaryReader.ReadString();
                        if (!string.Equals(purp, purpose))
                        {
                            return false;
                        }

                        var stamp = binaryReader.ReadString();
                        if (binaryReader.PeekChar() != -1)
                        {
                            return false;
                        }

                        if (!manager.SupportsUserSecurityStamp)
                        {
                            return stamp == "";
                        }

                        var expectedStamp = await manager.GetSecurityStampAsync(user.Id);

                        return stamp == expectedStamp;
                    }
                }
            }
            catch
            {
            }

            return false;
        }
    }
}
