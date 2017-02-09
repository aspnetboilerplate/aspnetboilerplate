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
        public static SimpleStringCipher Instance { get; } = new SimpleStringCipher();

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
        public static string DefaultPassPhrase { get; set; } = "gsKnGZ041HLL4IM8";

        /// <summary>
        /// Default value: Encoding.ASCII.GetBytes("jkE49230Tf093b42")
        /// </summary>
        public static byte[] DefaultInitVectorBytes { get; set; } = Encoding.ASCII.GetBytes("jkE49230Tf093b42");

        /// <summary>
        /// This constant is used to determine the keysize of the encryption algorithm.
        /// </summary>
        public const int Keysize = 256;

        public SimpleStringCipher()
        {
            InitVectorBytes = DefaultInitVectorBytes;
        }

        public string Encrypt(string plainText, string passPhrase = null)
        {
            if (plainText == null)
            {
                return null;
            }

            if (passPhrase == null)
            {
                passPhrase = DefaultPassPhrase;
            }

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null))
            {
                byte[] keyBytes = password.GetBytes(Keysize / 8);
                using (RijndaelManaged symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, InitVectorBytes))
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                byte[] cipherTextBytes = memoryStream.ToArray();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public string Decrypt(string cipherText, string passPhrase = null)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return null;
            }

            if (passPhrase == null)
            {
                passPhrase = DefaultPassPhrase;
            }

            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            using (PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null))
            {
                byte[] keyBytes = password.GetBytes(Keysize / 8);
                using (RijndaelManaged symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, InitVectorBytes))
                    {
                        using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }
    }
}
