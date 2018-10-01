using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

//This code is got from http://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp

namespace Abp.Runtime.Security
{
    /// <summary>
    /// Can be used to simply encrypt/decrypt texts.
    /// </summary>
    public class SimpleStringCipher
    {
        public static SimpleStringCipher Instance { get; }

        /// <summary>
        /// This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
        /// This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        /// 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        /// </summary>
        public byte[] InitVectorBytes;

        /// <summary>
        /// Default password to encrypt/decrypt texts.
        /// It's recommented to set to another value for security.
        /// Default value: "gsKnGZ041HLL4IM8"
        /// </summary>
        public static string DefaultPassPhrase { get; set; }

        /// <summary>
        /// Default value: Encoding.ASCII.GetBytes("jkE49230Tf093b42")
        /// </summary>
        public static byte[] DefaultInitVectorBytes { get; set; }

        /// <summary>
        /// Default value: Encoding.ASCII.GetBytes("hgt!16kl")
        /// </summary>
        public static byte[] DefaultSalt { get; set; }

        /// <summary>
        /// This constant is used to determine the keysize of the encryption algorithm.
        /// </summary>
        public const int Keysize = 256;

        static SimpleStringCipher()
        {
            DefaultPassPhrase = "gsKnGZ041HLL4IM8";
            DefaultInitVectorBytes = Encoding.ASCII.GetBytes("jkE49230Tf093b42");
            DefaultSalt = Encoding.ASCII.GetBytes("hgt!16kl");
            Instance = new SimpleStringCipher();
        }

        public SimpleStringCipher()
        {
            InitVectorBytes = DefaultInitVectorBytes;
        }

        public string Encrypt(string plainText, string passPhrase = null, byte[] salt = null)
        {
            if (plainText == null)
            {
                return null;
            }

            if (passPhrase == null)
            {
                passPhrase = DefaultPassPhrase;
            }

            if (salt == null)
            {
                salt = DefaultSalt;
            }

            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, salt))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = Aes.Create())
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

        public string Decrypt(string cipherText, string passPhrase = null, byte[] salt = null)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return null;
            }

            if (passPhrase == null)
            {
                passPhrase = DefaultPassPhrase;
            }

            if (salt == null)
            {
                salt = DefaultSalt;
            }

            var cipherTextBytes = Convert.FromBase64String(cipherText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, salt))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = Aes.Create())
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
