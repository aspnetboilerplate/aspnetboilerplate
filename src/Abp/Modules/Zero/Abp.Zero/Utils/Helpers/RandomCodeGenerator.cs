using System;
using System.Text;

namespace Abp.Utils.Helpers
{
    internal class RandomCodeGenerator
    {
        private static readonly Random Rnd = new Random();

        private const string CodeChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string Generate(int length)
        {
            var codeBuilder = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                codeBuilder.Append(CodeChars[GetRandomNumber(0, CodeChars.Length)]);
            }

            return codeBuilder.ToString();
        }

        private static int GetRandomNumber(int min, int max)
        {
            lock (Rnd)
            {
                return Rnd.Next(min, max);
            }
        }
    }
}