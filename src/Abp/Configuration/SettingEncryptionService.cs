using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Extensions;

namespace Abp.Configuration
{
    public class SettingEncryptionService : ISettingEncryptionService, ITransientDependency
    {
        private readonly ISettingsConfiguration _settingsConfiguration;

        public SettingEncryptionService(ISettingsConfiguration settingsConfiguration)
        {
            _settingsConfiguration = settingsConfiguration;
        }

        public string Encrypt(SettingDefinition settingDefinition, string plainValue)
        {
            if (plainValue.IsNullOrEmpty())
            {
                return plainValue;
            }

            var plainTextBytes = Encoding.UTF8.GetBytes(plainValue);
            using (var password = new Rfc2898DeriveBytes(
                _settingsConfiguration.SettingEncryptionConfiguration.DefaultPassPhrase,
                _settingsConfiguration.SettingEncryptionConfiguration.DefaultSalt))
            {
                var keyBytes = password.GetBytes(_settingsConfiguration.SettingEncryptionConfiguration.Keysize / 8);
                using (var symmetricKey = Aes.Create())
                {
                    if (symmetricKey == null)
                    {
                        throw new ArgumentNullException(nameof(symmetricKey));
                    }

                    symmetricKey.Mode = CipherMode.CBC;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes,
                        _settingsConfiguration.SettingEncryptionConfiguration.InitVectorBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                var cipherTextBytes = memoryStream.ToArray();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public string Decrypt(SettingDefinition settingDefinition, string encryptedValue)
        {
            if (encryptedValue.IsNullOrEmpty())
            {
                return encryptedValue;
            }

            var cipherTextBytes = Convert.FromBase64String(encryptedValue);
            using (var password = new Rfc2898DeriveBytes(_settingsConfiguration.SettingEncryptionConfiguration.DefaultPassPhrase, _settingsConfiguration.SettingEncryptionConfiguration.DefaultSalt))
            {
                var keyBytes = password.GetBytes(_settingsConfiguration.SettingEncryptionConfiguration.Keysize / 8);
                using (var symmetricKey = Aes.Create())
                {
                    if (symmetricKey == null)
                    {
                        throw new ArgumentNullException(nameof(symmetricKey));
                    }
                    
                    symmetricKey.Mode = CipherMode.CBC;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, _settingsConfiguration.SettingEncryptionConfiguration.InitVectorBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }
    }
}