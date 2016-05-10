using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

//This code is got from http://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp

namespace Abp.Runtime.Security
{
    /// <summary>
    ///     Can be used to simply encrypt/decrypt texts.
    /// </summary>
    public class SimpleStringCipher
    {
        /// <summary>
        ///     Default password to encrypt/decrypt texts.
        /// </summary>
        public const string DefaultPassPhrase = "gsKnGZ041HLL4IM8";

        /// <summary>
        ///     This constant is used to determine the keysize of the encryption algorithm.
        /// </summary>
        public const int Keysize = 256;

        /// <summary>
        ///     This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
        ///     This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        ///     32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        /// </summary>
        public byte[] InitVectorBytes;

        public SimpleStringCipher()
        {
            InitVectorBytes = Encoding.ASCII.GetBytes("jkE49230Tf093b42");
        }

        public static SimpleStringCipher Instance { get; } = new SimpleStringCipher();

        public string Encrypt(string plainText, string passPhrase = DefaultPassPhrase)
        {
            if (plainText == null)
            {
                return null;
            }

            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new PasswordDeriveBytes(passPhrase, null))
            {
                var keyBytes = password.GetBytes(Keysize/8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, InitVectorBytes))
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

        public string Decrypt(string cipherText, string passPhrase = DefaultPassPhrase)
        {
            if (cipherText == null)
            {
                return null;
            }

            var cipherTextBytes = Convert.FromBase64String(cipherText);
            using (var password = new PasswordDeriveBytes(passPhrase, null))
            {
                var keyBytes = password.GetBytes(Keysize/8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, InitVectorBytes))
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