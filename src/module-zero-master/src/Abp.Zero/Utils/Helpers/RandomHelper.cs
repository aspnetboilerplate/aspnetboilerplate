using System;
using System.Text;

namespace Abp.Utils.Helpers
{
    /// <summary>
    /// Internally used to generate random numbers and codes.
    /// </summary>
    internal class RandomHelper
    {
        private static readonly Random Rnd = new Random();

        private const string CodeChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string GenerateCode(int length)
        {
            var codeBuilder = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                codeBuilder.Append(CodeChars[GenerateNumber(0, CodeChars.Length)]);
            }

            return codeBuilder.ToString();
        }

        public static int GenerateNumber(int min, int max)
        {
            lock (Rnd)
            {
                return Rnd.Next(min, max);
            }
        }
    }
}